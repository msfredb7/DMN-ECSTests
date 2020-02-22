using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(BeginViewSystem))]
[UpdateBefore(typeof(EndViewSystem))]
[UpdateInGroup(typeof(PresentationSystemGroup))]
public abstract class ViewComponentSystem : ComponentSystem
{
    protected SimWorldAccessor SimWorldAccessor { get; private set; }

    protected override void OnCreate()
    {
        base.OnCreate();

        World simWorld = World.GetOrCreateSystem<SimulationWorldSystem>().SimulationWorld;
        
        SimWorldAccessor = new SimWorldAccessor(
            simWorld: simWorld, 
            beginViewSystem: World.GetOrCreateSystem<BeginViewSystem>(), 
            someSimSystem: simWorld.GetExistingSystem<SimPreInitializationSystemGroup>()); // could be any system
    }
}

[UpdateAfter(typeof(BeginViewSystem))]
[UpdateBefore(typeof(EndViewSystem))]
[UpdateInGroup(typeof(PresentationSystemGroup))]
public abstract class ViewJobComponentSystem : JobComponentSystem
{
    protected SimWorldAccessor SimWorldAccessor { get; private set; }

    protected override void OnCreate()
    {
        base.OnCreate();

        World simWorld = World.GetOrCreateSystem<SimulationWorldSystem>().SimulationWorld;

        SimWorldAccessor = new SimWorldAccessor(
            simWorld: simWorld,
            beginViewSystem: World.GetOrCreateSystem<BeginViewSystem>(),
            someSimSystem: simWorld.GetExistingSystem<SimPreInitializationSystemGroup>()); // could be any system
    }
}
