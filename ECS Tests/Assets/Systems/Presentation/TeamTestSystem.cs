using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class TeamTestSystem : ViewJobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle jobHandle)
    {
        //if (WorldMaster.SimulationWorld == null)
            return jobHandle;


        //return new TeamTestJob()
        //{
        //    SimTranslations = WorldMaster.SimulationWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().GetComponentDataFromEntity<Translation>()
        //}.Schedule(this, jobHandle);
    }

    struct TeamTestJob : IJobForEach<Team, LinkedSimEntity>
    {
        [ReadOnly] public ComponentDataFromEntity<Translation> SimTranslations;

        public void Execute(ref Team team, [ReadOnly] ref LinkedSimEntity linkedSimEntity)
        {
            float t = 0;
            for (int i = 0; i < 1000; i++)
            {
                if (SimTranslations.Exists(linkedSimEntity.SimWorldEntity))
                {
                    t += SimTranslations[linkedSimEntity.SimWorldEntity].Value.x;
                    team.Value = (t % 30 > 10) ? Team.Type.Alliance : Team.Type.Horde;
                }
                else
                {
                    team.Value = Team.Type.Horde;
                }
            }
        }
    }
}
