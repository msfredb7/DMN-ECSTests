using Unity.Entities;

public class SimulationWorldSystem : ComponentSystem
{
    public World SimulationWorld { get; private set; }
    public World PresentationWorld => World;

    protected override void OnCreate()
    {
        base.OnCreate();

        SimulationWorld = new World("Simulation World");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (SimulationWorld.IsCreated)
            SimulationWorld.Dispose();
        SimulationWorld = null;
    }

    protected override void OnUpdate() { }
}

