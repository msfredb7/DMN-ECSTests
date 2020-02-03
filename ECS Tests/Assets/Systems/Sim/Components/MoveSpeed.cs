using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct MoveSpeed : IComponentData, IValueComponent<float>
{
    public float Value;

    float IValueComponent<float>.Value { get => Value; set => Value = value; }
}