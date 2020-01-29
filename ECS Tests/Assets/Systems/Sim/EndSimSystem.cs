using Unity.Entities;

public class EndSimSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        EntityManager.CompleteAllJobs();
    }
}
