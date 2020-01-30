using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class LifeCycleTagSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        // Entities life cycle:
        // tick 0: Freshly created - no components
        // tick 1: NewlyCreatedTag + MidLifeCycleTag + MidLifeCycleSystemTag
        // tick 2:                   MidLifeCycleTag + MidLifeCycleSystemTag
        // ...
        // (destroy!)
        // tick 38:                                    MidLifeCycleSystemTag                        <- NB:  here the entity lost its MidLifeCycleTag because it is not a ISystemStateComponentData
        // tick 39:                                                            NewlyDestroyedTag
        // tick 40: (entity truly destroyed)


        // Each job represents one of those transformations
        // They should be scheduled in reverse order so that we let 1 tick happen inbetween some transformations
        //  (e.g. NewlyCreatedTag should stay for 1 tick before getting removed)

        // Remove NewlyDestroyedTag - truly destroying the entity
        Entities
            .WithAll<NewlyDestroyedTag>()
            .ForEach((Entity entity) =>
        {
            EntityManager.RemoveComponent<NewlyDestroyedTag>(entity);
        });

        // Change MidLifeCycleSystemTag for NewlyDestroyedTag
        Entities
            .WithAll<MidLifeCycleSystemTag>()
            .WithNone<MidLifeCycleTag>()
            .ForEach((Entity entity) =>
        {
            EntityManager.AddComponent<NewlyDestroyedTag>(entity);
            EntityManager.RemoveComponent<MidLifeCycleSystemTag>(entity);
        });

        // Remove NewlyCreatedTag
        Entities
            .WithAll<NewlyCreatedTag>()
            .ForEach((Entity entity) =>
        {
            EntityManager.RemoveComponent<NewlyCreatedTag>(entity);
        });

        // New entity! Add NewlyCreatedTag + MidLifeCycleTag + MidLifeCycleSystemTag
        Entities
            .WithNone<MidLifeCycleTag, MidLifeCycleSystemTag, NewlyDestroyedTag>()
            .ForEach((Entity entity) =>
        {
            EntityManager.AddComponent<NewlyCreatedTag>(entity);
            EntityManager.AddComponentData(entity, default(MidLifeCycleTag));
            EntityManager.AddComponent<MidLifeCycleSystemTag>(entity);
        });
    }
}