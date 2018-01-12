using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Data;

namespace MasterDataFlow.Trading.Tester
{
    public enum StoryType { Sell, Buy, Close, CloseByStopLoss, CloseByTakeProfit, CloseByForceStop, CloseByStop }

    [Serializable]
    public class Story
    {
        public int Id { get; internal set; }
        public DateTime Time { get; internal set; }
        public StoryType Type { get; internal set; }
        public int OrderTicket { get; internal set; }
        //public decimal Volume { get; internal set; }
        public decimal Price { get; internal set; }
        public decimal? StopLoss { get; internal set; }
        public decimal? TakeProfit { get; internal set; }
        public decimal? Profit { get; internal set; }
        public decimal Balance { get; internal set; }
    }

    [Serializable]
    public class TesterResult
    {
        private decimal _profit = 0.0m;
        private int _orderCount = 0;
        private int _plusCount = 0;
        private int _minusCount = 0;
        private decimal[] _equities = null;
        private decimal _minEquity = decimal.MaxValue;
        private int _minusEquityCount = 0;
        private decimal _maxEquity = decimal.MinValue;
        private int _plusEquityCount = 0;
        private bool _forceStop = false;

        private int _sellCount = 0;
        private int _buyCount = 0;
        private List<Story> _stories = new List<Story>();
        private List<Order> _orders = new List<Order>();

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

        public decimal MinEquity
        {
            get { return _minEquity; }
            internal set { _minEquity = value; }
        }

        public int MinusEquityCount
        {
            get { return _minusEquityCount; }
            internal set { _minusEquityCount = value; }
        }

        public decimal MaxEquity
        {
            get { return _maxEquity; }
            internal set { _maxEquity = value; }
        }

        public decimal[] Equities
        {
            get { return _equities; }
            internal set { _equities = value; }
        }

        public decimal Profit
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

        internal void AddStory(Story story)
        {
            story.Id = _stories.Count + 1;
            _stories.Add(story);
        }

        internal void AddOrder(Order order)
        {
            _orders.Add(order);
        }

        public IEnumerable<Story> Stories
        {
            get { return _stories; }
        }

        public List<Order> Orders
        {
            get { return _orders; }
        }
    }

    public enum Direction { Hold = 0, Close = 1, Down = 2, Up = 3 }

    public enum OrderType { Sell, Buy }

    [Serializable]
    public class Order
    {
        private int _ticket;
        private decimal _price;
        private OrderType _type;
        private decimal _stopLoss;
        private int _openBarIndex;
        private int _closeBarIndex;

        //private decimal _volume = 0.1m; // TODO now fixing

        public Order(OrderType type, decimal price, decimal stopLoss)
        {
            _price = price;
            _type = type;
            _stopLoss = stopLoss;
        }


        public decimal StopLoss
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

        public decimal Profit { get; internal set; }

        public OrderType Type
        {
            get { return _type; }
        }

        public decimal Price
        {
            get { return _price; }
        }

        //public decimal Volume
        //{
        //    get { return _volume; }
        //}

    }

    public class HistoryItem
    {
        private Order _order;
        private decimal _profit;

        public HistoryItem(Order order, decimal profit)
        {
            _order = order;
            _profit = profit;
        }

        public Order Order
        {
            get { return _order; }
        }

        public decimal Profit
        {
            get { return _profit; }
        }

    }

    public abstract class DirectionTester : SignleOrderTester
    {

        public DirectionTester(decimal deposit, Bar[] prices, int from, int length) :
            base(deposit, prices, from, length)
        {
            _lastBarNumber = -1;
        }

        protected abstract Direction GetDirection(int index);

