using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Interfaces;
using MasterDataFlow.Trading.Tester;

namespace MasterDataFlow.Trading.Advisors
{
    public enum TickStatus
    {
        Ok,
        Repeat,
        Error,
    }

    public enum AdvisorAction
    {
        Nothing,
        Hold = 0,
        Close = 1,
        Down = 2,
        Up = 3
    }


    public abstract class BaseAdvisor
    {
        private readonly ITrader _trader;

        protected BaseAdvisor(ITrader trader)
        {
            _trader = trader;
        }

        public TickStatus Tick(DateTime time, decimal price)
        {
            var action = GetAction(time, price);
            switch (action)
            {
                case AdvisorAction.Down:
                    return ProcessDown();
                case AdvisorAction.Up:
                    return ProcessUp();
                case AdvisorAction.Hold:
                    break;
                case AdvisorAction.Close:
                    break;
            }
            return TickStatus.Ok;
        }

        private TickStatus ProcessUp()
        {
            if (_trader.IsSellOrderExists())
            {
                var closeOrderStatus = _trader.CloseSellOrder();
                if (closeOrderStatus != TickStatus.Ok)
                    return closeOrderStatus;
            }
            if (_trader.IsBuyOrderExists())
                return TickStatus.Ok;

            return _trader.BuyOrder();
        }

        private TickStatus ProcessDown()
        {
            if (_trader.IsBuyOrderExists())
            {
                var closeOrderStatus = _trader.CloseBuyOrder();
                if (closeOrderStatus != TickStatus.Ok)
                    return closeOrderStatus;
            }
            if (_trader.IsSellOrderExists())
                return TickStatus.Ok;

            return _trader.SellOrder();
        }

        protected abstract AdvisorAction GetAction(DateTime time, decimal price);

    }

}
