using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct EffectMoveSpeedBoost : IComponentData, IValueComponent<float>
{
    public float AdditionalMoveSpeed;

    public float Value { get => AdditionalMoveSpeed; set => AdditionalMoveSpeed = value; }
}