using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct MidLifeCycleTag : IComponentData
{
}

[Serializable]
public struct MidLifeCycleSystemTag : ISystemStateComponentData
{
}