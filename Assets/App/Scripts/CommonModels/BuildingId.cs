using System;
using System.Collections.Generic;

namespace App.Scripts.CommonModels
{
    public readonly struct BuildingId : IEquatable<BuildingId>, IEqualityComparer<BuildingId>
    {
        public ulong Value { get; }
        public bool IsValid => Value != 0;
        
        public BuildingId(ulong value)
        {
            Value = value;
        }
        
        public static BuildingId Invalid = new BuildingId(0);
        
        public static bool operator ==(BuildingId a, BuildingId b) => a.Value == b.Value;
        public static bool operator !=(BuildingId a, BuildingId b) => a.Value != b.Value;

        public bool Equals(BuildingId other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is BuildingId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public bool Equals(BuildingId x, BuildingId y)
        {
            return x.Value == y.Value;
        }

        public int GetHashCode(BuildingId obj)
        {
            return obj.Value.GetHashCode();
        }
    }
}