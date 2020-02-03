using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct ItemReference : IComponentData
{
    public Entity ItemEntity;
}