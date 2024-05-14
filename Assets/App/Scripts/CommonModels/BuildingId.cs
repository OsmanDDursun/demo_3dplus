using System;
using System.Collections.Generic;

namespace App.Scripts.CommonModels
{
    public readonly struct BuildingId : IEquatable<BuildingId>, IEqualityComparer<BuildingId>
    {
        public ulong Value { get; }
        public bool IsValid => Value != 0;
        
        private static List<ulong> _ids = new List<ulong>();
        
        public BuildingId(ulong value)
        {
            Value = value;
        }
        
        public static BuildingId Invalid = new BuildingId(0);
        
        public static BuildingId Generate()
        {
            ulong id = 0;
            while (id == 0 || _ids.Contains(id))
            {
                id = (ulong)UnityEngine.Random.Range(1, int.MaxValue);
            }
            _ids.Add(id);
            return new BuildingId(id);
        }
        
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