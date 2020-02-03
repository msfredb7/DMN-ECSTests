using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class ViewBindingSystemSettingsAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [System.Serializable]
    public struct BlueprintDefinition
    {
        public GameObject PresentationGameObject;
        public int BlueprintId;
    }

    public List<BlueprintDefinition> BlueprintDefinitions;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        foreach (var item in BlueprintDefinitions)
            referencedPrefabs.Add(item.PresentationGameObject);
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
        var presentationEntities = blobBuilder.Allocate(ref root.BlueprintPresentationEntities, BlueprintDefinitions.Count);

        for (int i = 0; i < BlueprintDefinitions.Count; i++)
        {
            ids[i] = BlueprintDefinitions[i].BlueprintId;
            presentationEntities[i] = conversionSystem.GetPrimaryEntity(BlueprintDefinitions[i].PresentationGameObject);
        }

        BlobAssetReference<ViewBindingSystemSettings> blobRef = blobBuilder.CreateBlobAssetReference<ViewBindingSystemSettings>(Allocator.Persistent);

        blobBuilder.Dispose();

        return blobRef;
    }
}
