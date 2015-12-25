using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Intelligence.Genetic
{
    [Serializable]
    public class GeneticHistoryItem<TGeneticItem, TValue>
        where TGeneticItem : GeneticItem<TValue>
    {
        private readonly TGeneticItem _item;

        public GeneticHistoryItem(TGeneticItem item)
        {
            _item = item;
        }

        public TGeneticItem Item
        {
            get { return _item; }
        }
    }

    [Serializable]
    public class GeneticHistoryMutationItem<TGeneticItem, TValue> : GeneticHistoryItem<TGeneticItem, TValue>
        where TGeneticItem : GeneticItem<TValue>
    {
        public GeneticHistoryMutationItem(TGeneticItem item) : base(item)
        {
        }
    }

    [Serializable]
    public class GeneticHistoryReproductionItem<TGeneticItem, TValue> : GeneticHistoryItem<TGeneticItem, TValue>
        where TGeneticItem : GeneticItem<TValue>
    {
        private readonly TGeneticItem _firstParent;
        private readonly TGeneticItem _secondParent;

        public GeneticHistoryReproductionItem(TGeneticItem item, TGeneticItem firstParent, TGeneticItem secondParent) : base(item)
        {
            _firstParent = firstParent;
            _secondParent = secondParent;
        }

        public TGeneticItem FirstParent
        {
            get { return _firstParent; }
        }

        public TGeneticItem SecondParent
        {
            get { return _secondParent; }
        }
    }

    [Serializable]
    public class GeneticHistory<TGeneticItem, TValue> 
        where TGeneticItem : GeneticItem<TValue>
    {
        private readonly IList<GeneticHistoryItem<TGeneticItem, TValue>> _items;

        public GeneticHistory()
        {
            _items = new List<GeneticHistoryItem<TGeneticItem, TValue>>();
        }

        public IList<GeneticHistoryItem<TGeneticItem, TValue>> Items
        {
            get { return _items; }
        }
    }
}
