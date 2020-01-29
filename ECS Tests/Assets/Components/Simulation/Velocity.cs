using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct Velocity : IComponentData, IValueComponent<float3>
{
    public float3 Value;

    float3 IValueComponent<float3>.Value { get => Value; set => Value = value; }
}
