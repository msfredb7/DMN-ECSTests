using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BlobAssetReferenceComponentCleanUpSystem : ComponentSystem
{
    protected override void OnUpdate() { }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
