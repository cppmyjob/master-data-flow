using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using MasterDataFlow.Intelligence.Interfaces;
using MasterDataFlow.Intelligence.Random;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Keys;
using MasterDataFlow.Messages;

namespace MasterDataFlow.Intelligence.Genetic
{
    [Serializable]
    public class GeneticEndCycleMessage : CommandMessage
    {
        private readonly GeneticInfoDataObject _data;

        public GeneticEndCycleMessage(CommandKey key, GeneticInfoDataObject data) : base(key)
        {
            _data = data;
        }

        public GeneticInfoDataObject Data
        {
            get { return _data; }
        }
    }


    [Serializable]
    public class GeneticCommandInitData
    {
        private readonly int _itemsCount;
        private readonly int _surviveCount;
        private readonly int _repeatCount;

        public GeneticCommandInitData(int itemsCount, int surviveCount, int repeatCount)
        {
            _itemsCount = itemsCount;
            _surviveCount = surviveCount;
            _repeatCount = repeatCount;

            if (_surviveCount > _itemsCount)
                throw new Exception("Число выживших > число особей");
            if (_surviveCount < 1)
                throw new Exception("Число выживших не может быть < 1");
            if (_itemsCount < 2)
                throw new Exception("Число особей в популяции не может быть < 2");
        }

        public int ItemsCount { get { return _itemsCount; } }
        public int SurviveCount { get { return _surviveCount; } }
        public int RepeatCount { get { return _repeatCount; } }

    }

    [Serializable]
    public class GeneticDataObject<TGeneticItemInitData, TGeneticItem, TValue> : ICommandDataObject
        where TGeneticItemInitData : GeneticItemInitData
        where TGeneticItem : GeneticItem<TGeneticItemInitData, TValue>
    {
        public GeneticCommandInitData CommandInitData { get; set; }
        public TGeneticItemInitData ItemInitData { get; set; }

        public IList<TValue[]> InitPopulation { get; set; }

        public virtual void Init(TGeneticItem item, int index)
        {
            Array.Copy(InitPopulation[index], item.Values, InitPopulation[index].Length);
        }
    }

    [Serializable]
    public class GeneticInfoDataObject : ICommandDataObject
    {
        public object Best { get; set; }
    }

    public abstract class GeneticCommand<TGeneticDataObject, TGeneticItemInitData, TGeneticItem, TValue> : Command<TGeneticDataObject>
        where TGeneticItemInitData : GeneticItemInitData
        where TGeneticDataObject : GeneticDataObject<TGeneticItemInitData, TGeneticItem, TValue>
        where TGeneticItem : GeneticItem<TGeneticItemInitData, TValue>
    {
        protected TGeneticItem[] _itemsArray;
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

            if (DataObject.CommandInitData.RepeatCount == 0)
            {
                while (true)
                {
                    Process();

                    var info = new GeneticInfoDataObject
                                 {
                                     Best = _theBest ?? _itemsArray[0]
                                 };
                    SendMessage(CreatorWorkflowKey, new GeneticEndCycleMessage(Key, info));
                }
            }
            else
            {
                for (int i = 0; i < DataObject.CommandInitData.RepeatCount; i++)
                {
                    Process();
                    var info = new GeneticInfoDataObject
                               {
                                   Best = _theBest ?? _itemsArray[0]
                               };
                    SendMessage(CreatorWorkflowKey, new GeneticEndCycleMessage(Key, info));
                }
            }
            var result = new GeneticInfoDataObject
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
            _itemsArray = new TGeneticItem[DataObject.CommandInitData.ItemsCount];
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
            var child = CreateItem(DataObject.ItemInitData);
            return child;
        }

        protected abstract TGeneticItem CreateItem(TGeneticItemInitData initData);

        public abstract double CalculateFitness(TGeneticItem item, int processor);

        protected virtual void FillValues(TGeneticItem item)
        {
            TValue[] values = item.Values;
            for (int j = 0; j < DataObject.ItemInitData.ValuesNumber; j++)
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
            while (list.Count < DataObject.CommandInitData.ItemsCount)
            {
                var item = InternalCreateItem();
                FillValues(item);
                list.Add(item);
            }
            _itemsArray = list.ToArray();
        }

        private void CalculateWithOneProcessor()
        {
            //for (var i = 0; i < DataObject.CommandInitData.ItemsCount; i++)
            //{
            //    var item = _itemsArray[i];
            //    if (item.Fitness > 0.0)
            //        continue;
            //    item.Fitness = InternalCalculateFitness(item, 0);
            //}

            var o = new ParallelOptions();
            //o.MaxDegreeOfParallelism = 2;
            Parallel.For(0, DataObject.CommandInitData.ItemsCount, (i) => {
                var item = _itemsArray[i];
                if (item.Fitness < 0.0 || item.Fitness > 0.0)
                    return;
                item.Fitness = InternalCalculateFitness(item, 0);
            });
        }

        protected virtual double InternalCalculateFitness(TGeneticItem item, int processor)
        {
            return CalculateFitness(item, processor);
        }

        private void Reproduction()
        {
            var gSurveyI = -1;
            Parallel.For(0, DataObject.CommandInitData.ItemsCount - DataObject.CommandInitData.SurviveCount,
                (i) =>
                {
                    var surveyI = Interlocked.Increment(ref gSurveyI) % DataObject.CommandInitData.SurviveCount;
                    
                    var firstParent = _itemsArray[surveyI];
                    //var secondParentIndex = Random.Next(DataObject.CellInitData.SurviveCount);
                    var secondParentIndex = Random.Next(DataObject.CommandInitData.ItemsCount);
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
                    _itemsArray[i + DataObject.CommandInitData.SurviveCount] = child;


                });

            //int surveyI = 0;
            //for (int i = 0; i < DataObject.CommandInitData.ItemsCount - DataObject.CommandInitData.SurviveCount; i++)
            //{
            //    var firstParent = _itemsArray[surveyI];
            //    //var secondParentIndex = Random.Next(DataObject.CellInitData.SurviveCount);
            //    var secondParentIndex = Random.Next(DataObject.CommandInitData.ItemsCount);
            //    var secondParent = _itemsArray[secondParentIndex];
            //    var child = CreateChild(firstParent, secondParent);
            //    if (child != null)
            //    {
            //        Mutation(child);
            //    }
            //    else
            //    {
            //        child = InternalCreateItem();
            //        FillValues(child);
            //    }
            //    _itemsArray[i + DataObject.CommandInitData.SurviveCount] = child;

            //    ++surveyI;
            //    if (surveyI >= DataObject.CommandInitData.SurviveCount)
            //        surveyI = 0;
            //}
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
            int partCount = Random.Next(DataObject.ItemInitData.ValuesNumber) + 1;
            float coeff = (partCount / (float)DataObject.ItemInitData.ValuesNumber);
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
                secondOffset = Random.Next(DataObject.ItemInitData.ValuesNumber) + 1;
            // 
            for (int i = 0; i < DataObject.ItemInitData.ValuesNumber; i++)
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
                        if (secondOffset + i < DataObject.ItemInitData.ValuesNumber)
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
