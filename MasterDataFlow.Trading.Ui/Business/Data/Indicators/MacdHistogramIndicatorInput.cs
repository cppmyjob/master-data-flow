using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Data;
using Trady.Analysis.Extension;

namespace MasterDataFlow.Trading.Ui.Business.Data.Indicators
{
    public class MacdHistogramIndicatorInput : BaseInput
    {
        public MacdHistogramIndicatorInput() : base("MACD Histogram")
        {
        }

        public override InputValues GetValues(Bar[] bars)
        {
            var candles = Helper.BarsToCandles(bars);
            var macds = candles.Macd(12, 26, 9);
            var values = macds.Select(t => new InputValue(t.DateTime.Value.DateTime, (float) t.Tick.MacdHistogram)).ToArray();
            var result = new InputValues(Name, values);
            return result;
        }

        public override float GetMax()
        {
            return 7;
        }

        public override float GetMin()
        {
            return -7;
        }
    }
}
