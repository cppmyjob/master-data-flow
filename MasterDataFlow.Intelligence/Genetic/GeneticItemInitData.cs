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

        public GeneticItemInitData(int valuesNumber, bool isAddHistory = false)
        {
            _valuesNumber = valuesNumber;
            _isAddHistory = isAddHistory;
        }

        protected int _valuesNumber;
        private readonly bool _isAddHistory;

        public int ValuesNumber
        {
            get { return _valuesNumber; }
        }

        public bool IsAddHistory
        {
            get { return _isAddHistory; }
        }
    }
}
