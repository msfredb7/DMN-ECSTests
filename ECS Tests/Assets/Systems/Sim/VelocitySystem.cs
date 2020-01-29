using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class VelocitySystem : SimJobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;

        return Entities.ForEach(
            (ref Translation pos,
            in Velocity vel) =>
            {
                pos.Value += vel.Value * deltaTime;
            })
            .Schedule(inputDeps);
    }
}