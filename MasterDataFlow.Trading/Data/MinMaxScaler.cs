using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDataFlow.Trading.Data
{
    public class MinMaxScaler
    {
        private readonly float _min;
        private readonly float _max;

        public MinMaxScaler(float min, float max)
        {
            _min = min;
            _max = max;
        }

        public void Transform(InputValues values)
        {
            var min = _min;
            var max = _max;
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

    }
}
