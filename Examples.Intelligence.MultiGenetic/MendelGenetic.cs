using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Genetic;
using MasterDataFlow.Intelligence.Interfaces;

namespace Examples.Intelligence.MultiGenetic
{
    [Serializable]
    public class GeneAllele
    {
        public bool IsDominante { get; set; }
        public int Value { get; set; }
    }

    [Serializable]
    public class GenePair
    {
        public GenePair()
        {
            //Alleles = new GeneAllele[2] { nw};
        }

        public GeneAllele[] Alleles { get; set; }

        public int Value { get; set; }

        protected internal void SelectDominancValue(IRandom random)
        {
            var allele1 = Alleles[0];
            var allele2 = Alleles[1];
            if (allele1.IsDominante)
            {
                if (allele2.IsDominante)
                {
                    Value = random.Next(100) < 50 ? allele1.Value : allele2.Value;
                }
                else
                {
                    Value = allele1.Value;
                }
            }
            else
            {
                if (allele2.IsDominante)
                {
                    Value = allele2.Value;
                }
                else
                {
                    Value = random.Next(100) < 50 ? allele1.Value : allele2.Value;
                }
            }
        }

        //public GetDom
    }

    [Serializable]
    public class MendelGeneticDataObject : GeneticDataObject<GenePair>
    {

    }

    [Serializable]
    public abstract class MendelGeneticItem : GeneticItem<GenePair>
    {
        protected MendelGeneticItem(GeneticItemInitData initData)
            : base(initData)
        {
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
                Value = random.Next(InitData.Count)
            };
            return result;
        }
    }

    public abstract class MendelGeneticCommand<TGeneticCellDataObject> : GeneticCommand<TGeneticCellDataObject, MendelGeneticItem, GenePair>
        where TGeneticCellDataObject : MendelGeneticDataObject
    {

        protected override void Mutation(MendelGeneticItem item)
        {
            for (int i = 0; i < item.Values.Length; i++)
            {
                if (Random.NextDouble() > 0.9)
                {
                    var newValue = item.CreateValue(Random);
                    var oldValue = item.Values[i];
                    item.Values[i] = newValue;
                    for (int j = 0; j < item.Values.Length; j++)
                    {
                        if (j != i && item.Values[j].Value == newValue.Value)
                        {
                            item.Values[j] = oldValue;
                            break;
                        }
                    }
                }
            }
        }

        protected override MendelGeneticItem CreateChild(MendelGeneticItem firstParent, MendelGeneticItem secondParent)
        {
            //var result = InternalCreateItem();
            //for (int i = 0; i < firstParent.Values.Length; i++)
            //{
            //    var firstValue = firstParent.Values[i];
            //    var secondValue = secondParent.Values[i];
            //    var allele1 = GetGeneAllele(firstValue);
            //    var allele2 = GetGeneAllele(secondValue);
            //    var newPair = new GenePair {Alleles = new [] {allele1, allele2}};
            //    newPair.SelectDominancValue(Random);
            //    result.Values[i] = newPair;
            //}
            //return result;

            var result = InternalCreateItem();
            var zeroValue = firstParent.Values[0];
            for (int i = 1; i < firstParent.Values.Length; i++)
            {
                var firstValue = firstParent.Values[i];
                var secondValue = secondParent.Values[i-1];
                CreateNewPair(firstValue, secondValue, result, i);
            }
            CreateNewPair(zeroValue, secondParent.Values[secondParent.Values.Length-1], result, 0);
            return result;
        }

        private void CreateNewPair(GenePair firstValue, GenePair secondValue, MendelGeneticItem result, int i)
        {
            var allele1 = GetGeneAllele(firstValue);
            var allele2 = GetGeneAllele(secondValue);
            var newPair = new GenePair {Alleles = new[] {allele1, allele2}};
            newPair.SelectDominancValue(Random);
            result.Values[i] = newPair;
        }

        private GeneAllele GetGeneAllele(GenePair pair)
        {
            var allele1 = pair.Alleles[0];
            var allele2 = pair.Alleles[1];
            return Random.Next(100) < 50 ? allele1 : allele2;
        }
    }

}
