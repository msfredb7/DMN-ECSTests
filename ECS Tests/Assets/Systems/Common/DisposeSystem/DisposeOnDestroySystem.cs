using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class DisposeOnDestroySystem : ComponentSystem
{
    protected override void OnUpdate() { }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        MethodInfo toComponentDataArrayMethod = typeof(EntityQuery).GetMethod("ToComponentDataArray", new Type[] { typeof(Allocator) });
        if (toComponentDataArrayMethod == null)
        {
            Debug.LogError("EntityQuery.ToComponentDataArray<T> doesn't seem to exist anymore. Did the APi change?");
            return;
        }
        object[] parameters = new object[1] { Allocator.TempJob };

        TypeManager.TypeInfo[] typeInfos = TypeManager.GetAllTypes();
        for (int i = 0; i < typeInfos.Length; i++)
        {
            Type type = typeInfos[i].Type;

            if (type != null
                && typeof(IDisposeComponentDataOnWorldDestroy).IsAssignableFrom(type))
            {
                var query = EntityManager.CreateEntityQuery(ComponentType.FromTypeIndex(typeInfos[i].TypeIndex));
                if (query.CalculateEntityCount() > 0)
                {
                    object componentDataArray = toComponentDataArrayMethod.MakeGenericMethod(type).Invoke(query, parameters);
                    
                    foreach (var item in (IEnumerable)componentDataArray)
                    {
                        ((IDisposable)item).Dispose();
                    }
                    
                    if (componentDataArray is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }
    }
}
