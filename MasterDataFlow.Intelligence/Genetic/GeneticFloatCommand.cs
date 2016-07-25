using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Intelligence.Genetic
{
    [Serializable]
    public class GeneticFloatDataObject : GeneticDataObject<GeneticFloatItem, float>
    {
        
    }

    [Serializable]
    public abstract class GeneticFloatItem : GeneticItem<float>
    {
        protected GeneticFloatItem(GeneticItemInitData initData) : base(initData)
        {
        }
    }

    public abstract class GeneticFloatCommand<TGeneticCellDataObject> : GeneticCommand<TGeneticCellDataObject, GeneticFloatItem, float>
        where TGeneticCellDataObject : GeneticFloatDataObject
    {
        
    }

}
