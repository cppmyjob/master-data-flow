using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Intelligence.Genetic
{
    [Serializable]
    public class GeneticFloatDataObject<TGeneticItemInitData, TGeneticFloatItem> : GeneticDataObject<TGeneticItemInitData, TGeneticFloatItem, float>
        where TGeneticItemInitData : GeneticItemInitData
        where TGeneticFloatItem : GeneticFloatItem<TGeneticItemInitData>
    {

    }

    [Serializable]
    public abstract class GeneticFloatItem<TGeneticItemInitData> : GeneticItem<TGeneticItemInitData, float>
        where TGeneticItemInitData : GeneticItemInitData
    {
        protected GeneticFloatItem(TGeneticItemInitData initData)
            : base(initData)
        {
        }

        public override float ParseStringValue(string value)
        {
            return float.Parse(value);
        }

    }

    public abstract class GeneticFloatCommand<TGeneticFloatDataObject, TGeneticItemInitData, TGeneticFloatItem>
        : GeneticCommand<TGeneticFloatDataObject, TGeneticItemInitData, TGeneticFloatItem, float>
        where TGeneticItemInitData : GeneticItemInitData
        where TGeneticFloatDataObject : GeneticFloatDataObject<TGeneticItemInitData, TGeneticFloatItem>
        where TGeneticFloatItem : GeneticFloatItem<TGeneticItemInitData>
    {

    }
}
