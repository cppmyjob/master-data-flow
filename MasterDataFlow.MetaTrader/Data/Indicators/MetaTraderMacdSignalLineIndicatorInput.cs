using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Data.Indicators;

namespace MasterDataFlow.MetaTrader.Data.Indicators
{
    public class MetaTraderMacdSignalLineIndicatorInput : MacdSignalLineIndicatorInput
    {
        public override InputValues GetValues(Bar[] bars)
        {
            throw new NotImplementedException();
        }
    }
}
