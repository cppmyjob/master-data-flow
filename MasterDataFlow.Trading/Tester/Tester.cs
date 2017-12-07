using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Data;

namespace MasterDataFlow.Trading.Tester
{
    public enum FxStoryType { Sell, Buy, Close, CloseByStopLoss, CloseByTakeProfit, CloseByForceStop, CloseByStop }

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
        private double _profit = 0.0;
        private int _orderCount = 0;
        private int _plusCount = 0;
        private int _minusCount = 0;
        private double[] _equities = null;
        private double _minEquity = Double.MaxValue;
        private int _minusEquityCount = 0;
        private double _maxEquity = Double.MinValue;
        private int _plusEquityCount = 0;
        private bool _forceStop = false;

        private int _sellCount = 0;
        private int _buyCount = 0;
        private List<FxStory> _stories = new List<FxStory>();

        public bool ForceStop
        {
            get { return _forceStop; }
            internal set { _forceStop = value; }
        }

        public int PlusEquityCount
        {
            get { return _plusEquityCount; }
            internal set { _plusEquityCount = value; }
        }

        public double MinEquity
        {
            get { return _minEquity; }
            internal set { _minEquity = value; }
        }

        public int MinusEquityCount
        {
            get { return _minusEquityCount; }
            internal set { _minusEquityCount = value; }
        }

        public double MaxEquity
        {
            get { return _maxEquity; }
            internal set { _maxEquity = value; }
        }

        public double[] Equities
        {
            get { return _equities; }
            internal set { _equities = value; }
        }

        public double Profit
        {
            get { return _profit; }
            internal set { _profit = value; }
        }

        public int OrderCount
        {
            get { return _orderCount; }
            internal set { _orderCount = value; }
        }

        public int PlusCount
        {
            get { return _plusCount; }
            internal set { _plusCount = value; }
        }

        public int MinusCount
        {
            get { return _minusCount; }
            internal set { _minusCount = value; }
        }

        public int SellCount
        {
            get { return _sellCount; }
            internal set { _sellCount = value; }
        }

        public int BuyCount
        {
            get { return _buyCount; }
            internal set { _buyCount = value; }
        }

        internal void AddStory(FxStory story)
        {
            story.Id = _stories.Count + 1;
            _stories.Add(story);
        }

