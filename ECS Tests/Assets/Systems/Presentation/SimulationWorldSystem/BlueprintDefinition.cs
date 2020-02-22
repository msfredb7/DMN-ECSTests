using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintDefinition : MonoBehaviour
{
    public enum ESplitMode
    {
        FirstChildIsView,
        SecondChildIsView
    }

    public int BlueprintIdValue;
    public ESplitMode SplitMode = ESplitMode.SecondChildIsView;

    public BlueprintId GetBlueprintId() => new BlueprintId() { Value = BlueprintIdValue };

    public GameObject GetGameObject(GameWorldType gameWorldType)
    {
        switch (gameWorldType)
        {
            case GameWorldType.Simulation:
                return GetSimGameObject();
            case GameWorldType.Presentation:
                return GetViewGameObject();
            default:
                return null;
        }
    }
    public GameObject GetSimGameObject()
    {
        return SplitMode == ESplitMode.FirstChildIsView ? GetSecondChild() : GetFirstChild();
    }
    public GameObject GetViewGameObject()
    {
        return SplitMode == ESplitMode.FirstChildIsView ? GetFirstChild() : GetSecondChild();
    }

    GameObject GetFirstChild()
    {
        if (transform.childCount > 0)
            return transform.GetChild(0).gameObject;
        else
            return null;
    }
    GameObject GetSecondChild()
    {
        if (transform.childCount > 1)
            return transform.GetChild(1).gameObject;
        else
            return null;
    }
}
