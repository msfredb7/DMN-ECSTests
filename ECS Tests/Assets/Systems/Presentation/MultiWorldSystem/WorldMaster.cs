using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

public class WorldMaster : ComponentSystem
{
    public struct ConversionUnit
    {
        public BlueprintId BlueprintId;
        public GameObject SimGO;
        public GameObject ViewGO;
        public GameObject MixedGO;
        public ConvertToEntity.Mode ConversionMode;
    }

    public World SimulationWorld { get; private set; }
    public World PresentationWorld => World;

    List<ConversionUnit> _toConvert = new List<ConversionUnit>();
    GameObjectConversionSettings _simConversionSettings;
    bool _updatePlayerLoop = false;

    [UnityEngine.ExecuteAlways]
    public class PreInitializationSystemGroup : ComponentSystemGroup { }

    protected override void OnCreate()
    {
        base.OnCreate();

        SimulationWorld = new World("Simulation World");
        PreInitializationSystemGroup preInitGroup = SimulationWorld.CreateSystem<PreInitializationSystemGroup>();
        InitializationSystemGroup initGroup = SimulationWorld.CreateSystem<InitializationSystemGroup>();
        SimulationSystemGroup simGroup = SimulationWorld.CreateSystem<SimulationSystemGroup>();
        PresentationSystemGroup presGroup = SimulationWorld.CreateSystem<PresentationSystemGroup>();

        preInitGroup.AddSystemToUpdateList(SimulationWorld.CreateSystem<ChangeDetectionSystemEnd>());

        initGroup.AddSystemToUpdateList(SimulationWorld.CreateSystem<BeginInitializationEntityCommandBufferSystem>());
        initGroup.AddSystemToUpdateList(SimulationWorld.CreateSystem<EndInitializationEntityCommandBufferSystem>());
        initGroup.AddSystemToUpdateList(SimulationWorld.CreateSystem<UpdateWorldTimeSystem>());

        simGroup.AddSystemToUpdateList(SimulationWorld.CreateSystem<BeginSimulationEntityCommandBufferSystem>());
        simGroup.AddSystemToUpdateList(SimulationWorld.CreateSystem<EndSimulationEntityCommandBufferSystem>());
        foreach (Type systemType in TypeUtility.GetECSTypesDerivedFrom(typeof(SimComponentSystem)))
        {
            simGroup.AddSystemToUpdateList(SimulationWorld.CreateSystem(systemType));
        }
        foreach (Type systemType in TypeUtility.GetECSTypesDerivedFrom(typeof(SimJobComponentSystem)))
        {
            simGroup.AddSystemToUpdateList(SimulationWorld.CreateSystem(systemType));
        }

        presGroup.AddSystemToUpdateList(SimulationWorld.CreateSystem<BeginPresentationEntityCommandBufferSystem>());
        presGroup.AddSystemToUpdateList(SimulationWorld.CreateSystem<ChangeDetectionSystemBegin>());

        preInitGroup.SortSystemUpdateList();
        initGroup.SortSystemUpdateList();
        simGroup.SortSystemUpdateList();
        presGroup.SortSystemUpdateList();

        _updatePlayerLoop = true;

        _simConversionSettings = new GameObjectConversionSettings(SimulationWorld, GameObjectConversionUtility.ConversionFlags.AssignName);
    }

