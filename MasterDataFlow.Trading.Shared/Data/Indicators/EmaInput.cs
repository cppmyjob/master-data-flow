using System.Collections.Generic;
using Trady.Analysis;
using Trady.Analysis.Extension;
using Trady.Core.Infrastructure;

namespace MasterDataFlow.Trading.Shared.Data.Indicators
{
    public class EmaInput : TradyInput
    {
        private readonly int _period;

        public EmaInput(int period) : base("EMA " + period)
        {
            _period = period;
        }

        protected override IReadOnlyList<AnalyzableTick<decimal?>> GetIndicatorData(IOhlcv[] candles)
        {
            var data = candles.Ema(_period);
            return data;
        }

        public override float GetMax()
        {
            return 500;
        }

        public override float GetMin()
        {
            return 0;
        }
    }


}
