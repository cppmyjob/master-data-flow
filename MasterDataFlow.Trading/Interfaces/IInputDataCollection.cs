using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Data;

namespace MasterDataFlow.Trading.Interfaces
{
    public interface IInputDataCollection
    {
        List<BaseInput> GetInputs();
    }
}
