using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ConvertToSimEntity : ConvertToEntityMultiWorld
{
    public override GameWorldType WorldToConvertTo => GameWorldType.Simulation;
}
