using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class ViewTransformSystem : ViewJobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle jobHandle)
    {
        return new ViewTransformJob()
        {
            SimTranslations = SimWorldAccessor.GetComponentDataFromEntity<Translation>()
        }.Schedule(this, jobHandle);
    }

    struct ViewTransformJob : IJobForEach<Translation, BindedSimEntity>
    {
        [ReadOnly] public ComponentDataFromEntity<Translation> SimTranslations;

        public void Execute(ref Translation translation, [ReadOnly] ref BindedSimEntity linkedSimEntity)
        {
            if (SimTranslations.Exists(linkedSimEntity.SimWorldEntity))
            {
                translation.Value = SimTranslations[linkedSimEntity.SimWorldEntity].Value;
            }
        }
    }
}