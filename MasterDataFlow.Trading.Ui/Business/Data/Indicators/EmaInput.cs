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
    }


}
