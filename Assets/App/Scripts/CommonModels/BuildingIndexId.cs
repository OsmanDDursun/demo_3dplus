using System;

namespace App.Scripts.CommonModels
{
    public readonly struct BuildingIndexId : IEquatable<BuildingIndexId>
    {
        public int Value { get; }
        
        private static int _lastIndex = -1;
        
        public BuildingIndexId(int value)
        {
            Value = value;
        }
        
        public static BuildingIndexId Invalid = new BuildingIndexId(-1);
        
        public static BuildingIndexId Next()
        {
            _lastIndex++;
            return new BuildingIndexId(_lastIndex);
        }
        
        public static bool operator ==(BuildingIndexId a, BuildingIndexId b) => a.Value == b.Value;
        public static bool operator !=(BuildingIndexId a, BuildingIndexId b) => a.Value != b.Value;

        public bool Equals(BuildingIndexId other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is BuildingIndexId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
}