        public IEnumerable<FxStory> Stories
        {
            get { return _stories; }
        }

    }

    public enum FxDirection { None = 0, Down = 1, Up = 2 }

    public enum FxOrderType { Sell, Buy }

    public class FxOrder
    {
        private int _ticket;
        private double _price;
        private FxOrderType _type;
        private double _stopLoss;
        private int _openBarIndex;
        private int _closeBarIndex;

        private decimal _volume = 0.1m; // TODO now fixing

        public FxOrder(FxOrderType type, double price, double stopLoss)
        {
            _price = price;
            _type = type;
            _stopLoss = stopLoss;
        }


        public double StopLoss
        {
            get { return _stopLoss; }
        }

        public int Ticket
        {
            get { return _ticket; }
            internal set { _ticket = value; }
        }

        public int OpenBarIndex
        {
            get { return _openBarIndex; }
            internal set { _openBarIndex = value; }
        }

        public int CloseBarIndex
        {
            get { return _closeBarIndex; }
            internal set { _closeBarIndex = value; }
        }

        public FxOrderType Type
        {
            get { return _type; }
        }

        public double Price
        {
            get { return _price; }
        }

        public decimal Volume
        {
            get { return _volume; }
        }

    }

    public class FxHistoryItem
    {
        private FxOrder _order;
        private double _profit;

        public FxHistoryItem(FxOrder order, double profit)
        {
            _order = order;
            _profit = profit;
        }

        public FxOrder Order
        {
            get { return _order; }
        }

        public double Profit
        {
            get { return _profit; }
        }

    }

    public abstract class FxDirectionTester : FxAbstractTester
    {
        private FxOrder _currentOrder;

        public FxDirectionTester(double deposit, Bar[] prices, int from, int length) :
            base(deposit, prices, from, length)
        {
        }

        protected abstract FxDirection GetDirection(int index);
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
            {
                _currentOrder = null;
            }

            if (_currentOrder == null)
            {
                //switch (_getDirection(CurrentBar - From))
                switch (GetDirection(CurrentBar))
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
            }
            else
            {
                //switch (_getDirection(CurrentBar - From))
                switch (GetDirection(CurrentBar))
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

        }

        private FxOrder Buy()
        {
            int intSellOrder = GetStopLoss();
            double stopLoss = 0.0;
            if (intSellOrder != 0)
            {
                stopLoss = CurrentPrice - (double)intSellOrder / DecimalFactor;
            }
            FxOrder order = new FxOrder(FxOrderType.Buy, CurrentPrice, stopLoss);
            Buy(order);
            return order;
        }

        private FxOrder Sell()
        {
            int intSellOrder = GetStopLoss();
            double stopLoss = 0.0;
            if (intSellOrder != 0)
            {
                stopLoss = CurrentPrice + (double)intSellOrder / DecimalFactor;
            }
            FxOrder order = new FxOrder(FxOrderType.Sell, CurrentPrice, stopLoss);
            Sell(order);
            return order;
        }
    }

    public abstract class FxAbstractTester
    {
        private Bar[] _prices;
        private int _from;
        private int _length;

        private int _lastTicket;
        private Dictionary<int, FxOrder> _orders;
        private List<FxHistoryItem> _history;

        private int _currentBar;
        private double _deposit;
        private int _spred = 3;
        private int _decimals = 4;
        protected int _decimalsFactor = 10000;
        private decimal _point = 0.0001m;

        protected FxTesterResult _result;

        private int _tickNumber = -1;
        private double[] _ticks;
        private DateTime[] _ticksTimes;

        public FxAbstractTester(double deposit, Bar[] prices)
            : this(deposit, prices, 0, prices.Length)
        {
        }

        public List<FxHistoryItem> History
        {
            get { return _history; }
        }

        public int Decimals
        {
            get { return _decimals; }
            set
            {
                _decimals = value;
                _decimalsFactor = (int)Math.Pow(10, _decimals);
                _point = 1 / _decimalsFactor;
            }
        }

        protected int DecimalFactor
        {
            get { return _decimalsFactor; }
        }

        protected decimal Point
        {
            get { return _point; }
        }

        public int Spred
        {
            get { return _spred; }
            set { _spred = value; }
        }

        protected Dictionary<int, FxOrder> Orders
        {
            get { return _orders; }
        }

        public double Deposit
        {
            get { return _deposit; }
            set { _deposit = value; }
        }

        public FxAbstractTester(double deposit, Bar[] prices, int from, int length)
        {
            _deposit = deposit;
            _prices = prices;
            _from = from;
            _length = length;
            if (_from + _length > _prices.Length)
                throw new Exception("Выход зад диапазон");
            Reset();
        }

        public int From
        {
            get { return _from; }
        }

        public virtual void Reset()
        {
            _orders = new Dictionary<int, FxOrder>();
            _history = new List<FxHistoryItem>();
            _lastTicket = 0;
            _currentBar = 0;
            _result = new FxTesterResult();
            _result.Equities = new double[_prices.Length];
        }

        protected abstract void OnTick();

        public virtual FxTesterResult Run()
        {
            for (int i = _from; i < _from + _length; i++)
            {
                if (_result.ForceStop)
                    break;
                _currentBar = i;
                MakeTicks();

            }
            while (_orders.Keys.Count > 0)
            {
                // TODO неоптимально
                FxStoryType closeType = _result.ForceStop ? FxStoryType.CloseByForceStop : FxStoryType.CloseByStop;
                CloseOrder(_orders.Keys.ToArray()[0], CurrentPrice, closeType);
            }
            return _result;
        }

        private void MakeTicks()
        {
            _ticks = new double[4];
            _ticks[0] = _prices[_currentBar].Open;
            _ticks[1] = _prices[_currentBar].Low;
            _ticks[2] = _prices[_currentBar].High;
            _ticks[3] = _prices[_currentBar].Close;

            _ticksTimes = new DateTime[4];
            _ticksTimes[0] = _prices[_currentBar].Time;
            _ticksTimes[1] = _prices[_currentBar].Time;
            _ticksTimes[2] = _prices[_currentBar].Time;
            _ticksTimes[3] = _prices[_currentBar].Time;

            for (int i = 0; i < _ticks.Length; i++)
            {
                _tickNumber = i;
                // TODO Непонятно когда правильно вычислять  Equity
                MakeStopLoss();
                CalculateEquity(CurrentPrice);
                if (_deposit + (_result.Profit + _result.Equities[_currentBar]) * DecimalFactor < 0)
                {
                    _result.ForceStop = true;
                    break;
                }
                OnTick();
            }
        }

        private void MakeStopLoss()
        {
            List<FxOrder> localOrders = new List<FxOrder>(_orders.Values);
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
            double profit = 0.0;
            foreach (var pair in _orders)
            {
                FxOrder order = pair.Value;
                switch (order.Type)
                {
                    case FxOrderType.Buy:
                        profit += price - order.Price - (double)_spred / (double)DecimalFactor;
                        break;
                    case FxOrderType.Sell:
                        profit += order.Price - price - (double)_spred / (double)DecimalFactor;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return profit;
        }

        private void CalculateEquity(double price)
        {
            double profit = GetCurrentEquity(price);
            _result.Equities[_currentBar] = profit;
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


        protected double CurrentPrice
        {
            get { return _ticks[_tickNumber]; }
        }

        protected DateTime CurrentTime
        {
            get { return _ticksTimes[_tickNumber]; }
        }

        protected int CurrentBar
        {
            get { return _currentBar; }
        }

        protected void Buy(FxOrder order)
        {
            ++_lastTicket;

            FxStory story = new FxStory()
            {
                Balance = (decimal)(_deposit + _result.Profit * DecimalFactor),
                OrderTicket = _lastTicket,
                Price = (decimal)CurrentPrice,
                Profit = null,
                StopLoss = order.StopLoss > 0 ? (decimal?)order.StopLoss : (decimal?)null,
                TakeProfit = null, // TODO
                Time = CurrentTime,
                Type = FxStoryType.Buy,
                Volume = (decimal)order.Volume
            };
            _result.AddStory(story);

            order.Ticket = _lastTicket;
            order.OpenBarIndex = CurrentBar;
            _orders.Add(_lastTicket, order);
        }

        protected void Sell(FxOrder order)
        {
            ++_lastTicket;

            FxStory story = new FxStory()
            {
                Balance = (decimal)(_deposit + _result.Profit * DecimalFactor),
                OrderTicket = _lastTicket,
                Price = (decimal)CurrentPrice,
                Profit = null,
                StopLoss = order.StopLoss > 0 ? (decimal?)order.StopLoss : (decimal?)null,
                TakeProfit = null, // TODO
                Time = CurrentTime,
                Type = FxStoryType.Sell,
                Volume = (decimal)order.Volume
            };

            _result.AddStory(story);
            order.Ticket = _lastTicket;
            order.OpenBarIndex = CurrentBar;
            _orders.Add(_lastTicket, order);
        }

        protected void CloseOrder(int ticket)
        {
            CloseOrder(ticket, CurrentPrice, FxStoryType.Close);
        }

        private void CloseOrder(int ticket, double price, FxStoryType closeType)
        {
            FxOrder order = _orders[ticket];
            double profit = 0.0;
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
            _orders.Remove(ticket);
            order.CloseBarIndex = CurrentBar;
            _history.Add(new FxHistoryItem(order, profit));

            FxStory story = new FxStory()
            {
                Balance = (decimal)(_deposit + _result.Profit * DecimalFactor),
                OrderTicket = ticket,
                Price = (decimal)CurrentPrice,
                Profit = (decimal)profit * DecimalFactor,
                StopLoss = order.StopLoss > 0 ? (decimal?)order.StopLoss : (decimal?)null,
                TakeProfit = null, // TODO
                Time = CurrentTime,
                Type = closeType,
                Volume = (decimal)order.Volume
            };
            _result.AddStory(story);
        }

        private double CloseSellOrder(FxOrder order, double price)
        {
            double profit = order.Price - price - (double)_spred / (double)DecimalFactor;
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
            double profit = price - order.Price - (double)_spred / (double)DecimalFactor;
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
