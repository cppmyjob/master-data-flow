using System;
using System.Collections.Generic;
using System.Linq;
using MasterDataFlow.Trading.Common;

namespace MasterDataFlow.Trading.Tester
{
    public enum FxStoryType
    {
        Sell,
        Buy,
        Close,
        CloseByStopLoss,
        CloseByTakeProfit,
        CloseByForceStop,
        CloseByStop
    }

    public class FxStory
    {
        public int Id { get; internal set; }
        public DateTime Time { get; internal set; }
        public FxStoryType Type { get; internal set; }
        public int OrderTicket { get; internal set; }
        public decimal Volume { get; internal set; }
        public decimal Price { get; internal set; }
        public decimal? StopLoss { get; internal set; }
        public decimal? TakeProfit { get; internal set; }
        public decimal? Profit { get; internal set; }
        public decimal Balance { get; internal set; }
    }

    public class FxTesterResult
    {
        private readonly List<FxStory> _stories = new List<FxStory>();

        public bool ForceStop { get; internal set; }

        public int PlusEquityCount { get; internal set; }

        public double MinEquity { get; internal set; } = double.MaxValue;

        public int MinusEquityCount { get; internal set; }

        public double MaxEquity { get; internal set; } = double.MinValue;

        public double[] Equities { get; internal set; }

        public double Profit { get; internal set; }

        public int OrderCount { get; internal set; }

        public int PlusCount { get; internal set; }

        public int MinusCount { get; internal set; }

        public int SellCount { get; internal set; }

        public int BuyCount { get; internal set; }

        internal void AddStory(FxStory story)
        {
            story.Id = _stories.Count + 1;
            _stories.Add(story);
        }

        public IEnumerable<FxStory> Stories => _stories;
    }

    public enum FxDirection
    {
        None = 0,
        Down = 1,
        Up = 2
    }

    public enum FxOrderType
    {
        Sell,
        Buy
    }

    public class FxOrder
    {
        public FxOrder(FxOrderType type, double price, double stopLoss)
        {
            Price = price;
            Type = type;
            StopLoss = stopLoss;
        }


        public double StopLoss { get; }

        public int Ticket { get; internal set; }

        public int OpenBarIndex { get; internal set; }

        public int CloseBarIndex { get; internal set; }

        public FxOrderType Type { get; }

        public double Price { get; }

        public decimal Volume { get; } = 0.1m;
    }

    public class FxHistoryItem
    {
        public FxHistoryItem(FxOrder order, double profit)
        {
            Order = order;
            Profit = profit;
        }

        public FxOrder Order { get; }

        public double Profit { get; }
    }

    public delegate FxDirection FxDirectionGetDirection(int index);

    public abstract class FxDirectionTester : FxAbstractTester
    {
        private FxOrder _currentOrder;
        private readonly FxDirectionGetDirection _getDirection;

        public FxDirectionTester(double deposit, Bar[] prices, int from, int length) :
            base(deposit, prices, from, length)
        {
            _getDirection = GetDirectionDelegate();
        }

        protected abstract FxDirectionGetDirection GetDirectionDelegate();
        protected abstract int GetStopLoss();

        private int _lastBarNumber = -1;

        public override void Reset()
        {
            base.Reset();
            _currentOrder = null;
            _lastBarNumber = -1;
        }

        protected override void OnTick()
        {
            if (CurrentBar == _lastBarNumber)
                return;
            _lastBarNumber = CurrentBar;
            if (_currentOrder != null && !Orders.ContainsKey(_currentOrder.Ticket))
                _currentOrder = null;

            if (_currentOrder == null)
                switch (_getDirection(CurrentBar))
                {
                    case FxDirection.Up:
                        _currentOrder = Buy();
                        break;
                    case FxDirection.Down:
                        _currentOrder = Sell();
                        break;
                    case FxDirection.None:
                        break;
                }
            else
                switch (_getDirection(CurrentBar))
                {
                    case FxDirection.Up:
                        if (_currentOrder.Type == FxOrderType.Sell)
                        {
                            CloseOrder(_currentOrder.Ticket);
                            _currentOrder = Buy();
                        }
                        break;
                    case FxDirection.Down:
                        if (_currentOrder.Type == FxOrderType.Buy)
                        {
                            CloseOrder(_currentOrder.Ticket);
                            _currentOrder = Sell();
                        }
                        break;
                    case FxDirection.None:
                        CloseOrder(_currentOrder.Ticket);
                        _currentOrder = null;
                        break;
                }
        }

