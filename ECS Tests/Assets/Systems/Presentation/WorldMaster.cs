using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Entities;
using UnityEngine;

public class WorldMaster : ComponentSystem
{
    public World SimulationWorld { get; private set; }

    static Dictionary<SimConvertToEntity, ViewConvertToEntity> ToConvert = new Dictionary<SimConvertToEntity, ViewConvertToEntity>();
    static List<ViewConvertToEntity> LoneViewsToConvert = new List<ViewConvertToEntity>();

    static GameObjectConversionSettings _simConversionSettings;
    static GameObjectConversionSettings _viewConversionSettings;

    bool updatePlayerLoop = false;

    protected override void OnCreate()
    {
        base.OnCreate();

        SimulationWorld = new World("Simulation World");
        InitializationSystemGroup initGroup = SimulationWorld.CreateSystem<InitializationSystemGroup>();
        SimulationSystemGroup simGroup = SimulationWorld.CreateSystem<SimulationSystemGroup>();
        PresentationSystemGroup presGroup = SimulationWorld.CreateSystem<PresentationSystemGroup>();

        initGroup.AddSystemToUpdateList(SimulationWorld.CreateSystem<BeginInitializationEntityCommandBufferSystem>());
        initGroup.AddSystemToUpdateList(SimulationWorld.CreateSystem<EndInitializationEntityCommandBufferSystem>());
        initGroup.AddSystemToUpdateList(SimulationWorld.CreateSystem<UpdateWorldTimeSystem>());

        simGroup.AddSystemToUpdateList(SimulationWorld.CreateSystem<BeginSimulationEntityCommandBufferSystem>());
        simGroup.AddSystemToUpdateList(SimulationWorld.CreateSystem<EndSimulationEntityCommandBufferSystem>());
        foreach (Type systemType in GetTypesDerivedFrom(typeof(SimComponentSystem)))
        {
            simGroup.AddSystemToUpdateList(SimulationWorld.CreateSystem(systemType));
        }
        foreach (Type systemType in GetTypesDerivedFrom(typeof(SimJobComponentSystem)))
        {
            simGroup.AddSystemToUpdateList(SimulationWorld.CreateSystem(systemType));
        }

        presGroup.AddSystemToUpdateList(SimulationWorld.CreateSystem<BeginPresentationEntityCommandBufferSystem>());
        //presGroup.AddSystemToUpdateList(SimulationWorld.CreateSystem<SimPresentationSystemAccess>());

        initGroup.SortSystemUpdateList();
        simGroup.SortSystemUpdateList();
        presGroup.SortSystemUpdateList();

        updatePlayerLoop = true;
        //ScriptBehaviourUpdateOrder.UpdatePlayerLoop(_simulationWorld, ScriptBehaviourUpdateOrder.CurrentPlayerLoop);

        _simConversionSettings = new GameObjectConversionSettings(SimulationWorld, GameObjectConversionUtility.ConversionFlags.AssignName);
        _viewConversionSettings = new GameObjectConversionSettings(World.DefaultGameObjectInjectionWorld, GameObjectConversionUtility.ConversionFlags.AssignName);
    }

    protected override void OnUpdate()
    {
        if (ToConvert.Count > 0 || LoneViewsToConvert.Count > 0)
            Convert();

        if (updatePlayerLoop)
        {
            ScriptBehaviourUpdateOrder.UpdatePlayerLoop(SimulationWorld, ScriptBehaviourUpdateOrder.CurrentPlayerLoop);
            updatePlayerLoop = false;
        }

        //_simulationWorld.Update();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (SimulationWorld.IsCreated)
            SimulationWorld.Dispose();
        SimulationWorld = null;
    }

    void Convert()
    {
        foreach (var item in ToConvert)
        {
            SimConvertToEntity simGO = item.Key;
            ViewConvertToEntity viewGO = item.Value;

            Entity simEntity = ConvertGameObject(simGO, _simConversionSettings);

            if (viewGO != null)
            {
                Entity viewEntity = ConvertGameObject(viewGO, _viewConversionSettings);
                _viewConversionSettings.DestinationWorld.EntityManager.AddComponentData(viewEntity, new BindedSimEntity()
                {
                    SimWorldEntity = simEntity
                });
            }
        }

        foreach (ViewConvertToEntity item in LoneViewsToConvert)
        {
            ConvertGameObject(item, _viewConversionSettings);
        }

        ToConvert.Clear();
        LoneViewsToConvert.Clear();
    }

    public static void AddToConvert(SimConvertToEntity simTarget)
    {
        if (!ToConvert.ContainsKey(simTarget))
            ToConvert.Add(simTarget, null);
    }

    public static void AddToConvert(ViewConvertToEntity viewTarget)
    {
        if (viewTarget.ObservedSimEntity == null)
        {
            Debug.LogWarning($"Gameobject '{viewTarget.gameObject.name}' has no SimLink. It is recommanded to use a simple ConvertToEntity component instead" +
                $" of a ViewConvertToEntity component");

            LoneViewsToConvert.Add(viewTarget);
        }
        else
        {
            if (ToConvert.ContainsKey(viewTarget.ObservedSimEntity))
            {
                if (ToConvert[viewTarget.ObservedSimEntity] != null)
                    Debug.LogError($"Two view entities observe the same sim entity. " +
                        $"{ToConvert[viewTarget.ObservedSimEntity].gameObject.name} and {viewTarget.gameObject.name} observe {viewTarget.ObservedSimEntity.gameObject.name}");
                ToConvert[viewTarget.ObservedSimEntity] = viewTarget;
            }
            else
                ToConvert.Add(viewTarget.ObservedSimEntity, viewTarget);
        }
    }

    static Entity ConvertGameObject(ConvertToEntity root, GameObjectConversionSettings settings)
    {
        Entity entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(root.gameObject, settings);
        if (root.ConversionMode == ConvertToEntity.Mode.ConvertAndDestroy)
            UnityEngine.Object.DestroyImmediate(root.gameObject);

        return entity;
    }

    static IEnumerable<System.Type> GetTypesDerivedFrom(Type type)
    {
#if UNITY_EDITOR
        return UnityEditor.TypeCache.GetTypesDerivedFrom(type);
#else
        var types = new List<Type>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (!TypeManager.IsAssemblyReferencingEntities(assembly))
                continue;

            try
            {
                var assemblyTypes = assembly.GetTypes();
                foreach (var t in assemblyTypes)
                {
                    if (type.IsAssignableFrom(t))
                        types.Add(t);
                }
            }
            catch (ReflectionTypeLoadException e)
            {
                foreach (var t in e.Types)
                {
                    if (t != null && type.IsAssignableFrom(t))
                        types.Add(t);
                }

                Debug.LogWarning($"Failed loading assembly: {(assembly.IsDynamic ? assembly.ToString() : assembly.Location)}");
            }
        }

        return types;
#endif
    }
}
