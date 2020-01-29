using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

public struct BufferWithEntity<T> where T : struct, IBufferElementData
{
    public BufferWithEntity(Entity entity, BufferFromEntity<T> bufferFromEntity)
    {
        this.Entity = entity;
        this.BufferFromEntity = bufferFromEntity;
    }

    public BufferFromEntity<T> BufferFromEntity;
    [ReadOnly] public Entity Entity;

    public bool BufferExists() => BufferFromEntity.Exists(Entity);
    public DynamicBuffer<T> GetDynamicBuffer() => BufferFromEntity[Entity];
}

public static class BufferWithEntityExtensions
{
    public static BufferWithEntity<T> GetBufferWithEntity<T>(this ComponentSystemBase componentSystemBase, Entity entity, bool isReadOnly)
        where T : struct, IBufferElementData
    {
        return new BufferWithEntity<T>(entity, componentSystemBase.GetBufferFromEntity<T>(isReadOnly));
    }
}