using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Intelligence.Genetic
{
    [Serializable]
    public class GeneticFloatDataObject<TGeneticFloatItem> : GeneticDataObject<TGeneticFloatItem, float>
        where TGeneticFloatItem : GeneticFloatItem
    {

    }

    [Serializable]
    public abstract class GeneticFloatItem : GeneticItem<float>
    {
        protected GeneticFloatItem(GeneticItemInitData initData)
            : base(initData)
        {
        }

        public override float ParseStringValue(string value)
        {
            return float.Parse(value);
        }

    }

    public abstract class GeneticFloatCommand<TGeneticFloatDataObject, TGeneticFloatItem>
        : GeneticCommand<TGeneticFloatDataObject, TGeneticFloatItem, float>
        where TGeneticFloatDataObject : GeneticFloatDataObject<TGeneticFloatItem>
        where TGeneticFloatItem : GeneticFloatItem
    {

    }
}
