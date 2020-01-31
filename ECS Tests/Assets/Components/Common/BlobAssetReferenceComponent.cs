using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct BlobAssetReferenceComponent<T> : IComponentData
        where T : struct
{
    public BlobAssetReference<T> Value;
}
