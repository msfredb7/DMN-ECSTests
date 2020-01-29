using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

/*
public class LogOnCollisionSystem : JobComponentSystem
{
    struct LogOnCollisionJob : ICollisionReactionJob
    {
        public double Time;
        [BurstDiscard]
        public void Execute(in CollisionData collision)
        {
            UnityEngine.Debug.Log($"{Time}: collision between {collision.EntityA} and {collision.EntityB}");
        }
    }
    struct LogOnCollisionEnterJob : ICollisionEnterReactionJob
    {
        public double Time;
        [BurstDiscard]
        public void Execute(in CollisionEnterData collision)
        {
            UnityEngine.Debug.Log($"{Time}: collision enter between {collision.EntityA} and {collision.EntityB}");
        }
    }
    struct LogOnCollisionExitJob : ICollisionExitReactionJob
    {
        public double Time;
        [BurstDiscard]
        public void Execute(in CollisionExitData collision)
        {
            UnityEngine.Debug.Log($"{Time}: collision exit between {collision.EntityA} and {collision.EntityB}");
        }
    }

    CollisionSystem _collisionSystem;

    protected override void OnCreate()
    {
        _collisionSystem = World.GetOrCreateSystem<CollisionSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        LogOnCollisionEnterJob logOnEnterJob = new LogOnCollisionEnterJob() { Time = Time.ElapsedTime };
        LogOnCollisionExitJob logOnExitJob = new LogOnCollisionExitJob() { Time = Time.ElapsedTime };
        LogOnCollisionJob logOnStayJob = new LogOnCollisionJob() { Time = Time.ElapsedTime };

        return JobHandle.CombineDependencies(
            logOnEnterJob.Schedule(_collisionSystem, inputDependencies),
            logOnExitJob.Schedule(_collisionSystem, inputDependencies));
    }
}*/