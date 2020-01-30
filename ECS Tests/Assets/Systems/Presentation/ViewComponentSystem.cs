using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(BeginViewSystem))]
[UpdateBefore(typeof(EndViewSystem))]
[UpdateInGroup(typeof(PresentationSystemGroup))]
public abstract class ViewComponentSystem : ComponentSystem
{
    WorldMaster _worldMaster;
    BeginViewSystem _beginViewSystem;
    EndSimulationEntityCommandBufferSystem _aSimSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _worldMaster = World.GetOrCreateSystem<WorldMaster>();
        _beginViewSystem = World.GetOrCreateSystem<BeginViewSystem>();
        
        _aSimSystem = SimWorld.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected World SimWorld => _worldMaster.SimulationWorld;
    protected ExclusiveEntityTransaction ExclusiveSimWorld => _beginViewSystem.ExclusiveSimWorld;
    protected ComponentSystem SimSystem => _aSimSystem;
}

[UpdateAfter(typeof(BeginViewSystem))]
[UpdateBefore(typeof(EndViewSystem))]
[UpdateInGroup(typeof(PresentationSystemGroup))]
public abstract class ViewJobComponentSystem : JobComponentSystem
{
    WorldMaster _worldMaster;
    BeginViewSystem _beginViewSystem;
    EndSimulationEntityCommandBufferSystem _aSimSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _worldMaster = World.GetOrCreateSystem<WorldMaster>();
        _beginViewSystem = World.GetOrCreateSystem<BeginViewSystem>();

        _aSimSystem = SimWorld.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected World SimWorld => _worldMaster.SimulationWorld;
    protected ExclusiveEntityTransaction SimWorldForJobs => _beginViewSystem.ExclusiveSimWorld;
    protected ComponentSystem SimSystem => _aSimSystem;
}
