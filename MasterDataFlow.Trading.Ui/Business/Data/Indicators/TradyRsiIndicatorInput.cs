using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Data.Indicators;
using Trady.Analysis;
using Trady.Analysis.Extension;
using Trady.Core.Infrastructure;

namespace MasterDataFlow.Trading.Ui.Business.Data.Indicators
{
    public class TradyRsiIndicatorInput : RsiIndicatorInput
    {
        private readonly int _period;

        public TradyRsiIndicatorInput(int period) : base(period)
        {
            _period = period;
        }

        public override InputValues GetValues(Bar[] bars)
        {
            var result = TradyHelper.GetValues(Name, bars, (candles => candles.Rsi(_period)));
            return result;
        }

        public override float GetMax()
        {
            return 100;
        }

        public override float GetMin()
        {
            return 0;
        }
    }



}
