using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct BlueprintId : IComponentData, IValueComponent<int>, IEquatable<BlueprintId>
{
    public int Value;

    int IValueComponent<int>.Value { get => Value; set => Value = value; }

    public static BlueprintId Null => new BlueprintId();

    public override bool Equals(object obj) => Equals((BlueprintId)obj);
    public override int GetHashCode() => Value.GetHashCode();
    public bool Equals(BlueprintId other) => other.Value == Value;
    public static bool operator ==(BlueprintId a, BlueprintId b) => a.Equals(b);
    public static bool operator !=(BlueprintId a, BlueprintId b) => !(a.Value == b.Value);
}
