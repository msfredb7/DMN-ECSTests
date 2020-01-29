using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class ViewTransformSystem : ViewJobComponentSystem
{
    EntityQuery _simTranslations;

    protected override JobHandle OnUpdate(JobHandle jobHandle)
    {
        if (WorldMaster.SimulationWorld == null)
            return jobHandle;

        if (_simTranslations == null)
        {
            _simTranslations = WorldMaster.SimulationWorld.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<Translation>());
        }

        return new ViewTransformJob()
        {
            SimTranslations = WorldMaster.SimulationWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().GetComponentDataFromEntity<Translation>()
        }.Schedule(this, jobHandle);
    }

    struct ViewTransformJob : IJobForEach<Translation, LinkedSimEntity>
    {
        [ReadOnly] public ComponentDataFromEntity<Translation> SimTranslations;

        public void Execute(ref Translation translation, [ReadOnly] ref LinkedSimEntity linkedSimEntity)
        {
            if (SimTranslations.Exists(linkedSimEntity.SimWorldEntity))
            {
                translation.Value = SimTranslations[linkedSimEntity.SimWorldEntity].Value;
            }
        }
    }
}