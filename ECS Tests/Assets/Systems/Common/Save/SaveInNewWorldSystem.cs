using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine;

[DisableAutoCreation]
//[UpdateInGroup(typeof(SystemGroup))]
public class SaveInNewWorldSystem : ComponentSystem
{
    protected override void OnCreate()
    {
        base.OnCreate();

        string filePath = Application.persistentDataPath + "/test.txt";

        if (File.Exists(filePath))
            File.Delete(filePath);

        using (StreamWriter writer = new StreamWriter(filePath, append: false, System.Text.Encoding.UTF8) { NewLine = "\n" })
        {
            writer.Flush();
            SerializeUtility.SerializeWorldIntoYAML(EntityManager, writer, dumpChunkRawData: false);
        }

        Debug.Log("Save to " + filePath);
    }

    protected override void OnUpdate()
    {
    }
}
