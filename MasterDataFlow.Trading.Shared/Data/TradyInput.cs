using System;
using System.Collections.Generic;
using System.Linq;
using MasterDataFlow.Trading.Data;
using Trady.Analysis;
using Trady.Core.Infrastructure;

namespace MasterDataFlow.Trading.Shared.Data
{
    public abstract class TradyInput : BaseInput
    {
        protected TradyInput(string name) : base(name)
        {
        }

        public override InputValues GetValues(Bar[] bars)
        {
            var candles = Helper.BarsToCandles(bars);
            var data = GetIndicatorData(candles);
            var values = GetInputValues(data);
            var result = new InputValues(Name, values);
            return result;
        }

        protected abstract IReadOnlyList<AnalyzableTick<decimal?>> GetIndicatorData(IOhlcv[] candles);

        protected static InputValue[] GetInputValues(IReadOnlyList<AnalyzableTick<decimal?>> data)
        {
            var result = data.Select(t => new InputValue(t.DateTime.HasValue ? t.DateTime.Value.DateTime : new DateTime(),
                                         t.Tick.HasValue ? (float)t.Tick.Value : 0F)).ToArray();
            return result;
        }

    }
}
