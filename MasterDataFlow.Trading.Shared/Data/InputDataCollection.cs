using System.Collections.Generic;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Interfaces;
using MasterDataFlow.Trading.Shared.Data.Indicators;

namespace MasterDataFlow.Trading.Shared.Data
{
    public class InputDataCollection : IInputDataCollection
    {
        private IDictionary<string, BaseInput> _inputsDict = new Dictionary<string, BaseInput>();
        private readonly List<BaseInput> _inputsList = new List<BaseInput>();

        public InputDataCollection()
        {
            // RSI
            Add(new TradyRsiIndicatorInput(3));
            Add(new TradyRsiIndicatorInput(7));
            Add(new TradyRsiIndicatorInput(14));
//            Add(new TradyRsiIndicatorInput(21));
//            Add(new TradyRsiIndicatorInput(28));

            // SAR
            //Add(new SarIndicatorInput());

            // EMA
            //Add(new EmaInput(3));
            //Add(new EmaInput(5));
            //Add(new EmaInput(10));
            //Add(new EmaInput(15));
            //Add(new EmaInput(20));
            //Add(new EmaInput(25));

//            Add(new TradyMacdHistogramIndicatorInput());
            Add(new TradyMacdSignalLineIndicatorInput());
            Add(new TradyMacdLineIndicatorInput());

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
