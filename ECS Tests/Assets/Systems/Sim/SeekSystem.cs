using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(VelocitySystem))]
public class SeekSystem : SimJobComponentSystem
{
    EndSimulationEntityCommandBufferSystem _ecbSystem;
    protected override void OnCreate()
    {
        base.OnCreate();
        // Find the ECB system once and store it for later usage
        _ecbSystem = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle jobHandle)
    {
        float deltaTime = Time.DeltaTime;

        var ecb = _ecbSystem.CreateCommandBuffer().ToConcurrent();

        jobHandle = Entities.ForEach((
            Entity entity, int entityInQueryIndex,
            ref Velocity velocity,
            in Translation position, in Destination destination, in MoveSpeed moveSpeed) =>
        {
            float3 dv = destination.Value - position.Value;
            if (dv.Equals(float3.zero))
            {
                velocity.Value = float3.zero;
                ecb.RemoveComponent<Destination>(entityInQueryIndex, entity);
            }
            else
            {
                float3 normalVel = math.normalize(dv) * moveSpeed.Value;
                float3 teleportToDestinationVelocity = dv / deltaTime;

                if (math.lengthsq(teleportToDestinationVelocity) < math.lengthsq(normalVel) || math.any(math.isnan(normalVel)))
                {
                    velocity.Value = teleportToDestinationVelocity;
                }
                else
                {
                    velocity.Value = normalVel;
                }
            }
        })
            .WithBurst()
            .Schedule(jobHandle);

        _ecbSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}