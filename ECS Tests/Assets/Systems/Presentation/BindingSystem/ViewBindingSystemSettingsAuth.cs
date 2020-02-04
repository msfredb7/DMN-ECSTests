using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class ViewBindingSystemSettingsAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public List<BlueprintDefinition> BlueprintDefinitions;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        foreach (var item in BlueprintDefinitions)
            referencedPrefabs.Add(item.GetViewGameObject());
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BlobAssetReferenceComponent<ViewBindingSystemSettings>()
        {
            Value = CreateBlobAsset(conversionSystem)
        });
    }

    BlobAssetReference<ViewBindingSystemSettings> CreateBlobAsset(GameObjectConversionSystem conversionSystem)
    {
        BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp);

        ref ViewBindingSystemSettings root = ref blobBuilder.ConstructRoot<ViewBindingSystemSettings>();

        var ids = blobBuilder.Allocate(ref root.BlueprintIds, BlueprintDefinitions.Count);
        var viewEntities = blobBuilder.Allocate(ref root.BlueprintPresentationEntities, BlueprintDefinitions.Count);

        for (int i = 0; i < BlueprintDefinitions.Count; i++)
        {
            ids[i] = BlueprintDefinitions[i].GetBlueprintId().Value;
            viewEntities[i] = conversionSystem.GetPrimaryEntity(BlueprintDefinitions[i].GetViewGameObject());
        }

        BlobAssetReference<ViewBindingSystemSettings> blobRef = blobBuilder.CreateBlobAssetReference<ViewBindingSystemSettings>(Allocator.Persistent);

        blobBuilder.Dispose();

        return blobRef;
    }
}
