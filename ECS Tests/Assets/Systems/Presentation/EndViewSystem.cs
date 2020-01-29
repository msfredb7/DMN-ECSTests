using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(BeginViewSystem))]
[UpdateInGroup(typeof(PresentationSystemGroup))]
public class EndViewSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        if (WorldMaster.SimulationWorld == null)
            return;
        World.EntityManager.CompleteAllJobs();
        WorldMaster.SimulationWorld.EntityManager.EndExclusiveEntityTransaction();
    }
}
