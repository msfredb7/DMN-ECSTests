using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct BlobAssetReferenceComponent<T> : IComponentData, IDisposeComponentDataOnWorldDestroy
        where T : struct
{
    public BlobAssetReference<T> Value;

    public void Dispose()
    {
        if (Value.IsCreated)
            Value.Dispose();
    }
}
