using Unity.Entities;

public interface IValueComponent<T>
{
    T Value { get; set; }
}


public static class EntityIntValueExtensions
{
    public static int GetValue<T>(this in Entity entity, EntityManager entityManager)
        where T : struct, IComponentData, IValueComponent<int>
    {
        return entityManager.GetComponentData<T>(entity).Value;
    }
    public static void SetValue<T>(this in Entity entity, EntityManager entityManager, in int data)
        where T : struct, IComponentData, IValueComponent<int>
    {
        entityManager.SetComponentData(entity, new T() { Value = data });
    }
}

public static class EntityFloatValueExtensions
{
    public static float GetValue<T>(this in Entity entity, EntityManager entityManager)
        where T : struct, IComponentData, IValueComponent<float>
    {
        return entityManager.GetComponentData<T>(entity).Value;
    }
    public static void SetValue<T>(this in Entity entity, EntityManager entityManager, in float data)
        where T : struct, IComponentData, IValueComponent<float>
    {
        entityManager.SetComponentData(entity, new T() { Value = data });
    }
}