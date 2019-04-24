using System;
using System.Linq;

namespace MasterDataFlow.Trading.Data
{
    public class InputValues
    {
        public InputValues(string name, InputValue[] values)
        {
            Name = name;
            Values = values;
        }

        public string Name { get; private set; }
        public InputValue[] Values { get; private set; }
    }

    public class InputValue
    {
        public InputValue()
        {
            
        }

        public InputValue(DateTime time, float value)
        {
            Time = time;
            Value = value;
        }

        public float Value { get; set; }
        public DateTime Time { get; set; }
    }

    public abstract class BaseInput : IComparable
    {
        public class Search
        {
            private string _s;

            public Search(string s)
            {
                _s = s;
            }

            public bool Match(BaseInput e)
            {
                return e.Name == _s;
            }
        }

        protected BaseInput(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public abstract InputValues GetValues(Bar[] bars);
        public abstract float GetMax();
        public abstract float GetMin();

        public void Normalize(InputValues values)
        {
            var min = GetMin();
            var max = GetMax();
            var diff = max - min;
            var offset = diff * 10 / 100;
            min = min - offset;
            max = max + offset;

            diff = max - min;

            foreach (var t in values.Values)
            {
                t.Value = (t.Value - min) / diff;
            }
        }

        public int CompareTo(object o)
        {
            if (!(o is BaseInput indicator))
                throw new ArgumentException("o is not an BaseInput object.");

            return String.Compare(Name, indicator.Name, StringComparison.Ordinal);
        }
    }
}
