using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MasterDataFlow.Trading.Data;

namespace MasterDataFlow.Trading.Ui
{
    public class TradingChart
    {
        private readonly Chart _chart;
        private readonly Series _pricesSeries;
        private readonly Series _buySeries;
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
            _pricesSeries.ChartArea = _chartArea.Name;
            _pricesSeries.IsXValueIndexed = true;
            _pricesSeries.XValueType = ChartValueType.DateTime;
            _pricesSeries.YValuesPerPoint = 4;

            _buySeries = _chart.Series.Add("Buy");
            _buySeries.ChartArea = _chartArea.Name;
            _buySeries.Color = Color.Red;
            _buySeries.ChartType = SeriesChartType.Point;
            _buySeries.IsXValueIndexed = true;
            _buySeries.YValuesPerPoint = 2;


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
                        : 0;

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
                posXFinish = 0 <= (int)posXFinish && (int)posXFinish < _prices.Length ? (int)posXFinish : 0;


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
