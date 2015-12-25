using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Intelligence.Genetic
{
    [Serializable]
    public class GeneticItemInitData
    {
        private int _count;
        private bool _isAddHistory;
        private int _yearOfBorn;

        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        public int YearOfBorn
        {
            get { return _yearOfBorn; }
            set { _yearOfBorn = value; }
        }

        public bool IsAddHistory
        {
            get { return _isAddHistory; }
            set { _isAddHistory = value; }
        }
    }
}
