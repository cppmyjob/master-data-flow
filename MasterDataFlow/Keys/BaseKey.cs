using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MasterDataFlow.Keys
{
    public abstract class BaseKey : IComparable<BaseKey>
    {
        public class BaseKeyDefaultContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                //TODO: Maybe cache
                var prop = base.CreateProperty(member, memberSerialization);

                if (!prop.Writable)
                {
                    var property = member as PropertyInfo;
                    if (property != null)
                    {
                        var hasPrivateSetter = property.GetSetMethod(true) != null;
                        prop.Writable = hasPrivateSetter;
                    }
                }
                return prop;
            }
        }

        private string _key = null;

        [JsonIgnore]
        public string Key
        {
            get { return _key ?? (_key = JsonConvert.SerializeObject(this)); }
        }

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
                            {
                                ContractResolver = new BaseKeyDefaultContractResolver()
                            };

        public static T DeserializeKey<T>(string value) where T : BaseKey
        {
            var result = JsonConvert.DeserializeObject<T>(value, JsonSerializerSettings);
            return result;
        }

        public static bool operator ==(BaseKey a, BaseKey b)
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

        public static bool operator !=(BaseKey a, BaseKey b)
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
            var p = obj as BaseKey;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Key == p.Key);
        }

        public bool Equals(BaseKey p)
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

        public int CompareTo(BaseKey other)
        {
            return System.String.Compare(Key, other.Key, System.StringComparison.Ordinal);
        }

        public override string ToString()
        {
            return Key;
        }
    }

}
