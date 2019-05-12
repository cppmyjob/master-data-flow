using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDataFlow.Trading.Data.Indicators
{
    public abstract class RsiIndicatorInput : BaseInput
    {
        public RsiIndicatorInput(int period) : base("RSI " + period)
        {
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
