using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Intelligence.Genetic
{
    [Serializable]
    public class GeneticDoubleDataObject : GeneticDataObject<double>
    {
        
    }

    [Serializable]
    public abstract class GeneticDoubleItem : GeneticItem<double>
    {
        protected GeneticDoubleItem(GeneticItemInitData initData) : base(initData)
        {
        }
    }

    public abstract class GeneticDoubleCommand<TGeneticCellDataObject> : GeneticCommand<TGeneticCellDataObject, GeneticDoubleItem, double>
        where TGeneticCellDataObject : GeneticDoubleDataObject
    {
        
    }

}