        private int _lastBarNumber = -1;

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
                    case Direction.Up:
                        _currentOrder = Buy();
                        break;
                    case Direction.Down:
                        _currentOrder = Sell();
                        break;
                    case Direction.Hold:
                        break;
                    case Direction.Close:
                        break;
                }
            }
            else
            {
                //switch (_getDirection(CurrentBar - From))
                switch (GetDirection(CurrentBar))
                {
                    case Direction.Up:
                        if (_currentOrder.Type == OrderType.Sell)
                        {
                            CloseOrder(_currentOrder.Ticket);
                            _currentOrder = Buy();
                        }
                        break;
                    case Direction.Down:
                        if (_currentOrder.Type == OrderType.Buy)
                        {
                            CloseOrder(_currentOrder.Ticket);
                            _currentOrder = Sell();
                        }
                        break;
                    case Direction.Close:
                        CloseOrder(_currentOrder.Ticket);
                        _currentOrder = null;
                        break;
                    case Direction.Hold:
                        break;
                }
            }

        }


    }

    public abstract class SignleOrderTester : AbstractTester
    {
        protected Order _currentOrder;

        protected SignleOrderTester(decimal deposit, Bar[] prices, int from, int length) :
            base(deposit, prices, from, length)
        {
            _currentOrder = null;

        }

        protected abstract decimal GetStopLoss();


        protected Order Buy()
        {
            decimal stopLossValue = GetStopLoss();
            decimal stopLoss = 0.0m;
            if (stopLossValue != 0)
            {
                stopLoss = CurrentPrice - stopLossValue;
            }
            Order order = new Order(OrderType.Buy, CurrentPrice, stopLoss);
            Buy(order);
            return order;
        }

        protected Order Sell()
        {
            decimal stopLossValue = GetStopLoss();
            decimal stopLoss = 0.0m;
            if (stopLossValue != 0)
            {
                stopLoss = CurrentPrice + stopLossValue;
            }
            Order order = new Order(OrderType.Sell, CurrentPrice, stopLoss);
            Sell(order);
            return order;
        }
    }


    public abstract class AbstractTester
    {
        private Bar[] _prices;
        private int _from;
        private int _length;

        private int _lastTicket;
        private Dictionary<int, Order> _orders;
        private List<HistoryItem> _history;

        private int _currentBar;
        private decimal _deposit;
        private decimal _spred = 0.05m;

        protected TesterResult _result;

        private int _tickNumber = -1;
        private decimal[] _ticks;
        private DateTime[] _ticksTimes;

        public AbstractTester(decimal deposit, Bar[] prices)
            : this(deposit, prices, 0, prices.Length)
        {
        }

        public List<HistoryItem> History
        {
            get { return _history; }
        }


        public decimal Spred
        {
            get { return _spred; }
            set { _spred = value; }
        }

        protected Dictionary<int, Order> Orders
        {
            get { return _orders; }
        }

        public decimal Deposit
        {
            get { return _deposit; }
            set { _deposit = value; }
        }

        public AbstractTester(decimal deposit, Bar[] prices, int from, int length)
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

        private void Reset()
        {
            _orders = new Dictionary<int, Order>();
            _history = new List<HistoryItem>();
            _lastTicket = 0;
            _currentBar = 0;
            _result = new TesterResult();
            _result.Equities = new decimal[_prices.Length];
        }

        protected abstract void OnTick();

        public virtual TesterResult Run()
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
                StoryType closeType = _result.ForceStop ? StoryType.CloseByForceStop : StoryType.CloseByStop;
                CloseOrder(_orders.Keys.ToArray()[0], CurrentPrice, closeType);
            }
            return _result;
        }

        private void MakeTicks()
        {
            _ticks = new decimal[4];
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
                if (_deposit + (_result.Profit + _result.Equities[_currentBar]) < 0)
                {
                    _result.ForceStop = true;
                    break;
                }
                OnTick();
            }
        }

        private void MakeStopLoss()
        {
            List<Order> localOrders = new List<Order>(_orders.Values);
            foreach (var order in localOrders)
            {
                if (order.StopLoss == 0)
                    continue;
                switch (order.Type)
                {
                    case OrderType.Buy:
                        if (CurrentPrice <= order.StopLoss)
                        {
                            CalculateEquity(order.StopLoss);
                            CloseOrder(order.Ticket, order.StopLoss, StoryType.CloseByStopLoss);
                        }
                        break;
                    case OrderType.Sell:
                        if (CurrentPrice >= order.StopLoss)
                        {
                            CalculateEquity(order.StopLoss);
                            CloseOrder(order.Ticket, order.StopLoss, StoryType.CloseByStopLoss);
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        protected decimal GetCurrentEquity()
        {
            return GetCurrentEquity(CurrentPrice);
        }

        private decimal GetCurrentEquity(decimal price)
        {
            decimal profit = 0.0m;
            foreach (var pair in _orders)
            {
                Order order = pair.Value;
                switch (order.Type)
                {
                    case OrderType.Buy:
                        profit += price - order.Price - _spred;
                        break;
                    case OrderType.Sell:
                        profit += order.Price - price - _spred;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return profit;
        }

        private void CalculateEquity(decimal price)
        {
            decimal profit = GetCurrentEquity(price);
            _result.Equities[_currentBar] = profit;
            if (profit == 0.0m)
                return;
            if (profit >= 0.0m)
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


        protected decimal CurrentPrice
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

        protected void Buy(Order order)
        {
            ++_lastTicket;

            Story story = new Story()
            {
                Balance = (decimal)(_deposit + _result.Profit),
                OrderTicket = _lastTicket,
                Price = (decimal)CurrentPrice,
                Profit = null,
                StopLoss = order.StopLoss > 0 ? (decimal?)order.StopLoss : (decimal?)null,
                TakeProfit = null, // TODO
                Time = CurrentTime,
                Type = StoryType.Buy,
                //Volume = (decimal)order.Volume
            };
            _result.AddStory(story);

            order.Ticket = _lastTicket;
            order.OpenBarIndex = CurrentBar;
            _orders.Add(_lastTicket, order);
            _result.AddOrder(order);
        }

        protected void Sell(Order order)
        {
            ++_lastTicket;

            Story story = new Story()
            {
                Balance = _deposit + _result.Profit,
                OrderTicket = _lastTicket,
                Price = CurrentPrice,
                Profit = null,
                StopLoss = order.StopLoss > 0 ? (decimal?)order.StopLoss : (decimal?)null,
                TakeProfit = null, // TODO
                Time = CurrentTime,
                Type = StoryType.Sell,
                //Volume = (decimal)order.Volume
            };

            _result.AddStory(story);
            order.Ticket = _lastTicket;
            order.OpenBarIndex = CurrentBar;
            _orders.Add(_lastTicket, order);
            _result.AddOrder(order);
        }

        protected void CloseOrder(int ticket)
        {
            CloseOrder(ticket, CurrentPrice, StoryType.Close);
        }

        private void CloseOrder(int ticket, decimal price, StoryType closeType)
        {
            Order order = _orders[ticket];
            decimal profit = 0.0m;
            switch (order.Type)
            {
                case OrderType.Buy:
                    profit = CloseBuyOrder(order, price);
                    break;
                case OrderType.Sell:
                    profit = CloseSellOrder(order, price);
                    break;
                default:
                    throw new NotImplementedException();
            }
            _orders.Remove(ticket);
            order.CloseBarIndex = CurrentBar;
            _history.Add(new HistoryItem(order, profit));

            Story story = new Story()
            {
                Balance = (decimal)_deposit + _result.Profit,
                OrderTicket = ticket,
                Price = (decimal)CurrentPrice,
                Profit = (decimal)profit,
                StopLoss = order.StopLoss > 0 ? order.StopLoss : (decimal?)null,
                TakeProfit = null, // TODO
                Time = CurrentTime,
                Type = closeType,
                //Volume = (decimal)order.Volume
            };
            _result.AddStory(story);
        }

        private decimal CloseSellOrder(Order order, decimal price)
        {
            decimal profit = order.Price - price - _spred;
            if (profit >= 0)
                _result.PlusCount += 1;
            else
                _result.MinusCount += 1;
            _result.Profit += profit;
            order.Profit = profit;
            ++_result.OrderCount;
            ++_result.SellCount;
            return profit;
        }

        private decimal CloseBuyOrder(Order order, decimal price)
        {
            decimal profit = price - order.Price - _spred;
            if (profit >= 0)
                _result.PlusCount += 1;
            else
                _result.MinusCount += 1;
            _result.Profit += profit;
            order.Profit = profit;
            ++_result.OrderCount;
            ++_result.BuyCount;
            return profit;
        }

    }

}
