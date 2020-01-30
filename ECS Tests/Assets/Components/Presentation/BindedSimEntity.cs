using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

public struct BindedSimEntity : IComponentData
{
    public Entity SimWorldEntity;
}
