using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[Serializable]
public struct CollisionData : IBufferElementData, IEquatable<CollisionData>
{
    public Entity EntityA;
    public Entity EntityB;

    public bool Equals(CollisionData other)
    {
        // we consider a collision with flipped values equal
        return (other.EntityA == EntityA && other.EntityB == EntityB)
            || (other.EntityB == EntityA && other.EntityA == EntityB);
    }
}

[Serializable]
public struct CollisionEnterData : IBufferElementData
{
    public Entity EntityA;
    public Entity EntityB;
}

[Serializable]
public struct CollisionExitData : IBufferElementData
{
    public Entity EntityA;
    public Entity EntityB;
}

[Serializable]
public struct CollisionSystemData : IComponentData
{
}