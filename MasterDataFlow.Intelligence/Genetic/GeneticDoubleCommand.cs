using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Intelligence.Genetic
{
    [Serializable]
    public class GeneticDoubleDataObject<TGeneticDoubleItem> : GeneticDataObject<GeneticItemInitData, TGeneticDoubleItem, double>
        where TGeneticDoubleItem : GeneticDoubleItem
    {

    }

    [Serializable]
    public abstract class GeneticDoubleItem : GeneticItem<GeneticItemInitData, double>
    {
        protected GeneticDoubleItem(GeneticItemInitData initData)
            : base(initData)
        {
        }

        public override double ParseStringValue(string value)
        {
            return Double.Parse(value);
        }
    }

    public abstract class GeneticDoubleCommand<TGeneticDoubleDataObject, TGeneticDoubleItem>
        : GeneticCommand<TGeneticDoubleDataObject, GeneticItemInitData, TGeneticDoubleItem, double>
        where TGeneticDoubleDataObject : GeneticDoubleDataObject<TGeneticDoubleItem>
        where TGeneticDoubleItem : GeneticDoubleItem
    {

    }

}
