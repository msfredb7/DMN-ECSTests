using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct EffectMoveDistanceBoost : IComponentData, IValueComponent<int>
{
    public int AdditionalMoveDistance;

    public int Value { get => AdditionalMoveDistance; set => AdditionalMoveDistance = value; }
}
