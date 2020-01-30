using UnityEngine;
using System.Collections;
using Unity.Entities;

public static class EntityExtensions
{
    public static bool Has<T>(this in Entity entity, EntityManager entityManager) where T : struct, IComponentData
    {
        return entityManager.HasComponent<T>(entity);
    }
    public static T Get<T>(this in Entity entity, EntityManager entityManager) where T : struct, IComponentData
    {
        return entityManager.GetComponentData<T>(entity);
    }
    public static void Set<T>(this in Entity entity, EntityManager entityManager, in T data) where T : struct, IComponentData
    {
        entityManager.SetComponentData(entity, data);
    }
    public static bool IsNull(this in Entity entity)
    {
        return entity == Entity.Null;
    }
    public static bool Exists(this in Entity entity, EntityManager entityManager)
    {
        return entityManager.Exists(entity);
    }
}
