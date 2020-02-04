using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Entities;
using UnityEngine;

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
    //GameObjectConversionSettings _viewConversionSettings;
    bool _updatePlayerLoop = false;

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
        foreach (Type systemType in TypeUtility.GetECSTypesDerivedFrom(typeof(SimComponentSystem)))
        {
            simGroup.AddSystemToUpdateList(SimulationWorld.CreateSystem(systemType));
        }
        foreach (Type systemType in TypeUtility.GetECSTypesDerivedFrom(typeof(SimJobComponentSystem)))
        {
            simGroup.AddSystemToUpdateList(SimulationWorld.CreateSystem(systemType));
        }

        presGroup.AddSystemToUpdateList(SimulationWorld.CreateSystem<BeginPresentationEntityCommandBufferSystem>());

        initGroup.SortSystemUpdateList();
        simGroup.SortSystemUpdateList();
        presGroup.SortSystemUpdateList();

        _updatePlayerLoop = true;
        //ScriptBehaviourUpdateOrder.UpdatePlayerLoop(_simulationWorld, ScriptBehaviourUpdateOrder.CurrentPlayerLoop);

        _simConversionSettings = new GameObjectConversionSettings(SimulationWorld, GameObjectConversionUtility.ConversionFlags.AssignName);
        // _viewConversionSettings = new GameObjectConversionSettings(World.DefaultGameObjectInjectionWorld, GameObjectConversionUtility.ConversionFlags.AssignName);
    }

    protected override void OnUpdate()
    {
        if (_toConvert.Count > 0)
            Convert();

        if (_updatePlayerLoop)
        {
            ScriptBehaviourUpdateOrder.UpdatePlayerLoop(SimulationWorld, ScriptBehaviourUpdateOrder.CurrentPlayerLoop);
            _updatePlayerLoop = false;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (SimulationWorld.IsCreated)
            SimulationWorld.Dispose();
        SimulationWorld = null;
    }

    //public void AddSceneGOToConvert(ConvertToEntityMultiWorld c)
    //{
    //    _toConvert.Add(c.ProvideConversionUnit());
    //}

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

            //if (item.ViewGO)
            //{
            //    // create entity
            //    viewEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(item.ViewGO, _viewConversionSettings);

            //    // add blueprint id component
            //    if (item.BlueprintId != BlueprintId.Null)
            //        _viewConversionSettings.DestinationWorld.EntityManager.AddComponentData(viewEntity, item.BlueprintId);

            //    // add binding
            //    _viewConversionSettings.DestinationWorld.EntityManager.AddComponentData(viewEntity, new BindedSimEntity()
            //    {
            //        SimWorldEntity = simEntity
            //    });
            //}


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
}
