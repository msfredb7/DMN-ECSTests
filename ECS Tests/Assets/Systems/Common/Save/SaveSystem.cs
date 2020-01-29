using System.ComponentModel;
using System.IO;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine;

[DisableAutoCreation] // REMOVE THIS TO ENABLE IT BACK
public class SaveSystem : ComponentSystem
{
    object[] objectTable;
    ReferencedUnityObjects referencedUnityObjects;

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(UnityEngine.KeyCode.S))
        {
            EntityManager.CompleteAllJobs();
            SaveWorldYAML(EntityManager);
            SaveWorld(EntityManager, out objectTable);
            SaveHybrid(EntityManager, out referencedUnityObjects);

            for (int i = 0; i < objectTable.Length; i++)
            {
                Debug.Log($"Referenced Object: {objectTable[i]}");
            }

        }

        if (Input.GetKeyDown(UnityEngine.KeyCode.L))
        {
            World loadWorld = new World("load world");
            EntityManager.CompleteAllJobs();
            LoadWorld(loadWorld.EntityManager, objectTable);
            EntityManager.CopyAndReplaceEntitiesFrom(loadWorld.EntityManager);
            loadWorld.Dispose();
        }
        if (Input.GetKeyDown(UnityEngine.KeyCode.H))
        {
            //EntityManager.GetAllArchetypes
            //Entity w = default;
            //using (NativeArray<ComponentType> components = EntityManager.GetComponentTypes(w))
            //{
            //    EntityManager.GetComponentObject components[0]
            //}

            World loadWorld = new World("load world");
            EntityManager.CompleteAllJobs();
            LoadHybrid(loadWorld.EntityManager, referencedUnityObjects);
            EntityManager.CopyAndReplaceEntitiesFrom(loadWorld.EntityManager);
            loadWorld.Dispose();
        }
    }

    static void SaveWorldYAML(EntityManager entityManager)
    {
        string filePath = Application.persistentDataPath + "/test.txt";

        if (File.Exists(filePath))
            File.Delete(filePath);

        using (StreamWriter writer = new StreamWriter(filePath, append: false, System.Text.Encoding.UTF8) { NewLine = "\n" })
        {
            writer.Flush();
            SerializeUtility.SerializeWorldIntoYAML(entityManager, writer, dumpChunkRawData: false);
        }
    }
    static void SaveWorld(EntityManager entityManager, out object[] objectTable)
    {
        string filePath = Application.persistentDataPath + "/test.bin";

        if (File.Exists(filePath))
            File.Delete(filePath);

        using (var binaryWriter = new StreamBinaryWriter(filePath))
        {
            SerializeUtility.SerializeWorld(entityManager, binaryWriter, out objectTable);
        }
    }
    static void SaveHybrid(EntityManager entityManager, out ReferencedUnityObjects objectTable)
    {
        string filePath = Application.persistentDataPath + "/testHyb.bin";

        if (File.Exists(filePath))
            File.Delete(filePath);

        using (var binaryWriter = new StreamBinaryWriter(filePath))
        {
            SerializeUtilityHybrid.Serialize(entityManager, binaryWriter, out objectTable);
        }
    }
    static void LoadWorld(EntityManager entityManager, object[] objectTable)
    {
        string filePath = Application.persistentDataPath + "/test.bin";

        using (var binaryReader = new StreamBinaryReader(filePath))
        {
            ExclusiveEntityTransaction transaction = entityManager.BeginExclusiveEntityTransaction();

            SerializeUtility.DeserializeWorld(transaction, binaryReader, objectTable);

            entityManager.EndExclusiveEntityTransaction();
        }
    }
    static void LoadHybrid(EntityManager entityManager, ReferencedUnityObjects objectTable)
    {
        string filePath = Application.persistentDataPath + "/testHyb.bin";

        using (var binaryReader = new StreamBinaryReader(filePath))
        {
            //ExclusiveEntityTransaction transaction = entityManager.BeginExclusiveEntityTransaction();

            SerializeUtilityHybrid.Deserialize(entityManager, binaryReader, objectTable);

            //entityManager.EndExclusiveEntityTransaction();
        }
    }
}