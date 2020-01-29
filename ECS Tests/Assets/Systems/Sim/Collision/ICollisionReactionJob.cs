using UnityEngine;
using System.Collections;
using Unity.Jobs;

public interface ICollisionReactionJob : IReactionJob<CollisionData>
{
}
public interface ICollisionEnterReactionJob : IReactionJob<CollisionEnterData>
{
}
public interface ICollisionExitReactionJob : IReactionJob<CollisionExitData>
{
}

public static class ICollisionEventJobExtensions
{
    public static JobHandle Schedule<T>(this T job, CollisionSystem collisionSystem, JobHandle inputDepenencies)
        where T : struct, ICollisionReactionJob
    {
        return collisionSystem.ScheduleCollisionReactionJob(job, inputDepenencies);
    }
    public static void Execute<T>(this T job, CollisionSystem collisionSystem, JobHandle inputDepenencies)
        where T : struct, ICollisionReactionJob
    {
        collisionSystem.ExecuteCollisionReactionJob(job, inputDepenencies);
    }
}
public static class ICollisionEnterEventJobExtensions
{
    public static JobHandle Schedule<T>(this T job, CollisionSystem collisionSystem, JobHandle inputDepenencies)
        where T : struct, ICollisionEnterReactionJob
    {
        return collisionSystem.ScheduleCollisionEnterReactionJob(job, inputDepenencies);
    }
    public static void Execute<T>(this T job, CollisionSystem collisionSystem, JobHandle inputDepenencies)
        where T : struct, ICollisionEnterReactionJob
    {
        collisionSystem.ExecuteCollisionEnterReactionJob(job, inputDepenencies);
    }
}
public static class ICollisionExitEventJobExtensions
{
    public static JobHandle Schedule<T>(this T job, CollisionSystem collisionSystem, JobHandle inputDepenencies)
        where T : struct, ICollisionExitReactionJob
    {
        return collisionSystem.ScheduleCollisionExitReactionJob(job, inputDepenencies);
    }
    public static void Execute<T>(this T job, CollisionSystem collisionSystem, JobHandle inputDepenencies)
        where T : struct, ICollisionExitReactionJob
    {
        collisionSystem.ExecuteCollisionExitReactionJob(job, inputDepenencies);
    }
}