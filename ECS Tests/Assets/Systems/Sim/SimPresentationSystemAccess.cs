using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

// updates in presentation group
//public class SimPresentationSystemAccess : JobComponentSystem
//{
//    NativeList<JobHandle> _jobHandles;

//    protected override void OnCreate()
//    {
//        base.OnCreate();

//        _jobHandles = new NativeList<JobHandle>(Allocator.Persistent);
//    }

//    public JobHandle ScheduleJobOnSimulation<T>(T job, JobHandle inputDependencies) where T : struct, IBaseJobForEach
//    {
//        JobHandle handle = job.Schedule(inputDependencies);
//        _jobHandles.Add(handle);
//        return handle;
//    }

//    protected override JobHandle OnUpdate(JobHandle inputDependencies)
//    {
//        _jobHandles.Add(inputDependencies);

//        JobHandle combinedDependencies = JobHandle.CombineDependencies(_jobHandles.AsArray());

//        _jobHandles.Clear();

//        return combinedDependencies;
//    }
//}