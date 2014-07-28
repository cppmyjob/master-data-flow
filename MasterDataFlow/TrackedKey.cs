﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace MasterDataFlow
{
    public abstract class TrackedKey : IComparable<TrackedKey>
    {
        private string _key = null;

        [JsonIgnore]
        public string Key
        {
            get
            {
                if (_key == null)
                {
                    _key = JsonConvert.SerializeObject(this);
                }
                return _key;
            }
        }

        public static bool operator ==(TrackedKey a, TrackedKey b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.Key == b.Key;
        }

        public static bool operator !=(TrackedKey a, TrackedKey b)
        {
            return !(a == b);
        }


        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            var p = obj as TrackedKey;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Key == p.Key);
        }

        public bool Equals(TrackedKey p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Key == p.Key);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public int CompareTo(TrackedKey other)
        {
            return System.String.Compare(Key, other.Key, System.StringComparison.Ordinal);
        }

        public override string ToString()
        {
            return Key;
        }
    }

}
