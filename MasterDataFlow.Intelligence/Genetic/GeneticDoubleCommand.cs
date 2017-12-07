using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Intelligence.Genetic
{
    [Serializable]
    public class GeneticDoubleDataObject<TGeneticDoubleItem> : GeneticDataObject<TGeneticDoubleItem, double>
        where TGeneticDoubleItem : GeneticDoubleItem
    {

    }

    [Serializable]
    public abstract class GeneticDoubleItem : GeneticItem<double>
    {
        protected GeneticDoubleItem(GeneticItemInitData initData)
            : base(initData)
        {
        }
    }

    public abstract class GeneticDoubleCommand<TGeneticDoubleDataObject, TGeneticDoubleItem>
        : GeneticCommand<TGeneticDoubleDataObject, TGeneticDoubleItem, double>
        where TGeneticDoubleDataObject : GeneticDoubleDataObject<TGeneticDoubleItem>
        where TGeneticDoubleItem : GeneticDoubleItem
    {

    }

}
