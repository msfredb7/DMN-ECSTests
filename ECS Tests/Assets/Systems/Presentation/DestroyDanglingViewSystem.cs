using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class DestroyDanglingViewSystem : ViewJobComponentSystem
{
    struct DestroyDanglingViewSystemJob : IJobForEachWithEntity<LinkedSimEntity>
    {
        public EntityCommandBuffer.Concurrent Ecb;
        [ReadOnly] public ExclusiveEntityTransaction SimWorld;

        public void Execute(Entity viewEntity, int jobIndex, [ReadOnly] ref LinkedSimEntity linkedSimEntity)
        {
            if (!SimWorld.Exists(linkedSimEntity.SimWorldEntity))
            {
                Ecb.DestroyEntity(jobIndex, viewEntity);
            }
        }
    }

    private BeginInitializationEntityCommandBufferSystem _ecbSystem;
    private BeginViewSystem _beginViewSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        _ecbSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        _beginViewSystem = World.GetOrCreateSystem<BeginViewSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new DestroyDanglingViewSystemJob()
        {
            SimWorld = _beginViewSystem.ExclusiveSimWorld,
            Ecb = _ecbSystem.CreateCommandBuffer().ToConcurrent()
        };

        // Now that the job is set up, schedule it to be run. 
        return job.Schedule(this, inputDependencies);
    }
}