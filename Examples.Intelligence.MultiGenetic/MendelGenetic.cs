﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Genetic;
using MasterDataFlow.Intelligence.Interfaces;

namespace Examples.Intelligence.MultiGenetic
{
    [Serializable]
    public class GeneAllele : IValueClone<GeneAllele>
    {
        public bool IsDominante { get; set; }
        public int Value { get; set; }
        public int Mutation { get; set; }

        public GeneAllele Clone()
        {
            var result = new GeneAllele
            {
                Value = Value,
                Mutation = Mutation,
                IsDominante = IsDominante
            };
            return result;
        }
    }

    [Serializable]
    public class GenePair : IValueClone<GenePair>
    {
        public GenePair()
        {
            //Alleles = new GeneAllele[2] { nw};
        }

        public GeneAllele[] Alleles { get; set; }

        public int DomimantIndex { get; internal set; }

        public int Value
        {
            get { return Alleles[DomimantIndex].Value; }
            internal set { Alleles[DomimantIndex].Value = value; }
        }
        public int Mutation { get { return Alleles[DomimantIndex].Mutation; } }

        protected internal void SelectDominancValue(IRandom random)
        {
            var allele1 = Alleles[0];
            var allele2 = Alleles[1];
            if (allele1.IsDominante)
            {
                if (allele2.IsDominante)
                {
                    DomimantIndex = random.Next(100) < 50 ? 0 : 1;
                }
                else
                {
                    DomimantIndex = 0;
                }
            }
            else
            {
                if (allele2.IsDominante)
                {
                    DomimantIndex = 1;
                }
                else
                {
                    DomimantIndex = random.Next(100) < 50 ? 0 : 1;
                }
            }
        }

        public GenePair Clone()
        {
            var result = new GenePair
            {
                Alleles = new GeneAllele[Alleles.Length]
            };
            for (var i = 0; i < Alleles.Length; i++)
            {
                result.Alleles[i] = Alleles[i].Clone();
            }
            return result;
        }
    }

    [Serializable]
    public class MendelGeneticDataObject<TGeneticItem, TValue> : GeneticDataObject<GeneticItemInitData, GeneticItem<GeneticItemInitData, TValue>, TValue>
        where TGeneticItem : GeneticItem<GeneticItemInitData, TValue>
        where TValue : GenePair
    {

    }

    [Serializable]
    public abstract class MendelGeneticItem : GeneticItem<GeneticItemInitData, GenePair>
    {
        public int Age { get; set; }
        public int Sex { get; set; }

        protected MendelGeneticItem() : base()
        {

        }

        protected MendelGeneticItem(GeneticItemInitData initData, IRandom random)
            : base(initData)
        {
            Age = 100;//random.Next(100);
            Sex = random.Next(100) > 50 ? 0 : 1;
        }

        public override GenePair CreateValue(IRandom random)
        {
            var result = new GenePair();
            var allele1 = CreateAllele(random);
            var allele2 = CreateAllele(random);
            result.Alleles = new[] {allele1, allele2};
            result.SelectDominancValue(random);
            return result;
        }

        private GeneAllele CreateAllele(IRandom random)
        {
            var result = new GeneAllele
            {
                IsDominante = random.Next(100) > 50,
                Value = random.Next(InitData.ValuesNumber),
                Mutation = random.Next(InitData.ValuesNumber * 2) - InitData.ValuesNumber
            };
            return result;
        }

        public override GeneticItem<GeneticItemInitData, GenePair> Clone()
        {
            var result = (MendelGeneticItem)base.Clone();
            result.Age = Age;
            result.Sex = Sex;
            return result;
        }
    }

    public abstract class MendelGeneticCommand<TGeneticCellDataObject> 
        : GeneticCommand<TGeneticCellDataObject, GeneticItemInitData, MendelGeneticItem, GenePair>
        where TGeneticCellDataObject : GeneticDataObject<GeneticItemInitData, MendelGeneticItem, GenePair>
    {

        protected override void SortFitness()
        {
            List<MendelGeneticItem> list = (from item in _itemsArray
                                                 //where item.Age > 0 && item.Fitness > 0
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

        protected override void Mutation(MendelGeneticItem item)
        {
            if (Random.NextDouble() > 0.9)
            {
                if (item.History != null)
                {
                    var history = new GeneticHistoryMutationItem<GeneticItem<GeneticItemInitData, GenePair>, GenePair, GeneticItemInitData>(item.Clone());
                    item.History.Items.Add(history);
                }

                for (var i = item.Values.Length; i > 0; i--)
                {
                    var j = Random.Next(i);
                    var k = item.Values[j];
                    item.Values[j] = item.Values[i - 1];
                    item.Values[i - 1] = k;
                }
            }

            for (int i = 0; i < item.Values.Length; i++)
            {
                if (Random.NextDouble() > 0.9)
                {
                    //var newValue = item.CreateValue(Random);
                    ////var oldValue = item.Values[i];
                    //item.Values[i] = newValue;
                    if (item.History != null)
                    {
                        var history = new GeneticHistoryMutationItem<GeneticItem<GeneticItemInitData, GenePair>, GenePair, GeneticItemInitData>(item.Clone());
                        item.History.Items.Add(history);
                    }

                    var newValue = item.Values[i].Value + item.Values[i].Mutation;
                    if (newValue >= DataObject.ItemInitData.ValuesNumber)
                    {
                        item.Values[i].Value = DataObject.ItemInitData.ValuesNumber - 1;
                    }
                    else
                    {
                        if (newValue < 0)
                            item.Values[i].Value = 0;
                        else
                            item.Values[i].Value = newValue;
                    }

                }
            }
        }

        protected override MendelGeneticItem CreateChild(MendelGeneticItem firstParent, MendelGeneticItem secondParent)
        {
            if (firstParent.Sex == secondParent.Sex)
                return null;

            var result = InternalCreateItem();
            --firstParent.Age;
            --secondParent.Age;
            for (int i = 0; i < firstParent.Values.Length; i++)
            {
                var allele1 = GetGeneAllele(firstParent.Values[i]);
                var allele2 = GetGeneAllele(secondParent.Values[i]);
                var newPair = new GenePair { Alleles = new[] { allele1, allele2 } };
                newPair.SelectDominancValue(Random);
                result.Values[i] = newPair;
            }
            if (result.History != null)
            {
                var history = new GeneticHistoryReproductionItem<GeneticItem<GeneticItemInitData, GenePair>, GenePair, GeneticItemInitData>(result.Clone(), firstParent.Clone(), secondParent.Clone());
                result.History.Items.Add(history);
            }
            return result;
        }

        private GeneAllele GetGeneAllele(GenePair pair)
        {
            return Random.Next(100) < 50 ? pair.Alleles[0].Clone() : pair.Alleles[1].Clone();
        }
    }

}
