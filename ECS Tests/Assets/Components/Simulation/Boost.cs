using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[assembly: RegisterGenericComponentType(typeof(Boost<MoveSpeed>))]
[assembly: RegisterGenericComponentType(typeof(Boost<MoveDistance>))]

public struct Boost<T> : IComponentData
    where T : struct
{
    public int FlatIncrease;
    public float Multiplier;
}

public struct Boost : IComponentData
{
    public ComponentType ComponentType;
    public int FlatIncrease;
    public float Multiplier;
}