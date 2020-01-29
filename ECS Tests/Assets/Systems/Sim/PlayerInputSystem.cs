using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class PlayerInputSystem : SimJobComponentSystem
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
        float2 inputAxis = GetInputAxisDownValue();

        var ecb = _ecbSystem.CreateCommandBuffer().ToConcurrent();

        if (any(inputAxis != default(float2)))
        {
            jobHandle = Entities.ForEach((Entity entity, int entityInQueryIndex, in Translation position, in MoveDistance moveDistance) =>
            {
                ecb.AddComponent(entityInQueryIndex, entity,
                    new Destination()
                    {
                        Value = round(position.Value) + float3(inputAxis, 0) * moveDistance.Value
                    });
            })
                .WithAll<PawnTag>()
                .WithBurst()
                .Schedule(jobHandle);
        }

        _ecbSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }

    float2 GetInputAxisDownValue()
    {
        float2 result = default;

        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.UpArrow))
            result += float2(0, 1);
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.DownArrow))
            result += float2(0, -1);
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.LeftArrow))
            result += float2(-1, 0);
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.RightArrow))
            result += float2(1, 0);

        return result;
    }
}