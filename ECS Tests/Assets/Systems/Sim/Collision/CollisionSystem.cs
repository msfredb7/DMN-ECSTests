using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

[UpdateAfter(typeof(VelocitySystem))]
public class CollisionSystem : SimJobComponentSystem
{
    public NativeList<JobHandle> _propagationJobs;
    public JobHandle _collisionDetectionJobHandle;

    EntityQuery _collidersEntityQuery;

    protected override void OnCreate()
    {
        if (!HasSingleton<CollisionSystemData>())
        {
            EntityManager.CreateEntity(typeof(CollisionSystemData));

            // get singleton entity
            Entity singletonEntity = GetSingletonEntity<CollisionSystemData>();

            // add collision data array
            EntityManager.AddBuffer<CollisionData>(singletonEntity);
            EntityManager.AddBuffer<CollisionEnterData>(singletonEntity);
            EntityManager.AddBuffer<CollisionExitData>(singletonEntity);
        }

        _collidersEntityQuery = GetEntityQuery(
            ComponentType.ReadOnly<SquareCollider>(),
            ComponentType.ReadOnly<Translation>());

        _propagationJobs = new NativeList<JobHandle>(32, Allocator.Persistent);

        RequireForUpdate(_collidersEntityQuery);
    }
    protected override void OnDestroy()
    {
        _propagationJobs.Dispose();
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        // complete previous stuff
        _collisionDetectionJobHandle.Complete();
        for (int i = 0; i < _propagationJobs.Length; i++)
        {
            _propagationJobs[i].Complete();
        }
        _propagationJobs.Clear();

        // detect collisions
        Entity singletonEntity = GetSingletonEntity<CollisionSystemData>();
        CollisionDetectionJob detectionJob = new CollisionDetectionJob()
        {
            Entities = _collidersEntityQuery.ToEntityArray(Allocator.TempJob),
            Colliders = _collidersEntityQuery.ToComponentDataArray<SquareCollider>(Allocator.TempJob),
            Translations = _collidersEntityQuery.ToComponentDataArray<Translation>(Allocator.TempJob),
            CollisionDatasBufferWithEntity = this.GetBufferWithEntity<CollisionData>(singletonEntity, isReadOnly: false),
            CollisionEnterDatasBufferWithEntity = this.GetBufferWithEntity<CollisionEnterData>(singletonEntity, isReadOnly: false),
            CollisionExitDatasBufferWithEntity = this.GetBufferWithEntity<CollisionExitData>(singletonEntity, isReadOnly: false)
        };

        _collisionDetectionJobHandle = detectionJob.Schedule(inputDependencies);

        return _collisionDetectionJobHandle;
    }

    [BurstCompile]
    struct CollisionDetectionJob : IJob
    {
        [ReadOnly, DeallocateOnJobCompletion] public NativeArray<Entity> Entities;
        [ReadOnly, DeallocateOnJobCompletion] public NativeArray<SquareCollider> Colliders;
        [ReadOnly, DeallocateOnJobCompletion] public NativeArray<Translation> Translations;

        public BufferWithEntity<CollisionData> CollisionDatasBufferWithEntity;
        public BufferWithEntity<CollisionEnterData> CollisionEnterDatasBufferWithEntity;
        public BufferWithEntity<CollisionExitData> CollisionExitDatasBufferWithEntity;


        public void Execute()
        {
            DynamicBuffer<CollisionData> collisionsBuffer = CollisionDatasBufferWithEntity.GetDynamicBuffer();
            DynamicBuffer<CollisionEnterData> collisionEntersBuffer = CollisionEnterDatasBufferWithEntity.GetDynamicBuffer();
            DynamicBuffer<CollisionExitData> collisionExitsBuffer = CollisionExitDatasBufferWithEntity.GetDynamicBuffer();
            NativeArray<CollisionData> oldCollisions = collisionsBuffer.ToNativeArray(Allocator.Temp);

            collisionsBuffer.Clear();
            collisionEntersBuffer.Clear();
            collisionExitsBuffer.Clear();

            // gather new collisions
            for (int i = 0; i < Colliders.Length; i++)
            {
                for (int j = i + 1; j < Colliders.Length; j++)
                {
                    if (Intersects(
                        Colliders[i], Translations[i],
                        Colliders[j], Translations[j]))
                    {
                        // collision detected!
                        collisionsBuffer.Add(new CollisionData()
                        {
                            EntityA = Entities[i],
                            EntityB = Entities[j]
                        });
                    }
                }
            }

            // find new and old collision datas (NB: THIS COULD BE OPTIMIZED BY SORTING COLLISIONS)
            NativeArray<CollisionData> newCollisions = collisionsBuffer.AsNativeArray();
            for (int i = 0; i < oldCollisions.Length; i++)
            {
                if (!newCollisions.Contains(oldCollisions[i]))
                {
                    collisionExitsBuffer.Add(new CollisionExitData()
                    {
                        EntityA = oldCollisions[i].EntityA,
                        EntityB = oldCollisions[i].EntityB,
                    });
                }
            }

            for (int i = 0; i < newCollisions.Length; i++)
            {
                if (!oldCollisions.Contains(newCollisions[i]))
                {
                    collisionEntersBuffer.Add(new CollisionEnterData()
                    {
                        EntityA = newCollisions[i].EntityA,
                        EntityB = newCollisions[i].EntityB,
                    });
                }
            }


            oldCollisions.Dispose();
        }