        private FxOrder Buy()
        {
            var intSellOrder = GetStopLoss();
            var stopLoss = 0.0;
            if (intSellOrder != 0)
                stopLoss = CurrentPrice - (double) intSellOrder / DecimalFactor;
            var order = new FxOrder(FxOrderType.Buy, CurrentPrice, stopLoss);
            Buy(order);
            return order;
        }

        private FxOrder Sell()
        {
            var intSellOrder = GetStopLoss();
            var stopLoss = 0.0;
            if (intSellOrder != 0)
                stopLoss = CurrentPrice + (double) intSellOrder / DecimalFactor;
            var order = new FxOrder(FxOrderType.Sell, CurrentPrice, stopLoss);
            Sell(order);
            return order;
        }
    }

    public abstract class FxAbstractTester
    {
        private readonly Bar[] _prices;
        private readonly int _length;

        private int _lastTicket;

        private int _decimals = 4;
        protected int _decimalsFactor = 10000;

        protected FxTesterResult _result;

        private int _tickNumber = -1;
        private double[] _ticks;
        private DateTime[] _ticksTimes;

        protected FxAbstractTester(double deposit, Bar[] prices)
            : this(deposit, prices, 0, prices.Length)
        {
        }

        public List<FxHistoryItem> History { get; private set; }

        public int Decimals
        {
            get => _decimals;
            set
            {
                _decimals = value;
                _decimalsFactor = (int) Math.Pow(10, _decimals);
                Point = 1 / _decimalsFactor;
            }
        }

        protected int DecimalFactor => _decimalsFactor;

        protected decimal Point { get; private set; } = 0.0001m;

        public int Spred { get; set; } = 3;

        protected Dictionary<int, FxOrder> Orders { get; private set; }

        public double Deposit { get; set; }

        public FxAbstractTester(double deposit, Bar[] prices, int from, int length)
        {
            Deposit = deposit;
            _prices = prices;
            From = from;
            _length = length;
            if (From + _length > _prices.Length)
                throw new Exception("Выход за диапазон");
            Reset();
        }

        public int From { get; }

        public virtual void Reset()
        {
            Orders = new Dictionary<int, FxOrder>();
            History = new List<FxHistoryItem>();
            _lastTicket = 0;
            CurrentBar = 0;
            _result = new FxTesterResult();
            _result.Equities = new double[_prices.Length];
        }

        protected abstract void OnTick();

        public virtual FxTesterResult Run()
        {
            for (var i = From; i < From + _length; i++)
            {
                if (_result.ForceStop)
                    break;
                CurrentBar = i;
                MakeTicks();
            }
            while (Orders.Keys.Count > 0)
            {
                // TODO неоптимально
                var closeType = _result.ForceStop ? FxStoryType.CloseByForceStop : FxStoryType.CloseByStop;
                CloseOrder(Orders.Keys.ToArray()[0], CurrentPrice, closeType);
            }
            return _result;
        }

        private void MakeTicks()
        {
            _ticks = new double[4];
            _ticks[0] = _prices[CurrentBar].Open;
            _ticks[1] = _prices[CurrentBar].Low;
            _ticks[2] = _prices[CurrentBar].High;
            _ticks[3] = _prices[CurrentBar].Close;

            _ticksTimes = new DateTime[4];
            _ticksTimes[0] = _prices[CurrentBar].Time;
            _ticksTimes[1] = _prices[CurrentBar].Time;
            _ticksTimes[2] = _prices[CurrentBar].Time;
            _ticksTimes[3] = _prices[CurrentBar].Time;

            for (var i = 0; i < _ticks.Length; i++)
            {
                _tickNumber = i;
                // TODO Непонятно когда правильно вычислять  Equity
                MakeStopLoss();
                CalculateEquity(CurrentPrice);
                if (Deposit + (_result.Profit + _result.Equities[CurrentBar]) * DecimalFactor < 0)
                {
                    _result.ForceStop = true;
                    break;
                }
                OnTick();
            }
        }