    protected override void OnUpdate()
    {
        if (_toConvert.Count > 0)
            Convert();

        if (_updatePlayerLoop)
        {
            ScriptBehaviourUpdateOrderEx.AddWorldSystemGroupsIntoPlayerLoop(SimulationWorld, ScriptBehaviourUpdateOrder.CurrentPlayerLoop);
            _updatePlayerLoop = false;
        }

        //using (NativeArray<ArchetypeChunk> chunks = SimulationWorld.EntityManager.GetAllChunks(Unity.Collections.Allocator.TempJob))
        //{
        //    foreach (ArchetypeChunk chunk in chunks)
        //    {
        //        EntityArchetype archetype = chunk.Archetype;
        //        using (NativeArray<ComponentType> componentTypes = archetype.GetComponentTypes(Allocator.Temp))
        //        {
        //            foreach (ComponentType componentType in componentTypes)
        //            {
        //                uint version = chunk.GetComponentVersion(componentType);
        //                if (version != 1)
        //                    Debug.Log($"{componentType.GetManagedType()}'s version: {chunk.GetComponentVersion(componentType)}");
        //            }
        //        }
        //    }
        //}



        //Debug.Log($"Global versioning: {SimulationWorld.EntityManager.GlobalSystemVersion}");
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
        foreach (var item in _toConvert)
        {
            if (item.SimGO)
            {
                // create entity
                Entity simEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(item.SimGO, _simConversionSettings);

                // add blueprint id component
                if (item.BlueprintId != BlueprintId.Null)
                    _simConversionSettings.DestinationWorld.EntityManager.AddComponentData(simEntity, item.BlueprintId);
            }

            if (item.ConversionMode == ConvertToEntity.Mode.ConvertAndDestroy)
            {
                if (item.MixedGO)
                    UnityEngine.Object.DestroyImmediate(item.MixedGO);
                if (item.ViewGO)
                    UnityEngine.Object.DestroyImmediate(item.ViewGO);
                if (item.SimGO)
                    UnityEngine.Object.DestroyImmediate(item.SimGO);
            }
        }

        _toConvert.Clear();
    }



#if !UNITY_DOTSPLAYER
    static class ScriptBehaviourUpdateOrderEx
    {
        /// <summary>
        /// Update the player loop with a world's root-level systems
        /// </summary>
        /// <param name="world">World with root-level systems that need insertion into the player loop</param>
        /// <param name="existingPlayerLoop">Optional parameter to preserve existing player loops (e.g. ScriptBehaviourUpdateOrder.CurrentPlayerLoop)</param>
        public static void AddWorldSystemGroupsIntoPlayerLoop(World world, PlayerLoopSystem? existingPlayerLoop = null)
        {
            // TODO: PlayerLoop.GetCurrentPlayerLoop was added in 2019.3, so when minspec is updated revisit whether
            // we can drop the optional parameter
            var playerLoop = existingPlayerLoop ?? PlayerLoop.GetDefaultPlayerLoop();

            if (world != null)
            {
                // Insert the root-level systems into the appropriate PlayerLoopSystem subsystems:
                for (var i = 0; i < playerLoop.subSystemList.Length; ++i)
                {
                    int subsystemListLength = playerLoop.subSystemList[i].subSystemList.Length;
                    if (playerLoop.subSystemList[i].type == typeof(Update))
                    {
                        playerLoop.subSystemList[i].subSystemList =
                            InsertSystem<SimulationSystemGroup>(
                                world,
                                playerLoop.subSystemList[i].subSystemList,
                                playerLoop.subSystemList[i].subSystemList.Length - 1); // insert before default world system
                    }
                    else if (playerLoop.subSystemList[i].type == typeof(PreLateUpdate))
                    {
                        playerLoop.subSystemList[i].subSystemList =
                            InsertSystem<PresentationSystemGroup>(
                                world,
                                playerLoop.subSystemList[i].subSystemList,
                                playerLoop.subSystemList[i].subSystemList.Length - 1); // insert before default world system
                    }
                    else if (playerLoop.subSystemList[i].type == typeof(Initialization))
                    {
                        playerLoop.subSystemList[i].subSystemList =
                            InsertSystem<PreInitializationSystemGroup>(
                                world,
                                playerLoop.subSystemList[i].subSystemList,
                                playerLoop.subSystemList[i].subSystemList.Length - 1); // insert before default world system

                        playerLoop.subSystemList[i].subSystemList =
                            InsertSystem<InitializationSystemGroup>(
                                world,
                                playerLoop.subSystemList[i].subSystemList,
                                playerLoop.subSystemList[i].subSystemList.Length - 1); // insert before default world system
                    }
                }
            }

            ScriptBehaviourUpdateOrder.SetPlayerLoop(playerLoop);
        }

        static PlayerLoopSystem[] InsertSystem<T>(World world, PlayerLoopSystem[] oldArray, int insertIndex) 
            where T : ComponentSystemBase
        {
            T system = world.GetExistingSystem<T>();
            if (system == null)
                return oldArray;

            var newArray = new PlayerLoopSystem[oldArray.Length + 1];

            int o = 0;
            for (int n = 0; n < newArray.Length; ++n)
            {
                if (n == insertIndex)
                {
                    continue;
                }

                newArray[n] = oldArray[o];
                ++o;

            }
            ScriptBehaviourUpdateOrder.InsertManagerIntoSubsystemList<T>(newArray, insertIndex, world.GetOrCreateSystem<T>());

            return newArray;
        }
    }
#endif
}

