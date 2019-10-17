using System.Collections.Generic;
using Trady.Analysis;
using Trady.Analysis.Extension;
using Trady.Core.Infrastructure;

namespace MasterDataFlow.Trading.Shared.Data.Indicators
{
    public class TradySarIndicatorInput : TradyInput
    {
        public TradySarIndicatorInput() : base("SAR")
        {
        }

        protected override IReadOnlyList<AnalyzableTick<decimal?>> GetIndicatorData(IOhlcv[] candles)
        {
            var data = candles.Sar(0.02M, 0.2M);
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
