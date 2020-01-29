using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct MoveDistance : IComponentData, IValueComponent<int>
{
    public int Value;

    int IValueComponent<int>.Value { get => Value; set => Value = value; }
}