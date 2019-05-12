using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.MetaTrader.Data.Indicators;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Interfaces;

namespace MasterDataFlow.MetaTrader.Data
{
    public class InputDataCollection : IInputDataCollection
    {
        private readonly IDictionary<string, BaseInput> _inputsDict = new Dictionary<string, BaseInput>();
        private readonly List<BaseInput> _inputsList = new List<BaseInput>();

        public InputDataCollection()
        {
            // RSI
            Add(new MetaTraderRsiIndicatorInput(3));
            Add(new MetaTraderRsiIndicatorInput(7));
            Add(new MetaTraderRsiIndicatorInput(14));
            Add(new MetaTraderRsiIndicatorInput(21));
            Add(new MetaTraderRsiIndicatorInput(28));

            // SAR
            //Add(new SarIndicatorInput());

            // EMA
            //Add(new EmaInput(3));
            //Add(new EmaInput(5));
            //Add(new EmaInput(10));
            //Add(new EmaInput(15));
            //Add(new EmaInput(20));
            //Add(new EmaInput(25));

            Add(new MetaTraderMacdHistogramIndicatorInput());
            Add(new MetaTraderMacdSignalLineIndicatorInput());
            Add(new MetaTraderMacdLineIndicatorInput());

            //Add(new VolumesInput());
        }

        public void Add(BaseInput input)
        {
            _inputsList.Add(input);
            _inputsDict.Add(input.Name, input);
        }

        public List<BaseInput> GetInputs()
        {
            return _inputsList;
        }
    }
}
