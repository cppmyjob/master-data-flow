using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Advisors;

namespace MasterDataFlow.Trading.Interfaces
{
    public interface ITrader
    {
        bool IsSellOrderExists();
        TickStatus CloseSellOrder();
        bool IsBuyOrderExists();
        TickStatus BuyOrder();
        TickStatus CloseBuyOrder();
        TickStatus SellOrder();
    }
}
