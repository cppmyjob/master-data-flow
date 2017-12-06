using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Intelligence.Genetic
{
    [Serializable]
    public class GeneticFloatDataObject<TGeneticFloatItem> : GeneticDataObject<TGeneticFloatItem, int>
        where TGeneticFloatItem : GeneticFloatItem
    {

    }

    [Serializable]
    public abstract class GeneticFloatItem : GeneticItem<int>
    {
        protected GeneticFloatItem(GeneticItemInitData initData)
            : base(initData)
        {
        }
    }

    public abstract class GeneticFloatCommand<TGeneticFloatDataObject, TGeneticFloatItem>
        : GeneticCommand<TGeneticFloatDataObject, TGeneticFloatItem, int>
        where TGeneticFloatDataObject : GeneticFloatDataObject<TGeneticFloatItem>
        where TGeneticFloatItem : GeneticFloatItem
    {

    }
}
