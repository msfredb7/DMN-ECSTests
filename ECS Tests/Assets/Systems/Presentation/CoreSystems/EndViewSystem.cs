﻿using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(BeginViewSystem))]
[UpdateInGroup(typeof(PresentationSystemGroup))]
public class EndViewSystem : ComponentSystem
{
    SimulationWorldSystem _worldMaster;

    protected override void OnCreate()
    {
        base.OnCreate();

        _worldMaster = World.GetOrCreateSystem<SimulationWorldSystem>();
    }

    protected override void OnUpdate()
    {
        World.EntityManager.CompleteAllJobs();
        _worldMaster.SimulationWorld.EntityManager.EndExclusiveEntityTransaction();
    }
}
