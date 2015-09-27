using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Intelligence.Genetic
{
    [Serializable]
    public class GeneticIntDataObject : GeneticDataObject<int>
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

    public abstract class GeneticIntCommand<TGeneticCellDataObject> : GeneticCommand<TGeneticCellDataObject, GeneticIntItem, int>
        where TGeneticCellDataObject : GeneticIntDataObject
    {

    }
}
