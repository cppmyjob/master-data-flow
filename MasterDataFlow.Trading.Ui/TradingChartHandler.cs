using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Tester;

namespace MasterDataFlow.Trading.Ui
{
    public class TradingChartHandler
    {
        private readonly Chart _chart;
        private readonly Series _pricesSeries;
        private readonly Series _buySeries;
        private readonly Series _sellSeries;
        private readonly Series _closeSeries;
        private readonly Series _zigZagSeries;
        
        private readonly ChartArea _chartArea;
        private readonly ChartArea _chartArea2;

        private Bar[] _prices;

        public TradingChartHandler(Chart chart)
        {
            _chart = chart;
            _chart.DataManipulator.IsEmptyPointIgnored = false;
            _chart.Series.Clear();
            _chart.Legends.Clear();
            _chart.ChartAreas.Clear();

            _chart = chart;
            _chart.MouseWheel += OnMouseWheel;
            _chart.AxisViewChanged += OnAxisViewChanged;


            _chartArea = _chart.ChartAreas.Add("MainChartArea");
            //_chartArea.AxisX.LabelStyle.Format = "dd.MM.yyyy hh:mm";


            const bool isXValueIndexed = false;

            //_chartArea.CursorX.IsUserEnabled = true;
            //_chartArea.CursorY.IsUserEnabled = true;

            _pricesSeries = _chart.Series.Add("Prices");
            _pricesSeries.ChartType = SeriesChartType.Candlestick;
            _pricesSeries.Color = Color.Black;
            _pricesSeries.ChartArea = _chartArea.Name;
            _pricesSeries.IsXValueIndexed = isXValueIndexed;
            //_pricesSeries.XValueType = ChartValueType.DateTime;
            _pricesSeries.YValuesPerPoint = 4;

            _buySeries = _chart.Series.Add("Buy");
            _buySeries.ChartArea = _chartArea.Name;
            _buySeries.Color = Color.Blue;
            _buySeries.ChartType = SeriesChartType.Point;
            _buySeries.IsXValueIndexed = isXValueIndexed;
            _buySeries.YValuesPerPoint = 2;

            _sellSeries = _chart.Series.Add("Sell");
            _sellSeries.ChartArea = _chartArea.Name;
            _sellSeries.Color = Color.Green;
            _sellSeries.ChartType = SeriesChartType.Point;
            _sellSeries.IsXValueIndexed = isXValueIndexed;
            _sellSeries.YValuesPerPoint = 2;


            _closeSeries = _chart.Series.Add("Close");
            _closeSeries.ChartArea = _chartArea.Name;
            _closeSeries.Color = Color.Red;
            _closeSeries.ChartType = SeriesChartType.Point;
            _closeSeries.IsXValueIndexed = isXValueIndexed;
            _closeSeries.YValuesPerPoint = 2;


            _zigZagSeries = _chart.Series.Add("ZigZag");
            _zigZagSeries.ChartType = SeriesChartType.Line;
            _zigZagSeries.Color = Color.DarkRed;
            _zigZagSeries.ChartArea = _chartArea.Name;
            _zigZagSeries.IsXValueIndexed = isXValueIndexed;
            _zigZagSeries.XValueType = ChartValueType.DateTime;
            _zigZagSeries.YValuesPerPoint = 4;


        }

        public void SetZigZag(int[] indexes)
        {
            var isHigh = _prices[indexes[0]].High > _prices[indexes[1]].Low;

            for (int i = 0; i < indexes.Length; i++)
            {
                var index = indexes[i];
                int pi;
                if (isHigh)
                    pi = _zigZagSeries.Points.AddXY(index, _prices[index].High);
                else
                    pi = _zigZagSeries.Points.AddXY(index, _prices[index].Low);
                isHigh = !isHigh;
                _pricesSeries.Points[pi].AxisLabel = _prices[index].Time.ToString("G");
            }
        }


        public void SetPrices(Bar[] prices)
        {
            _prices = prices;
            _pricesSeries.Points.Clear();

            for (int i = 0; i < prices.Length; i++)
            {
                var quote = prices[i];
                var index = _pricesSeries.Points.AddXY(i, quote.Low, quote.High, quote.Open, quote.Close);
                _pricesSeries.Points[index].AxisLabel = quote.Time.ToString("G");
            }

            SetChartMinMaxPrices(_prices);
        }

        private enum OrderPointType
        {
            None, Buy, Sell, Close,
        }

        private class OrderPoint
        {
            public int Index { get; set; }
            public DateTime Time { get; set; }
            public decimal? Buy { get; set; }
            public decimal? Sell { get; set; }
            public decimal? Stop { get; set; }
        }

