using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Interfaces;
using MasterDataFlow.Intelligence.Random;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Messages;

namespace MasterDataFlow.Intelligence.Genetic
{
    [Serializable]
    public class GeneticInitData
    {
        private readonly int _itemsCount;
        private readonly int _surviveCount;
        private readonly int _valuesCount;
        private readonly bool _isAddFistory;

        public GeneticInitData(int itemsCount, int surviveCount, int valuesCount, bool isAddFistory = false)
        {
            _itemsCount = itemsCount;
            _surviveCount = surviveCount;
            _valuesCount = valuesCount;
            _isAddFistory = isAddFistory;

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
        public bool IsAddFistory { get { return _isAddFistory; } }
    }
    
    [Serializable]
    public class GeneticDataObject<TGeneticItem, TValue> : ICommandDataObject
        where TGeneticItem : GeneticItem<TValue>
    {
        public GeneticInitData CellInitData { get; set; }
        public int RepeatCount { get; set; }
        public IList<TValue[]> InitPopulation { get; set; }

        public virtual void Init(TGeneticItem item, int index)
        {
            Array.Copy(InitPopulation[index], item.Values, InitPopulation[index].Length);
        }
    }

    [Serializable]
    public class GeneticStopDataObject : ICommandDataObject
    {
        public object Best { get; set; }
    }

    public abstract class GeneticCommand<TGeneticCellDataObject, TGeneticItem, TValue> : Command<TGeneticCellDataObject> 
        where TGeneticCellDataObject : GeneticDataObject<TGeneticItem, TValue>
        where TGeneticItem : GeneticItem<TValue>
    {
        protected TGeneticItem[] _itemsArray;
        private int _currentYear = 1;
        protected IRandom Random = RandomFactory.Instance.Create();
        private TGeneticItem _theBest;

        public TGeneticItem TheBest
        {
            get { return _theBest; }
        }

        public override BaseMessage Execute()
        {
            Init();
            if (DataObject.InitPopulation == null)
            {
                CreatingPopulation(0);
            }
            else
            {
                InitPopulation();
                CreatingPopulation(DataObject.InitPopulation.Count);
            }
            for (int i = 0; i < DataObject.RepeatCount; i++)
            {
                Process();
            }
            var result = new GeneticStopDataObject
            {
                Best = _theBest ?? _itemsArray[0]
            };
            return Stop(result);
        }

        private void InitPopulation()
        {
            for (int i = 0; i < DataObject.InitPopulation.Count; i++)
            {
                var item = InternalCreateItem();
                DataObject.Init(item, i);
                _itemsArray[i] = item;
            }
        }

        private void Init()
        {
            _itemsArray = new TGeneticItem[DataObject.CellInitData.ItemsCount];
        }

        private void CreatingPopulation(int from)
        {
            for (int i = from; i < _itemsArray.Length; i++)
            {
                var item = InternalCreateItem();
                FillValues(item);
                _itemsArray[i] = item;
            }
        }

        protected TGeneticItem InternalCreateItem()
        {
            GeneticItemInitData initData = new GeneticItemInitData
            {
                Count = DataObject.CellInitData.ValuesCount,
                IsAddHistory = DataObject.CellInitData.IsAddFistory,
                YearOfBorn = _currentYear
            };
            var child = CreateItem(initData);
            return child;
        }

        protected abstract TGeneticItem CreateItem(GeneticItemInitData initData);

        public abstract double CalculateFitness(TGeneticItem item, int processor);

        protected virtual void FillValues(TGeneticItem item)
        {
            TValue[] values = item.Values;
            for (int j = 0; j < DataObject.CellInitData.ValuesCount; j++)
            {
                var valueValue = item.CreateValue(Random);
                values[j] = valueValue;
            }
            item.InitOtherValues(Random);
        }

        private double Process()
        {
            Random = RandomFactory.Instance.Create();
            Reproduction();
            Selection();
            ++_currentYear;
            return _itemsArray[0].Fitness;
        }

        private void Selection()
        {
            CalculateWithOneProcessor();
            SortFitness();
            GetTheBestFitness();
        }

        protected virtual void GetTheBestFitness()
        {
            if (_theBest == null)
                _theBest = _itemsArray[0];
            else
            {
                if (_itemsArray[0].Fitness > _theBest.Fitness)
                {
                    _theBest = _itemsArray[0];
                }
            }
        }

        protected virtual void SortFitness()
        {
            var list = (from item in _itemsArray
                                      orderby item.Fitness descending
                                      select item).ToList();
            while (list.Count < DataObject.CellInitData.ItemsCount)
            {
                var item = InternalCreateItem();
                FillValues(item);
                list.Add(item);
            }
            _itemsArray = list.ToArray();
        }

        private void CalculateWithOneProcessor()
        {
            for (var i = 0; i < DataObject.CellInitData.ItemsCount; i++)
            {
                var item = _itemsArray[i];
                if (item.Fitness > 0.0)
                    continue;
                item.Fitness = InternalCalculateFitness(item, 0);
            }
        }

        protected virtual double InternalCalculateFitness(TGeneticItem item, int processor)
        {
            return CalculateFitness(item, processor);
        }

        private void Reproduction()
        {
            int surveyI = 0;
            for (int i = 0; i < DataObject.CellInitData.ItemsCount - DataObject.CellInitData.SurviveCount; i++)
            {
                var firstParent = _itemsArray[surveyI];
                //var secondParentIndex = Random.Next(DataObject.CellInitData.SurviveCount);
                var secondParentIndex = Random.Next(DataObject.CellInitData.ItemsCount);
                var secondParent = _itemsArray[secondParentIndex];
                var child = CreateChild(firstParent, secondParent);
                if (child != null)
                {
                    Mutation(child);
                }
                else
                {
                    child = InternalCreateItem();
                    FillValues(child);
                }
                _itemsArray[i + DataObject.CellInitData.SurviveCount] = child;

                ++surveyI;
                if (surveyI >= DataObject.CellInitData.SurviveCount)
                    surveyI = 0;
            }
        }

        protected virtual void Mutation(TGeneticItem item)
        {
            for (int i = 0; i < item.Values.Length; i++)
            {
                if (Random.NextDouble() > 0.999)
                {
                    var valueValue = item.CreateValue(Random);
                    item.Values[i] = valueValue;
                }
            }
        }

        protected virtual TGeneticItem CreateChild(TGeneticItem firstParent, TGeneticItem secondParent)
        {
            var child = InternalCreateItem();
            // Определяем количество частей для обмена
            int partCount = Random.Next(DataObject.CellInitData.ValuesCount) + 1;
            float coeff = (partCount / (float)DataObject.CellInitData.ValuesCount);
            // Признак того, кто будет первым при копировании 1 или 2 предок
            bool isFirst = Random.Next(10) < 5;
            int lastPartIndex = 0;
            TValue[] firstValues = firstParent.Values;
            TValue[] secondValues = secondParent.Values;
            TValue[] childValues = child.Values;
            // Это признак того, что-бы копировать одного предка относительно 
            // другого со смещением
            int secondOffset = 0;
            if (Random.NextDouble() > 0.999)
                //if (_random.NextDouble() > 0.9)
                secondOffset = Random.Next(DataObject.CellInitData.ValuesCount) + 1;
            // 
            for (int i = 0; i < DataObject.CellInitData.ValuesCount; i++)
            {
                TValue value;
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
                            value = child.CreateValue(Random);
                        }
                    }
                }

                // Это мутация значения
                if (Random.NextDouble() > 0.999)
                //                if (_random.NextDouble() > 0.9)
                {
                    var valueValue = child.CreateValue(Random);
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
