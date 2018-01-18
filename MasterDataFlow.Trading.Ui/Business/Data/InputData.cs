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
        private IDictionary<string, BaseInput> _inputsDict = new Dictionary<string, BaseInput>();

        public InputData()
        {
            // RSI
            Add(new RsiIndicatorInput(3));
            Add(new RsiIndicatorInput(7));
            Add(new RsiIndicatorInput(14));
            Add(new RsiIndicatorInput(21));
            Add(new RsiIndicatorInput(28));

            // SAR
            //Add(new SarIndicatorInput());

            // EMA
            //Add(new EmaInput(3));
            //Add(new EmaInput(5));
            //Add(new EmaInput(10));
            //Add(new EmaInput(15));
            //Add(new EmaInput(20));
            //Add(new EmaInput(25));

            Add(new MacdHistogramIndicatorInput());
            Add(new MacdSignalLineIndicatorInput());
            Add(new MacdLineIndicatorInput());

            //Add(new VolumesInput());
        }

        public void Add(BaseInput input)
        {
            _inputsDict.Add(input.Name, input);
        }

        public BaseInput GetInput(string name)
        {
            BaseInput result;
            if (_inputsDict.TryGetValue(name, out result))
                return result;
            return null;
        }

        public BaseInput[] GetInputs()
        {
            return _inputsDict.Values.ToArray();
        }

    }
}
