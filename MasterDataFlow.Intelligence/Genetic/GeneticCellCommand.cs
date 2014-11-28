using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Genetic.Old;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Messages;

namespace MasterDataFlow.Intelligence.Genetic
{
    public class GeneticCellInitData
    {
        private readonly int _itemsCount;
        private readonly int _surviveCount;
        private readonly int _valuesCount;

        public GeneticCellInitData(int itemsCount, int surviveCount, int valuesCount)
        {
            _itemsCount = itemsCount;
            _surviveCount = surviveCount;
            _valuesCount = valuesCount;

            if (_surviveCount > _itemsCount)
                throw new Exception("Число выживших > число особей");
            if (_surviveCount < 1)
                throw new Exception("Число выживших не может быть < 1");
            if (_itemsCount < 2)
                throw new Exception("Число особей в популяции не может быть < 2");
        }

        public int ItemsCount { get { return _itemsCount; } }
        public int SurviveCount { get { return _surviveCount; } }
        public int ValuesCount { get { return _valuesCount; } }
    }
    
    public class GeneticCellDataObject : ICommandDataObject
    {
        public GeneticCellInitData CellInitData { get; set; }
    }

    public class GeneticCellCommand : Command<GeneticCellDataObject>
    {
        protected GeneticItem[] _itemsArray;

        public override BaseMessage Execute()
        {
//            Init();
//            CreatingPopulation(0);
            return Stop(null);
        }

        //private void Init()
        //{
        //    _itemsArray = new GeneticItem[DataObject.CellInitData.ItemsCount];
        //}

        //private void CreatingPopulation(int from)
        //{
        //    for (int i = from; i < _itemsArray.Length; i++)
        //    {
        //        GeneticItem item = InternalCreateItem();
        //        FillValues(item);
        //        _itemsArray[i] = item;
        //    }
        //}

        //protected GeneticItem InternalCreateItem()
        //{
        //    GeneticInitData initData = new GeneticInitData();
        //    initData.Count = _valuesCount;
        //    initData.YearOfBorn = _currentYear;
        //    GeneticItem child = CreateItem(initData);
        //    return child;
        //}

        //protected void FillValues(GeneticItem item)
        //{
        //    double[] values = item.Values;
        //    for (int j = 0; j < _valuesCount; j++)
        //    {
        //        double valueValue = item.CreateValue(_random.NextDouble());
        //        values[j] = valueValue;
        //    }
        //    item.InitOtherValues(_random);
        //}
    }
}
