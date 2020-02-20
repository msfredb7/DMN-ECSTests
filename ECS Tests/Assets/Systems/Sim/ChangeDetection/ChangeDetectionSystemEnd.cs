using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;

public class ChangeDetectionSystemEnd : ComponentSystem
{
    public enum LogMode
    {
        NoLog,
        Info,
        Warning,
        Error
    }

    public LogMode LoggingMode = LogMode.Error;
    Dictionary<ComponentType, uint> _summedVersionNumbers = new Dictionary<ComponentType, uint>();

    protected override void OnUpdate()
    {
        var beginSystem = World.GetExistingSystem<ChangeDetectionSystemBegin>();
        if(beginSystem == null)
        {
            UnityEngine.Debug.LogWarning($"{nameof(ChangeDetectionSystemEnd)} cannot detect changes without the existance of {nameof(ChangeDetectionSystemBegin)}");
            return;
        }

        if (!beginSystem.HasUpdatedAtLeastOnce)
            return;
        
        ChangeDetectionSystemUtility.GatherAllVersionNumbers(EntityManager, _summedVersionNumbers);

        // compare 'begin values' with 'end values'
        CompareAndLogChanges(beginSystem.SummedVersionNumbers, _summedVersionNumbers);
    }

    void CompareAndLogChanges(Dictionary<ComponentType, uint> a, Dictionary<ComponentType, uint> b)
    {
        if(a.Count < b.Count)
        {
            LogChange($"new archetypes were created");
        }
        else if (a.Count > b.Count)
        {
            LogChange($"archetypes were destroyed");
        }

        foreach (ComponentType componentType in a.Keys)
        {
            if (b.TryGetValue(componentType, out uint bValue))
            {
                if (a[componentType] != bValue)
                    LogChange($"an entity's {componentType.GetManagedType()} was modified");
            }
            else
            {
                LogChange($"an archetype with {componentType.GetManagedType()} was destroyed");
            }
        }
    }

    void LogChange(string s)
    {
        s = $"Change detected: {s}";

        switch (LoggingMode)
        {
            default:
            case LogMode.NoLog:
                break;
            case LogMode.Info:
                UnityEngine.Debug.Log(s);
                break;
            case LogMode.Warning:
                UnityEngine.Debug.LogWarning(s);
                break;
            case LogMode.Error:
                UnityEngine.Debug.LogError(s);
                break;
        }
    }
}
