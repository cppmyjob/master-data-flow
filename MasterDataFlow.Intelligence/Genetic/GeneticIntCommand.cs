using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Intelligence.Genetic
{
    [Serializable]
    public class GeneticIntDataObject<TGeneticIntItem> : GeneticDataObject<GeneticItemInitData, TGeneticIntItem, int>
        where TGeneticIntItem : GeneticIntItem
    {

    }

    [Serializable]
    public abstract class GeneticIntItem : GeneticItem<GeneticItemInitData, int>
    {
        protected GeneticIntItem(GeneticItemInitData initData)
            : base(initData)
        {
        }

        public override int ParseStringValue(string value)
        {
            return int.Parse(value);
        }
    }

    public abstract class GeneticIntCommand<TGeneticIntDataObject, TGeneticIntItem> 
        : GeneticCommand<TGeneticIntDataObject, GeneticItemInitData, TGeneticIntItem, int>
        where TGeneticIntDataObject : GeneticIntDataObject<TGeneticIntItem>
        where TGeneticIntItem : GeneticIntItem
    {

    }
}
