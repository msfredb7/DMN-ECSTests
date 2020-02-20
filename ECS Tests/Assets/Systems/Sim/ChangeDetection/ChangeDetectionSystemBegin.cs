using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

public class ChangeDetectionSystemBegin : ComponentSystem
{
    public bool HasUpdatedAtLeastOnce = false;
    public Dictionary<ComponentType, uint> SummedVersionNumbers = new Dictionary<ComponentType, uint>();

    protected override void OnUpdate()
    {
        HasUpdatedAtLeastOnce = true;
        ChangeDetectionSystemUtility.GatherAllVersionNumbers(EntityManager, SummedVersionNumbers);
    }
}
