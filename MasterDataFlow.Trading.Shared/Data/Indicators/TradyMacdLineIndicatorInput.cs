using System.Linq;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Data.Indicators;
using Trady.Analysis.Extension;

namespace MasterDataFlow.Trading.Shared.Data.Indicators
{
    public class TradyMacdLineIndicatorInput : MacdLineIndicatorInput
    {
        public override InputValues GetValues(Bar[] bars)
        {
            var candles = Helper.BarsToCandles(bars);
            var macds = candles.Macd(12, 26, 9);
            var values = macds.Select(t => new InputValue(t.DateTime.Value.DateTime, (float) t.Tick.MacdLine)).ToArray();
            var result = new InputValues(Name, values);
            return result;
        }
    }
}
