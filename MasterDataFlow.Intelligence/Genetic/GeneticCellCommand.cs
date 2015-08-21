using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Interfaces;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Messages;

namespace MasterDataFlow.Intelligence.Genetic
{
    [Serializable]
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
    
    [Serializable]
    public class GeneticCellDataObject : ICommandDataObject
    {
        public GeneticCellInitData CellInitData { get; set; }
        public int RepeatCount { get; set; }
    }

    [Serializable]
    public class GeneticStopDataObject : ICommandDataObject
    {
        public object Best { get; set; }
    }

    public class GeneticRandomFactory : IRandomFactory
    {
        public static IRandomFactory Instance = new GeneticRandomFactory();

        public IRandom Create()
        {
            return new Random();
        }
    }

    public abstract class GeneticCellCommand : Command<GeneticCellDataObject>
    {
        protected GeneticItem[] _itemsArray;
        private int _currentYear = 1;
        //protected Random _random = new Random((int)DateTime.Now.Ticks);
        protected IRandom Random = GeneticRandomFactory.Instance.Create();

        public override BaseMessage Execute()
        {
            Init();
            CreatingPopulation(0);
            for (int i = 0; i < DataObject.RepeatCount; i++)
            {
                Process();
            }
            var result = new GeneticStopDataObject
            {
                Best = _itemsArray[0]
            };
            return Stop(result);
        }

        private void Init()
        {
            _itemsArray = new GeneticItem[DataObject.CellInitData.ItemsCount];
        }

        private void CreatingPopulation(int from)
        {
            for (int i = from; i < _itemsArray.Length; i++)
            {
                GeneticItem item = InternalCreateItem();
                FillValues(item);
                _itemsArray[i] = item;
            }
        }

        protected GeneticItem InternalCreateItem()
        {
            GeneticInitData initData = new GeneticInitData();
            initData.Count = DataObject.CellInitData.ValuesCount;
            initData.YearOfBorn = _currentYear;
            GeneticItem child = CreateItem(initData);
            return child;
        }

        protected abstract GeneticItem CreateItem(GeneticInitData initData);

        public abstract double CalculateFitness(GeneticItem item, int processor);

        protected void FillValues(GeneticItem item)
        {
            double[] values = item.Values;
            for (int j = 0; j < DataObject.CellInitData.ValuesCount; j++)
            {
                double valueValue = item.CreateValue(Random.NextDouble());
                values[j] = valueValue;
            }
            item.InitOtherValues(Random);
        }

        private double Process()
        {
            Random = GeneticRandomFactory.Instance.Create();
            Reproduction();
            Selection();
            ++_currentYear;
            return _itemsArray[0].Fitness;
        }

        private void Selection()
        {
            CalculateWithOneProcessor();
            SortFitness();
        }

        protected virtual void SortFitness()
        {
            List<GeneticItem> list = (from item in _itemsArray
                                      orderby item.Fitness descending
                                      select item).ToList();
            while (list.Count < DataObject.CellInitData.ItemsCount)
            {
                GeneticItem item = InternalCreateItem();
                FillValues(item);
                list.Add(item);
            }

            _itemsArray = list.ToArray();
        }

        private void CalculateWithOneProcessor()
        {
            for (int i = 0; i < DataObject.CellInitData.ItemsCount; i++)
            {
                GeneticItem item = _itemsArray[i];
                if (item.Fitness == 0.0)
                    item.Fitness = InternalCalculateFitness(item, 0);
            }
        }

        protected virtual double InternalCalculateFitness(GeneticItem item, int processor)
        {
            return CalculateFitness(item, processor);
        }

        private void Reproduction()
        {
            for (int i = 0; i < DataObject.CellInitData.ItemsCount - DataObject.CellInitData.SurviveCount; i++)
            {
                GeneticItem firstParent = _itemsArray[i + DataObject.CellInitData.SurviveCount];
                if (firstParent.Fitness == 0)
                    continue;
                int secondParentIndex;
                //                do
                //                {
                secondParentIndex = Random.Next(DataObject.CellInitData.SurviveCount);
                //               ` } while (secondParentIndex == i);
                GeneticItem secondParent = _itemsArray[secondParentIndex];
                if (secondParent.Fitness == 0)
                    continue;
                GeneticItem child = CreateChild(firstParent, secondParent);
                _itemsArray[i + DataObject.CellInitData.SurviveCount] = child;
            }
        }

        protected virtual GeneticItem CreateChild(GeneticItem firstParent, GeneticItem secondParent)
        {
            GeneticItem child = InternalCreateItem();
            // Определяем количество частей для обмена
            int partCount = Random.Next(DataObject.CellInitData.ValuesCount) + 1;
            float coeff = (partCount / (float)DataObject.CellInitData.ValuesCount);
            // Признак того, кто будет первым при копировании 1 или 2 предок
            bool isFirst = Random.Next(10) < 5;
            int lastPartIndex = 0;
            double[] firstValues = firstParent.Values;
            double[] secondValues = secondParent.Values;
            double[] childValues = child.Values;
            // Это признак того, что-бы копировать одного предка относительно 
            // другого со смещением
            int secondOffset = 0;
            if (Random.NextDouble() > 0.999)
                //if (_random.NextDouble() > 0.9)
                secondOffset = Random.Next(DataObject.CellInitData.ValuesCount) + 1;
            // 
            for (int i = 0; i < DataObject.CellInitData.ValuesCount; i++)
            {
                double value;
                if (secondOffset == 0)
                    value = isFirst ? firstValues[i] : secondValues[i];
                else
                {
                    if (isFirst)
                    {
                        value = firstValues[i];
                    }
                    else
                    {
                        if (secondOffset + i < DataObject.CellInitData.ValuesCount)
                            value = secondValues[secondOffset + i];
                        else
                        {
                            value = child.CreateValue(Random.NextDouble());
                        }
                    }
                }

                // Это мутация значения
                if (Random.NextDouble() > 0.999)
                //                if (_random.NextDouble() > 0.9)
                {
                    double valueValue = child.CreateValue(Random.NextDouble());
                    value = valueValue;
                }
                childValues[i] = value;
                int partIndex = (int)(coeff * i);
                if (partIndex != lastPartIndex)
                {
                    isFirst = !isFirst;
                    lastPartIndex = partIndex;
                }
            }
            return child;
        }

    }
}
