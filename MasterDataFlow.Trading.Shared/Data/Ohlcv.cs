using System;
using Trady.Core.Infrastructure;

namespace MasterDataFlow.Trading.Shared.Data
{
    public class Ohlcv : IOhlcv
    {
        public DateTimeOffset DateTime { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
    }
}
