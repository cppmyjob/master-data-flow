using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDataFlow.MetaTrader.Api.Dto
{
    public class TickDto
    {
        public int Direction { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return Convert.ToString(Direction) + "|" + Value;
        }
    }
}
