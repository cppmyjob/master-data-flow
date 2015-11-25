using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Genetic;

namespace MasterDataFlow.Intelligence
{

    public class NeuronGeneticDataObject : GeneticFloatDataObject
    {
        
    }

    public class NeuronGeneticCommand : GeneticFloatCommand<NeuronGeneticDataObject>
    {
        protected override GeneticFloatItem CreateItem(GeneticItemInitData initData)
        {
            throw new NotImplementedException();
        }

        public override double CalculateFitness(GeneticFloatItem item, int processor)
        {
            throw new NotImplementedException();
        }
    }
}
