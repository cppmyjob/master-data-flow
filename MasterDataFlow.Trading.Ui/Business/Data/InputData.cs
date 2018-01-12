using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Ui.Business.Data.Indicators;

namespace MasterDataFlow.Trading.Ui.Business.Data
{
    public class InputData
    {
        public IDictionary<string, BaseInput> _inputs = new Dictionary<string, BaseInput>();

        public InputData()
        {
            Add(new MacdHistogramIndicatorInput());
            Add(new MacdSignalLineIndicatorInput());
        }

        public void Add(BaseInput input)
        {
            _inputs.Add(input.Name, input);
        }

    }
}
