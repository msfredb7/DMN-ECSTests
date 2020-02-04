using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class ConvertToEntityMultiWorld : ConvertToEntity
{
    public abstract GameWorldType WorldToConvertTo { get; }

    void Awake()
    {
        if (World.DefaultGameObjectInjectionWorld != null)
        {
            var convertSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>();
            convertSystem.AddToBeConverted(GetAssociatedWorld(), this);
        }
        else
        {
            Debug.LogWarning($"{nameof(ConvertToEntity)} failed because there is no {nameof(World.DefaultGameObjectInjectionWorld)}", this);
        }
    }

    World GetAssociatedWorld()
    {
        var worldMasterSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<WorldMaster>();
        switch (WorldToConvertTo)
        {
            case GameWorldType.Simulation:
                return worldMasterSystem.SimulationWorld;
            case GameWorldType.Presentation:
                return worldMasterSystem.PresentationWorld;
            default:
                return null;
        }
    }
}
