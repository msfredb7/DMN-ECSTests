using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(BeginViewSystem))]
[UpdateBefore(typeof(EndViewSystem))]
[UpdateInGroup(typeof(PresentationSystemGroup))]
public abstract class ViewComponentSystem : ComponentSystem
{
}

[UpdateAfter(typeof(BeginViewSystem))]
[UpdateBefore(typeof(EndViewSystem))]
[UpdateInGroup(typeof(PresentationSystemGroup))]
public abstract class ViewJobComponentSystem : JobComponentSystem
{
}
