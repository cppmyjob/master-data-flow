using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Genetic;
using MasterDataFlow.Intelligence.Interfaces;

namespace MasterDataFlow.Intelligence
{

    public class NeuronGeneticDataObject : GeneticFloatDataObject<NeuronGeneticFloatItem>
    {
        
    }

    public class NeuronGeneticFloatItem : GeneticFloatItem
    {
        public NeuronGeneticFloatItem(GeneticItemInitData initData) : base(initData)
        {
        }

        public override int CreateValue(IRandom random)
        {
            throw new NotImplementedException();
        }
    }


    public class NeuronGeneticCommand : GeneticFloatCommand<NeuronGeneticDataObject, NeuronGeneticFloatItem>
    {
        protected override NeuronGeneticFloatItem CreateItem(GeneticItemInitData initData)
        {
            throw new NotImplementedException();
        }

        public override double CalculateFitness(NeuronGeneticFloatItem item, int processor)
        {
            throw new NotImplementedException();
        }
    }
}
