using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Data;
using Trady.Analysis;
using Trady.Core.Infrastructure;

namespace MasterDataFlow.Trading.Ui.Business.Data
{
    public static class TradyHelper
    {
        public static InputValues GetValues(string name, Bar[] bars,
            Func<IOhlcv[], IReadOnlyList<AnalyzableTick<decimal?>>> getIndicatorData)
        {
            var candles = Helper.BarsToCandles(bars);
            var data = getIndicatorData(candles);
            var values = GetInputValues(data);
            var result = new InputValues(name, values);
            return result;
        }

        private static InputValue[] GetInputValues(IReadOnlyList<AnalyzableTick<decimal?>> data)
        {
            var result = data.Select(t => new InputValue(t.DateTime.HasValue ? t.DateTime.Value.DateTime : new DateTime(),
                                         t.Tick.HasValue ? (float)t.Tick.Value : 0F)).ToArray();
            return result;
        }
    }
}
