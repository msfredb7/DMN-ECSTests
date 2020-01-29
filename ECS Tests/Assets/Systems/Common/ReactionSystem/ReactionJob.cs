using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public interface IReactionJob<T0>
{
    void Execute(in T0 arg1);
}

[BurstCompile]
public struct ReactionPropagationJob<U, T0> : IJob
    where U : IReactionJob<T0>
    where T0 : struct, IBufferElementData
{
    public U Job;
    [ReadOnly] public NativeArray<T0> Data;

    public void Execute()
    {
        for (int i = 0; i < Data.Length; i++)
        {
            Job.Execute(Data[i]);
        }
    }
}

[BurstCompile]
public struct ReactionPropagationFromBufferJob<J, T0> : IJobForEach_B<T0>
    where J : IReactionJob<T0>
    where T0 : struct, IBufferElementData
{
    public J Job;

    public void Execute([ReadOnly] DynamicBuffer<T0> data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            Job.Execute(data[i]);
        }
    }
}