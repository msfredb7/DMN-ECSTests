using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

/*[GenerateAuthoringComponent]*/ // doesn't work ...
public struct SharedVelocityMultiplier : ISharedComponentData
{
    public float Multiplier;
}
