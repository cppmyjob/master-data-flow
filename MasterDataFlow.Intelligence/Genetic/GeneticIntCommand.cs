using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Intelligence.Genetic
{
    [Serializable]
    public class GeneticIntDataObject<TGeneticIntItem> : GeneticDataObject<TGeneticIntItem, int>
        where TGeneticIntItem : GeneticIntItem
    {

    }

    [Serializable]
    public abstract class GeneticIntItem : GeneticItem<int>
    {
        protected GeneticIntItem(GeneticItemInitData initData)
            : base(initData)
        {
        }
    }

    public abstract class GeneticIntCommand<TGeneticIntDataObject, TGeneticIntItem> 
        : GeneticCommand<TGeneticIntDataObject, TGeneticIntItem, int>
        where TGeneticIntDataObject : GeneticIntDataObject<TGeneticIntItem>
        where TGeneticIntItem : GeneticIntItem
    {

    }
}
