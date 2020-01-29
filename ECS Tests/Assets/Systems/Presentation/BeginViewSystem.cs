using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public class BeginViewSystem : ComponentSystem
{
    public ExclusiveEntityTransaction ExclusiveSimWorld;

    protected override void OnUpdate()
    {
        if (WorldMaster.SimulationWorld == null)
            return;

        ExclusiveSimWorld = WorldMaster.SimulationWorld.EntityManager.BeginExclusiveEntityTransaction();
    }
}
