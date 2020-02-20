using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Entities;

public static class ChangeDetectionSystemUtility
{
    public static void GatherAllVersionNumbers(EntityManager entityManager, Dictionary<ComponentType, uint> summedVersionNumbers)
    {
        summedVersionNumbers.Clear();

        using (NativeArray<ArchetypeChunk> chunks = entityManager.GetAllChunks(Allocator.TempJob)) // needs to be temp job to preven unity error :(
        {
            foreach (ArchetypeChunk chunk in chunks)
            {
                EntityArchetype archetype = chunk.Archetype;

                //StringBuilder s = new StringBuilder("archetype: ");

                using (NativeArray<ComponentType> componentTypes = archetype.GetComponentTypes(Allocator.Temp))
                {
                    foreach (ComponentType componentType in componentTypes)
                    {
                        //s.Append(componentType.GetManagedType());
                        //s.Append(" - ");
                        uint version = chunk.GetComponentVersion(componentType);
                        if (summedVersionNumbers.TryGetValue(componentType, out uint sum))
                        {
                            summedVersionNumbers[componentType] = sum + version;
                        }
                        else
                        {
                            summedVersionNumbers.Add(componentType, version);
                        }
                    }
                }

                //UnityEngine.Debug.Log(s.ToString());
            }
        }
    }
}
