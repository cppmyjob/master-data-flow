using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Data;
using Trady.Analysis;
using Trady.Analysis.Extension;
using Trady.Core.Infrastructure;

namespace MasterDataFlow.Trading.Ui.Business.Data.Indicators
{
    public class SarIndicatorInput : TradyInput
    {
        public SarIndicatorInput() : base("SAR")
        {
        }

        protected override IReadOnlyList<AnalyzableTick<decimal?>> GetIndicatorData(IOhlcv[] candles)
        {
            var data = candles.Sar(0.02M, 0.2M);
            return data;
        }
    }



}
