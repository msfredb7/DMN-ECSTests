using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;


struct BlobTest
{

}

public class SimViewBindingSystem : ViewJobComponentSystem
{
    private EntityQuery _newSimEntitiesQ;
    private BeginPresentationEntityCommandBufferSystem _ecbSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _newSimEntitiesQ = SimWorld.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<NewlyCreatedTag>(), ComponentType.ReadOnly<BlueprintId>());
        _ecbSystem = World.GetOrCreateSystem<BeginPresentationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var jobHandle = new FetchNewSimEntitiesJob()
        {
            Ecb = _ecbSystem.CreateCommandBuffer().ToConcurrent(),
            VisualPrefab = EntityManager.GetComponentData<ViewBindingSystemSettings>(GetSingletonEntity<ViewBindingSystemSettings>()).Prefab
        }.Schedule(_newSimEntitiesQ, inputDependencies);

        _ecbSystem.AddJobHandleForProducer(jobHandle);

        BlobBuilder blobBuilder = new BlobBuilder();
        BlobTest blobTest = blobBuilder.ConstructRoot<BlobTest>();
         blobBuilder.CreateBlobAssetReference(Allocator.Persistent);

        return jobHandle;
    }

    [BurstCompile]
    struct FetchNewSimEntitiesJob : IJobForEachWithEntity_ECC<NewlyCreatedTag, BlueprintId>
    {
        public EntityCommandBuffer.Concurrent Ecb;
        public Entity VisualPrefab;

        public void Execute(Entity simEntity, int index, [ReadOnly] ref NewlyCreatedTag c0, [ReadOnly] ref BlueprintId c1)
        {
            Entity viewEntity = Ecb.Instantiate(index, VisualPrefab);
            Ecb.AddComponent(index, viewEntity, new BindedSimEntity() { SimWorldEntity = simEntity });
        }
    }
}