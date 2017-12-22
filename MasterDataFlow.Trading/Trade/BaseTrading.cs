using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Tester;

namespace MasterDataFlow.Trading.Trade
{
    public enum TickStatus
    {
        Ok,
        Repeat,
        Error,
    }

    public abstract class BaseTrading
    {
        protected TickStatus Tick()
        {
            var direction = GetDirection();
            switch (direction)
            {
                case Direction.Down:
                    return ProcessDown();
                case Direction.Up:
                    return ProcessUp();
                case Direction.None:
                    break;
            }
            return TickStatus.Ok;
        }

        private TickStatus ProcessUp()
        {
            if (IsSellOrderExists())
            {
                var closeOrderStatus = CloseSellOrder();
                if (closeOrderStatus != TickStatus.Ok)
                    return closeOrderStatus;
            }
            if (IsBuyOrderExists())
                return TickStatus.Ok;

            return BuyOrder();
        }

        private TickStatus ProcessDown()
        {
            if (IsBuyOrderExists())
            {
                var closeOrderStatus = CloseBuyOrder();
                if (closeOrderStatus != TickStatus.Ok)
                    return closeOrderStatus;
            }
            if (IsSellOrderExists())
                return TickStatus.Ok;

            return SellOrder();
        }

        protected abstract Direction GetDirection();

        protected abstract TickStatus SellOrder();

        protected abstract TickStatus CloseBuyOrder();

        protected abstract bool IsBuyOrderExists();

        protected abstract TickStatus BuyOrder();

        protected abstract TickStatus CloseSellOrder();

        protected abstract bool IsSellOrderExists();
    }
}
