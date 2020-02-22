using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ConvertToViewEntity : ConvertToEntityMultiWorld
{
    public override GameWorldType WorldToConvertTo => GameWorldType.Presentation;
}
