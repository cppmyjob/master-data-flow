using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Intelligence.Genetic
{
    [Serializable]
    public class GeneticDoubleDataObject<TGeneticDoubleItem> : GeneticDataObject<TGeneticDoubleItem, int>
        where TGeneticDoubleItem : GeneticDoubleItem
    {

    }

    [Serializable]
    public abstract class GeneticDoubleItem : GeneticItem<int>
    {
        protected GeneticDoubleItem(GeneticItemInitData initData)
            : base(initData)
        {
        }
    }

    public abstract class GeneticDoubleCommand<TGeneticDoubleDataObject, TGeneticDoubleItem>
        : GeneticCommand<TGeneticDoubleDataObject, TGeneticDoubleItem, int>
        where TGeneticDoubleDataObject : GeneticDoubleDataObject<TGeneticDoubleItem>
        where TGeneticDoubleItem : GeneticDoubleItem
    {

    }

}
