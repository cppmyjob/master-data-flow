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
    public class TradingChart
    {
        private readonly Chart _chart;
        private readonly Series _pricesSeries;
        private readonly Series _buySeries;
        private readonly Series _sellSeries;
        private readonly Series _closeSeries;
        private readonly ChartArea _chartArea;

        private Bar[] _prices;

        public TradingChart(Chart chart)
        {
            _chart = chart;
            _chart.Series.Clear();
            _chart.Legends.Clear();
            _chart.ChartAreas.Clear();

            _chart = chart;
            _chart.MouseWheel += OnMouseWheel;
            _chart.AxisViewChanged += OnAxisViewChanged;


            _chartArea = _chart.ChartAreas.Add("MainChartArea");
            _chartArea.AxisX.LabelStyle.Format = "dd.MM.yyyy hh:mm";
            _chartArea.CursorX.IsUserEnabled = true;
            _chartArea.CursorY.IsUserEnabled = true;

            _pricesSeries = _chart.Series.Add("Prices");
            _pricesSeries.ChartType = SeriesChartType.Candlestick;
            _pricesSeries.Color = Color.Black;
            _pricesSeries.ChartArea = _chartArea.Name;
            _pricesSeries.IsXValueIndexed = true;
            _pricesSeries.XValueType = ChartValueType.DateTime;
            _pricesSeries.YValuesPerPoint = 4;

            _buySeries = _chart.Series.Add("Buy");
            _buySeries.ChartArea = _chartArea.Name;
            _buySeries.Color = Color.Blue;
            _buySeries.ChartType = SeriesChartType.Point;
            _buySeries.IsXValueIndexed = true;
            _buySeries.YValuesPerPoint = 2;

            _sellSeries = _chart.Series.Add("Sell");
            _sellSeries.ChartArea = _chartArea.Name;
            _sellSeries.Color = Color.Green;
            _sellSeries.ChartType = SeriesChartType.Point;
            _sellSeries.IsXValueIndexed = true;
            _sellSeries.YValuesPerPoint = 2;


            _closeSeries = _chart.Series.Add("Close");
            _closeSeries.ChartArea = _chartArea.Name;
            _closeSeries.Color = Color.Red;
            _closeSeries.ChartType = SeriesChartType.Point;
            _closeSeries.IsXValueIndexed = true;
            _closeSeries.YValuesPerPoint = 2;

        }

        public void SetPrices(Bar[] prices)
        {
            _prices = prices;
            _pricesSeries.Points.Clear();
            foreach (var quote in prices)
            {
                _pricesSeries.Points.AddXY(quote.Time, quote.Low, quote.High, quote.Open, quote.Close);
            }
            SetChartMinMaxPrices(_prices);
        }

        public void Test()
        {
            foreach (var quote in _prices)
            {
                var index = _buySeries.Points.AddXY(quote.Time, quote.Low);
                index = _buySeries.Points.AddXY(quote.Time, quote.Low);
            }
        }


        private enum OrderPointType
        {
            None, Buy, Sell, Close,
        }

        private class OrderPoint
        {
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

            var i = 0;

            OrderPointType currentOrderType = OrderPointType.None;
            OrderPointType lastOrderType = OrderPointType.None;
            foreach (var price in _prices)
            {
                var orderPoint = new OrderPoint();
                orderPoint.Time = price.Time;
                lastOrderType = currentOrderType;
                while (i < a.Length && price.Time == a[i].Time)
                {
                    switch (a[i].Type)
                    {
                        case StoryType.Buy:
                            orderPoint.Buy = a[i].Price;
                            currentOrderType = OrderPointType.Buy;
                            break;
                        case StoryType.Sell:
                            orderPoint.Sell = a[i].Price;
                            currentOrderType = OrderPointType.Sell;
                            break;
                        default:
                            orderPoint.Stop = a[i].Price;
                            currentOrderType = OrderPointType.Close;
                            break;
                    }
                    ++i;
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
                _buySeries.Points.AddXY(orderPoint.Time, orderPoint.Buy.Value);
            else
            {
                var index = _buySeries.Points.AddXY(orderPoint.Time, 0);
                _buySeries.Points[index].IsEmpty = true;
            }


            if (orderPoint.Sell.HasValue)
                _sellSeries.Points.AddXY(orderPoint.Time, orderPoint.Sell.Value);
            else
            {
                var index = _sellSeries.Points.AddXY(orderPoint.Time, 0);
                _sellSeries.Points[index].IsEmpty = true;
            }

            if (orderPoint.Stop.HasValue)
                _closeSeries.Points.AddXY(orderPoint.Time, orderPoint.Stop.Value);
            else
            {
                var index = _closeSeries.Points.AddXY(orderPoint.Time, 0);
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
