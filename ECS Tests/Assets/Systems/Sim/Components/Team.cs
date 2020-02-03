using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct Team : IComponentData
{
    public enum Type
    {
        Alliance, Horde
    }

    public Type Value;
}
