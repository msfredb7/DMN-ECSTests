using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public class BeginViewSystem : ComponentSystem
{
    [ReadOnly] public ExclusiveEntityTransaction ExclusiveSimWorld;

    WorldMaster _worldMaster;

    protected override void OnCreate()
    {
        base.OnCreate();

        _worldMaster = World.GetOrCreateSystem<WorldMaster>();
    }

    protected override void OnUpdate()
    {
        ExclusiveSimWorld = _worldMaster.SimulationWorld.EntityManager.BeginExclusiveEntityTransaction();
    }
}
