using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDataFlow.Trading.Data.Indicators
{
    public abstract class MacdLineIndicatorInput : BaseInput
    {
        public MacdLineIndicatorInput() : base("MACD Line")
        {
        }

        public override float GetMax()
        {
            return 7;
        }

        public override float GetMin()
        {
            return -7;
        }
    }
}
