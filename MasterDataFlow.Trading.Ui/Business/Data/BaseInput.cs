using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Data;

namespace MasterDataFlow.Trading.Ui.Business.Data
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
        public class IndicatorSearch
        {
            private string _s;

            public IndicatorSearch(string s)
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

        public void Normalize(InputValues values)
        {
            var min = values.Values.Select(t => t.Value).Min();
            var max = values.Values.Select(t => t.Value).Max();
            var diff = max - min;
            var offset = diff * 20 / 100;
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
