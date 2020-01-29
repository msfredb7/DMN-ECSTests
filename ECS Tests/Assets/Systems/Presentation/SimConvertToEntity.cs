using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SimConvertToEntity : ConvertToEntity
{
    void Awake()
    {
        WorldMaster.AddToConvert(this);
    }
}
