using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct ViewBindingSystemSettings : IComponentData
{
    public Entity Prefab;
}

//[Serializable]
//public struct ViewBindingSystemSettings : IComponentData
//{
//    public Entity Prefab;
//}