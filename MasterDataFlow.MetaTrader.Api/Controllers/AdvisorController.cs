using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using MasterDataFlow.MetaTrader.Api.Dto;
using MasterDataFlow.MetaTrader.Api.Infarastructure;

namespace MasterDataFlow.MetaTrader.Api.Controllers
{
    public class AdvisorController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Init()
        {
            var dto = new InitDto();
            dto.IsError = false;
            return new StringResult(Request, dto.ToString());
        }

        [HttpPost]
        public IHttpActionResult Tick(string value)
        {
            var dto = new TickDto();
            dto.Direction = 33;
            dto.Value = value;
            return new StringResult(Request, dto.ToString());
        }

    }
}
