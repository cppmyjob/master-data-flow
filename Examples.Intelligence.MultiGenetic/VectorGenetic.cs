using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Genetic;

namespace Examples.Intelligence.MultiGenetic
{
    [Serializable]
    public class VectorGeneticDataObject : GeneticDataObject<GeneticItemInitData, GeneticItem<GeneticItemInitData, Vector>, Vector>
    {

    }

    public class Vector
    {
        
    }

    public abstract class VectorGeneticItem : GeneticItem<GeneticItemInitData, Vector>
    {
    }

    public abstract class VectorGeneticCommand<TGeneticCellDataObject> :
        GeneticCommand<TGeneticCellDataObject, GeneticItemInitData, VectorGeneticItem, Vector>
        where TGeneticCellDataObject : GeneticDataObject<GeneticItemInitData, VectorGeneticItem, Vector>
    {
    }
}