        private void MakeStopLoss()
        {
            var localOrders = new List<FxOrder>(Orders.Values);
            foreach (var order in localOrders)
            {
                if (order.StopLoss == 0)
                    continue;
                switch (order.Type)
                {
                    case FxOrderType.Buy:
                        if (CurrentPrice <= order.StopLoss)
                        {
                            CalculateEquity(order.StopLoss);
                            CloseOrder(order.Ticket, order.StopLoss, FxStoryType.CloseByStopLoss);
                        }
                        break;
                    case FxOrderType.Sell:
                        if (CurrentPrice >= order.StopLoss)
                        {
                            CalculateEquity(order.StopLoss);
                            CloseOrder(order.Ticket, order.StopLoss, FxStoryType.CloseByStopLoss);
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        protected double GetCurrentEquity()
        {
            return GetCurrentEquity(CurrentPrice);
        }

        private double GetCurrentEquity(double price)
        {
            var profit = 0.0;
            foreach (var pair in Orders)
            {
                var order = pair.Value;
                switch (order.Type)
                {
                    case FxOrderType.Buy:
                        profit += price - order.Price - Spred / (double) DecimalFactor;
                        break;
                    case FxOrderType.Sell:
                        profit += order.Price - price - Spred / (double) DecimalFactor;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return profit;
        }

        private void CalculateEquity(double price)
        {
            var profit = GetCurrentEquity(price);
            _result.Equities[CurrentBar] = profit;
            if (profit == 0.0)
                return;
            if (profit >= 0.0)
            {
                if (profit > _result.MaxEquity)
                    _result.MaxEquity = profit;
                ++_result.PlusEquityCount;
            }
            else
            {
                if (profit < _result.MinEquity)
                    _result.MinEquity = profit;
                ++_result.MinusEquityCount;
            }
        }


        protected double CurrentPrice => _ticks[_tickNumber];

        protected DateTime CurrentTime => _ticksTimes[_tickNumber];

        protected int CurrentBar { get; private set; }

        protected void Buy(FxOrder order)
        {
            ++_lastTicket;

            var story = new FxStory
                        {
                            Balance = (decimal) (Deposit + _result.Profit * DecimalFactor),
                            OrderTicket = _lastTicket,
                            Price = (decimal) CurrentPrice,
                            Profit = null,
                            StopLoss = order.StopLoss > 0 ? (decimal?) order.StopLoss : null,
                            TakeProfit = null, // TODO
                            Time = CurrentTime,
                            Type = FxStoryType.Buy,
                            Volume = order.Volume
                        };
            _result.AddStory(story);

            order.Ticket = _lastTicket;
            order.OpenBarIndex = CurrentBar;
            Orders.Add(_lastTicket, order);
        }

        protected void Sell(FxOrder order)
        {
            ++_lastTicket;

            var story = new FxStory
                        {
                            Balance = (decimal) (Deposit + _result.Profit * DecimalFactor),
                            OrderTicket = _lastTicket,
                            Price = (decimal) CurrentPrice,
                            Profit = null,
                            StopLoss = order.StopLoss > 0 ? (decimal?) order.StopLoss : null,
                            TakeProfit = null, // TODO
                            Time = CurrentTime,
                            Type = FxStoryType.Sell,
                            Volume = order.Volume
                        };

            _result.AddStory(story);
            order.Ticket = _lastTicket;
            order.OpenBarIndex = CurrentBar;
            Orders.Add(_lastTicket, order);
        }

        protected void CloseOrder(int ticket)
        {
            CloseOrder(ticket, CurrentPrice, FxStoryType.Close);
        }

        private void CloseOrder(int ticket, double price, FxStoryType closeType)
        {
            var order = Orders[ticket];
            var profit = 0.0;
            switch (order.Type)
            {
                case FxOrderType.Buy:
                    profit = CloseBuyOrder(order, price);
                    break;
                case FxOrderType.Sell:
                    profit = CloseSellOrder(order, price);
                    break;
                default:
                    throw new NotImplementedException();
            }
            Orders.Remove(ticket);
            order.CloseBarIndex = CurrentBar;
            History.Add(new FxHistoryItem(order, profit));

            var story = new FxStory
                        {
                            Balance = (decimal) (Deposit + _result.Profit * DecimalFactor),
                            OrderTicket = ticket,
                            Price = (decimal) CurrentPrice,
                            Profit = (decimal) profit * DecimalFactor,
                            StopLoss = order.StopLoss > 0 ? (decimal?) order.StopLoss : null,
                            TakeProfit = null, // TODO
                            Time = CurrentTime,
                            Type = closeType,
                            Volume = order.Volume
                        };
            _result.AddStory(story);
        }

        private double CloseSellOrder(FxOrder order, double price)
        {
            var profit = order.Price - price - Spred / (double) DecimalFactor;
            if (profit >= 0)
                _result.PlusCount += 1;
            else
                _result.MinusCount += 1;
            _result.Profit += profit;
            ++_result.OrderCount;
            ++_result.SellCount;
            return profit;
        }

        private double CloseBuyOrder(FxOrder order, double price)
        {
            var profit = price - order.Price - Spred / (double) DecimalFactor;
            if (profit >= 0)
                _result.PlusCount += 1;
            else
                _result.MinusCount += 1;
            _result.Profit += profit;
            ++_result.OrderCount;
            ++_result.BuyCount;
            return profit;
        }
    }
}