        public void SetStories(IEnumerable<Story> stories)
        {
            _buySeries.Points.Clear();
            _sellSeries.Points.Clear();
            _closeSeries.Points.Clear();

            var a = stories.ToArray();
            if (a.Length == 0)
                return;

            var j = 0;

            OrderPointType currentOrderType = OrderPointType.None;
            OrderPointType lastOrderType = OrderPointType.None;

            for (int i = 0; i < _prices.Length; i++)
            {
                var price = _prices[i];
                var orderPoint = new OrderPoint();
                orderPoint.Index = i;
                orderPoint.Time = price.Time;
                lastOrderType = currentOrderType;
                while (j < a.Length && price.Time == a[j].Time)
                {
                    switch (a[j].Type)
                    {
                        case StoryType.Buy:
                            orderPoint.Buy = a[j].Price;
                            currentOrderType = OrderPointType.Buy;
                            break;
                        case StoryType.Sell:
                            orderPoint.Sell = a[j].Price;
                            currentOrderType = OrderPointType.Sell;
                            break;
                        default:
                            orderPoint.Stop = a[j].Price;
                            currentOrderType = OrderPointType.Close;
                            break;
                    }
                    ++j;
                }
                if (lastOrderType == currentOrderType)
                {
                    switch (lastOrderType)
                    {
                        case OrderPointType.Buy:
                            orderPoint.Buy = price.Close;
                            break;
                        case OrderPointType.Sell:
                            orderPoint.Sell = price.Close;
                            break;
                        case OrderPointType.Close:
                            currentOrderType = OrderPointType.None;
                            break;
                    }
                }
                AddOrderPoint(orderPoint);
            }

        }

        private void AddOrderPoint(OrderPoint orderPoint)
        {
            if (orderPoint.Buy.HasValue)
                _buySeries.Points.AddXY(orderPoint.Index, orderPoint.Buy.Value);
            else
            {
                var index = _buySeries.Points.AddXY(orderPoint.Index, 0);
                _buySeries.Points[index].IsEmpty = true;
            }


            if (orderPoint.Sell.HasValue)
                _sellSeries.Points.AddXY(orderPoint.Index, orderPoint.Sell.Value);
            else
            {
                var index = _sellSeries.Points.AddXY(orderPoint.Index, 0);
                _sellSeries.Points[index].IsEmpty = true;
            }

            if (orderPoint.Stop.HasValue)
                _closeSeries.Points.AddXY(orderPoint.Index, orderPoint.Stop.Value);
            else
            {
                var index = _closeSeries.Points.AddXY(orderPoint.Index, 0);
                _closeSeries.Points[index].IsEmpty = true;
            }
        }

        private void OnMouseWheel(object sender, MouseEventArgs mouseEventArgs)
        {
            try
            {
                if (mouseEventArgs.Delta < 0)
                {
                    _chartArea.AxisX.ScaleView.ZoomReset();
                }

                if (mouseEventArgs.Delta > 0)
                {
                    double xMin = _chartArea.AxisX.ScaleView.ViewMinimum;
                    double xMax = _chartArea.AxisX.ScaleView.ViewMaximum;

                    double posXStart =
                        _chartArea.AxisX.PixelPositionToValue(mouseEventArgs.Location.X) -
                        (xMax - xMin) / 4;
                    double posXFinish =
                        _chartArea.AxisX.PixelPositionToValue(mouseEventArgs.Location.X) +
                        (xMax - xMin) / 4;

                    posXStart = 0 <= (int)posXStart && (int)posXStart < _prices.Length ? (int)posXStart : 0;
                    posXFinish = 0 <= (int)posXFinish && (int)posXFinish < _prices.Length
                        ? (int)posXFinish
                        : _prices.Length - 1;

                    var dateStart = _prices[(int)posXStart].Time;
                    var dateEnd = _prices[(int)posXFinish].Time;
                    ;

                    //var dateStart = DateTime.FromOADate(posXStart);
                    //var dateEnd = DateTime.FromOADate(posXFinish);

                    var barRange = _prices.SkipWhile(t => t.Time <= dateStart).TakeWhile(t => t.Time <= dateEnd)
                        .ToArray();
                    SetChartMinMaxPrices(barRange);

                    _chartArea.AxisX.ScaleView.Zoom(posXStart, posXFinish);
                }
                else
                {
                    SetChartMinMaxPrices(_prices);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void OnAxisViewChanged(object sender, ViewEventArgs e)
        {
            try
            {
                var posXStart = _chartArea.AxisX.ScaleView.ViewMinimum;
                var posXFinish = _chartArea.AxisX.ScaleView.ViewMaximum;

                posXStart = 0 <= (int)posXStart && (int)posXStart < _prices.Length ? (int)posXStart : 0;
                posXFinish = 0 <= (int)posXFinish && (int)posXFinish < _prices.Length ? (int)posXFinish : _prices.Length - 1;


                var dateStart = _prices[(int)posXStart].Time;
                var dateEnd = _prices[(int)posXFinish].Time; ;

                //var dateStart = DateTime.FromOADate(posXStart);
                //var dateEnd = DateTime.FromOADate(posXFinish);


                var barRange = _prices.SkipWhile(t => t.Time <= dateStart).TakeWhile(t => t.Time <= dateEnd)
                    .ToArray();
                SetChartMinMaxPrices(barRange);
            }
            catch { }
        }

        private void SetChartMinMaxPrices(Bar[] bars)
        {
            var minimum = Math.Min(bars.Min(t => t.Low), bars.Min(t => t.Low));
            var maximum = Math.Max(bars.Max(t => t.High), bars.Max(t => t.High));
            _chartArea.AxisY.Minimum = (double)minimum;
            _chartArea.AxisY.Maximum = (double)maximum;

        }
    }
}
