using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Ui.Business.Data;
using Trady.Core.Infrastructure;

namespace MasterDataFlow.Trading.Ui.Business
{
    public static class Helper
    {
        public static Bar[] CandlesToBars(IEnumerable<IOhlcv> candles)
        {
            var result = candles.Select(t => new Bar
            {
                Close = t.Close,
                High = t.High,
                Low = t.Low,
                Open = t.Open,
                Volume = t.Volume,
                Time = t.DateTime.DateTime,
            }).ToArray();
            return result;
        }

        public static IOhlcv[] BarsToCandles(IEnumerable<Bar> bars)
        {
            var result = bars.Select(t => new Ohlcv
            {
                Close = t.Close,
                High = t.High,
                Low = t.Low,
                Open = t.Open,
                Volume = t.Volume,
                DateTime = t.Time,
            }).ToArray();
            return result;
        }

    }
}
