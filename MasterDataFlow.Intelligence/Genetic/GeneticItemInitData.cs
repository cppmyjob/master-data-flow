using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Intelligence.Genetic
{
    [Serializable]
    public class GeneticItemInitData
    {

        public GeneticItemInitData()
        {
        }

        public GeneticItemInitData(int count, bool isAddHistory = false)
        {
            _count = count;
            _isAddHistory = isAddHistory;
        }

        private readonly int _count;
        private readonly bool _isAddHistory;

        public int Count
        {
            get { return _count; }
        }

        public bool IsAddHistory
        {
            get { return _isAddHistory; }
        }
    }
}
