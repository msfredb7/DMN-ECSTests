﻿using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class DestroyDanglingViewSystem : ViewJobComponentSystem
{
    struct DestroyDanglingViewSystemJob : IJobForEachWithEntity<BindedSimEntity>
    {
        public EntityCommandBuffer.Concurrent Ecb;
        public SimWorldAccessorJob SimWorld;

        public void Execute(Entity viewEntity, int jobIndex, [ReadOnly] ref BindedSimEntity linkedSimEntity)
        {
            if (!SimWorld.Exists(linkedSimEntity.SimWorldEntity))
            {
                Ecb.DestroyEntity(jobIndex, viewEntity);
            }
        }
    }

    private BeginInitializationEntityCommandBufferSystem _ecbSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        _ecbSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var jobHandle = new DestroyDanglingViewSystemJob()
        {
            SimWorld = SimWorldAccessor.JobAccessor,
            Ecb = _ecbSystem.CreateCommandBuffer().ToConcurrent()
        }.Schedule(this, inputDependencies);

        _ecbSystem.AddJobHandleForProducer(jobHandle);
        
        // Now that the job is set up, schedule it to be run. 
        return jobHandle;
    }
}