using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Intelligence.Neuron;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Tester;

namespace MasterDataFlow.Trading.Genetic
{
    public class DirectionTester : FxDirectionTester
    {
        private readonly GeneticNeuronDLL1 _dll;
        private readonly LearningData _learningData;
        private const double START_DEPOSIT = 100000;

        public DirectionTester(GeneticNeuronDLL1 dll, LearningData learningData) 
            : base(START_DEPOSIT, learningData.Prices, 0, learningData.Prices.Length)
        {
            _dll = dll;
            _learningData = learningData;
        }

        protected override FxDirectionGetDirection GetDirectionDelegate()
        {
            throw new NotImplementedException();
        }

        protected override int GetStopLoss()
        {
            throw new NotImplementedException();
        }
    }
}
