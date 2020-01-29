using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public SimConvertToEntity One;
    public ViewConvertToEntity Two;
    public int Duplicates;

    private void Awake()
    {
        for (int i = 0; i < Duplicates; i++)
        {
            Instantiate(Two.gameObject)
                .GetComponent<ViewConvertToEntity>().ObservedSimEntity = RemoveRandomSimComponent(Instantiate(One.gameObject)).GetComponent<SimConvertToEntity>();
        }
    }

    List<Component> components = new List<Component>();
    GameObject RemoveRandomSimComponent(GameObject gameObject)
    {
        gameObject.GetComponents(components);

        for (int i = 0; i < components.Count; i++)
        {
            if (components[i] is SimConvertToEntity)
                continue;
            if (components[i] is Transform tr)
            {
                //tr.position = new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f) * 40;
                continue;
            }

            if (Random.value < 0.4f)
                DestroyImmediate(components[i]);
        }

        return gameObject;
    }
}
