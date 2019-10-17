using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Data.Indicators;
using Trady.Analysis.Extension;

namespace MasterDataFlow.Trading.Shared.Data.Indicators
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
