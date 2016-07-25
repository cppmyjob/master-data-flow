using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Genetic;

namespace Examples.Intelligence.MultiGenetic
{
    [Serializable]
    public class VectorGeneticDataObject : GeneticDataObject<GeneticItem<Vector>, Vector>
    {

    }

    public class Vector
    {
        
    }

    public abstract class VectorGeneticItem : GeneticItem<Vector>
    {
    }

    public abstract class VectorGeneticCommand<TGeneticCellDataObject> :
        GeneticCommand<TGeneticCellDataObject, VectorGeneticItem, Vector>
        where TGeneticCellDataObject : GeneticDataObject<VectorGeneticItem, Vector>
    {
    }
}
