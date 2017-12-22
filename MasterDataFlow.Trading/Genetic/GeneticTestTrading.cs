using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Tester;
using MasterDataFlow.Trading.Trade;

namespace MasterDataFlow.Trading.Genetic
{
    public class GeneticTestTrading : BaseTrading
    {
        private readonly Bar[] _prices;

        public GeneticTestTrading(Bar[] prices)
        {
            _prices = prices;
        }

        public 


        protected override Direction GetDirection()
        {
            throw new NotImplementedException();
        }

        protected override TickStatus SellOrder()
        {
            throw new NotImplementedException();
        }

        protected override TickStatus CloseBuyOrder()
        {
            throw new NotImplementedException();
        }

        protected override bool IsBuyOrderExists()
        {
            throw new NotImplementedException();
        }

        protected override TickStatus BuyOrder()
        {
            throw new NotImplementedException();
        }

        protected override TickStatus CloseSellOrder()
        {
            throw new NotImplementedException();
        }

        protected override bool IsSellOrderExists()
        {
            throw new NotImplementedException();
        }
    }
}
