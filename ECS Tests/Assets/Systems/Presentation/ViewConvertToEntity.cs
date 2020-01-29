using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ViewConvertToEntity : ConvertToEntity
{
    public SimConvertToEntity ObservedSimEntity;

    void Awake()
    {
        //if (WorldMaster.UIWorld != null)
        //{
        //    var system = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>();
        //    system.AddToBeConverted(WorldMaster.UIWorld, this);
        //}
        //else
        //{
        //    UnityEngine.Debug.LogWarning($"{nameof(ConvertToEntity)} failed because there is no {nameof(World.DefaultGameObjectInjectionWorld)}", this);
        //}
    }

    private void Start()
    {
        WorldMaster.AddToConvert(this);
    }
}
