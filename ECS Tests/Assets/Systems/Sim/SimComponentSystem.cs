using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

public abstract class SimComponentSystem : ComponentSystem
{
}

public abstract class SimJobComponentSystem : JobComponentSystem
{
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class CreateInSimWorldAttribute : Attribute
{

}