using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Data;
using Trady.Analysis.Extension;

namespace MasterDataFlow.Trading.Ui.Business.Data.Indicators
{
    public class MacdSignalLineIndicatorInput : BaseInput
    {
        public MacdSignalLineIndicatorInput() : base("MACD SignalLine")
        {
        }

        public override InputValue[] GetValues(Bar[] bars)
        {
            var candles = Helper.BarsToCandles(bars);
            var macds = candles.Macd(12, 26, 9);
            var result = macds.Select(t => new InputValue(t.DateTime.Value.DateTime, (float) t.Tick.SignalLine)).ToArray();
            return result;
        }
    }
}
