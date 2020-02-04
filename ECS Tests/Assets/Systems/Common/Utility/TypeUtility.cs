using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TypeUtility
{
    public static IEnumerable<Type> GetECSTypesDerivedFrom(Type type)
    {
#if UNITY_EDITOR
        return UnityEditor.TypeCache.GetTypesDerivedFrom(type);
#else
        var types = new List<Type>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (!TypeManager.IsAssemblyReferencingEntities(assembly))
                continue;

            try
            {
                var assemblyTypes = assembly.GetTypes();
                foreach (var t in assemblyTypes)
                {
                    if (type.IsAssignableFrom(t))
                        types.Add(t);
                }
            }
            catch (ReflectionTypeLoadException e)
            {
                foreach (var t in e.Types)
                {
                    if (t != null && type.IsAssignableFrom(t))
                        types.Add(t);
                }

                Debug.LogWarning($"Failed loading assembly: {(assembly.IsDynamic ? assembly.ToString() : assembly.Location)}");
            }
        }

        return types;
#endif
    }
}