        bool Intersects(
            in SquareCollider colA, in Translation posA,
            in SquareCollider colB, in Translation posB)
        {
            float3 posDiff = abs(posB.Value - posA.Value);
            float distanceBeforeCollidersTouch = (colA.Width + colB.Width) / 2f;

            return posDiff[0] < distanceBeforeCollidersTouch
                && posDiff[1] < distanceBeforeCollidersTouch
                && posDiff[2] < distanceBeforeCollidersTouch;
        }
    }

    #region Collision Reaction Scheduling
    EntityQuery _collisionDataPropagationEntityQuery;
    public JobHandle ScheduleCollisionReactionJob<J>(J job, JobHandle inputDependencies)
        where J : struct, ICollisionReactionJob
    {
        // create and cache query
        if (_collisionDataPropagationEntityQuery == null)
        {
            _collisionDataPropagationEntityQuery = GetEntityQuery(
                ComponentType.ReadOnly<CollisionSystemData>(),
                ComponentType.ReadOnly<CollisionData>());
        }

        return ScheduleCollisionReactionJob<J, CollisionData>(job, _collisionDataPropagationEntityQuery, inputDependencies);
    }

    EntityQuery _collisionEnterDataPropagationEntityQuery;
    public JobHandle ScheduleCollisionEnterReactionJob<J>(J job, JobHandle inputDependencies)
        where J : struct, ICollisionEnterReactionJob
    {
        // create and cache query
        if (_collisionEnterDataPropagationEntityQuery == null)
        {
            _collisionEnterDataPropagationEntityQuery = GetEntityQuery(
                ComponentType.ReadOnly<CollisionSystemData>(),
                ComponentType.ReadOnly<CollisionEnterData>());
        }

        return ScheduleCollisionReactionJob<J, CollisionEnterData>(job, _collisionEnterDataPropagationEntityQuery, inputDependencies);
    }

    EntityQuery _collisionExitDataPropagationEntityQuery;
    public JobHandle ScheduleCollisionExitReactionJob<J>(J job, JobHandle inputDependencies)
        where J : struct, ICollisionExitReactionJob
    {
        // create and cache query
        if (_collisionExitDataPropagationEntityQuery == null)
        {
            _collisionExitDataPropagationEntityQuery = GetEntityQuery(
                ComponentType.ReadOnly<CollisionSystemData>(),
                ComponentType.ReadOnly<CollisionExitData>());
        }

        return ScheduleCollisionReactionJob<J, CollisionExitData>(job, _collisionExitDataPropagationEntityQuery, inputDependencies);
    }

    JobHandle ScheduleCollisionReactionJob<J, T>(J job, EntityQuery entityQuery, JobHandle inputDependencies)
        where J : struct, IReactionJob<T>
        where T : struct, IBufferElementData
    {
        // add 'collision detection job' as a dependency
        JobHandle dep = JobHandle.CombineDependencies(inputDependencies, _collisionDetectionJobHandle);

        // create propagation job
        ReactionPropagationFromBufferJob<J, T> propagationJob = new ReactionPropagationFromBufferJob<J, T>()
        {
            Job = job
        };

        // schedule job and cache it
        JobHandle jobHandle = propagationJob.Schedule(entityQuery, dep);
        _propagationJobs.Add(jobHandle);
        return jobHandle;
    }


    public void ExecuteCollisionEnterReactionJob<J>(J job, JobHandle inputDependencies)
        where J : struct, ICollisionEnterReactionJob
    {
        ExecuteCollisionReactionJob<J, CollisionEnterData>(job, inputDependencies);
    }
    public void ExecuteCollisionReactionJob<J>(J job, JobHandle inputDependencies)
        where J : struct, ICollisionReactionJob
    {
        ExecuteCollisionReactionJob<J, CollisionData>(job, inputDependencies);
    }
    public void ExecuteCollisionExitReactionJob<J>(J job, JobHandle inputDependencies)
        where J : struct, ICollisionExitReactionJob
    {
        ExecuteCollisionReactionJob<J, CollisionExitData>(job, inputDependencies);
    }
    void ExecuteCollisionReactionJob<J, T>(J job, JobHandle inputDependencies)
        where J : struct, IReactionJob<T>
        where T : struct, IBufferElementData
    {
        // add 'collision detection job' as a dependency
        JobHandle dep = JobHandle.CombineDependencies(inputDependencies, _collisionDetectionJobHandle);
        dep.Complete();

        // create propagation job
        ReactionPropagationJob<J, T> propagationJob = new ReactionPropagationJob<J, T>()
        {
            Job = job,
            Data = EntityManager.GetBuffer<T>(GetSingletonEntity<CollisionSystemData>()).AsNativeArray()
        };

        // schedule job and cache it
        propagationJob.Execute();
    }
    #endregion

}