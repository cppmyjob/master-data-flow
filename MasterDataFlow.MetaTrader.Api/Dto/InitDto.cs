using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDataFlow.MetaTrader.Api.Dto
{
    public class InitDto
    {
        public bool IsError { get; set; }

        public override string ToString()
        {
            return IsError ? "true" : "false";
        }
    }
}
