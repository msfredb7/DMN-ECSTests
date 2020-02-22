using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class ViewBindingSystem : ViewJobComponentSystem
{
    private EntityQuery _newSimEntitiesQ;
    private BeginPresentationEntityCommandBufferSystem _ecbSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _newSimEntitiesQ = SimWorldAccessor.CreateEntityQuery(ComponentType.ReadOnly<NewlyCreatedTag>(), ComponentType.ReadOnly<BlueprintId>());
        _ecbSystem = World.GetOrCreateSystem<BeginPresentationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<BlobAssetReferenceComponent<ViewBindingSystemSettings>>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var jobHandle = new FetchNewSimEntitiesJob()
        {
            Ecb = _ecbSystem.CreateCommandBuffer().ToConcurrent(),
            SettingsRef = GetSingleton<BlobAssetReferenceComponent<ViewBindingSystemSettings>>().Value
        }.Schedule(_newSimEntitiesQ, inputDependencies);

        _ecbSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }

    [BurstCompile]
    struct FetchNewSimEntitiesJob : IJobForEachWithEntity_ECC<NewlyCreatedTag, BlueprintId>
    {
        public EntityCommandBuffer.Concurrent Ecb;
        public BlobAssetReference<ViewBindingSystemSettings> SettingsRef;

        public void Execute(Entity simEntity, int index, [ReadOnly] ref NewlyCreatedTag c0, [ReadOnly] ref BlueprintId blueprintId)
        {
            ref var settings = ref SettingsRef.Value;

            for (int i = 0; i < settings.BlueprintIds.Length; i++)
            {
                if (settings.BlueprintIds[i] == blueprintId.Value)
                {
                    Entity presentationEntity = Ecb.Instantiate(index, settings.BlueprintPresentationEntities[i]);
                    Ecb.AddComponent(index, presentationEntity, new BindedSimEntity() { SimWorldEntity = simEntity });
                    break;
                }
            }
        }
    }
}