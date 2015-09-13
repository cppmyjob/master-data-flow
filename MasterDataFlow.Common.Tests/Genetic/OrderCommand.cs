﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Genetic;
using MasterDataFlow.Messages;

namespace MasterDataFlow.Common.Tests.Genetic
{
    [Serializable]
    public class OrderGeneticItem : GeneticItem
    {
        public OrderGeneticItem(GeneticInitData initData) : base(initData)
        {
        }

        protected override double CreateValue(double random)
        {
            return Math.Floor(random * 5);
        }
    }

    [Serializable]
    public class OrderDataObject : GeneticCellDataObject
    {
    }


    public class OrderCommand : GeneticCellCommand
    {
        public static GeneticCellDataObject StaticDataObject;

        protected override BaseMessage BaseExecute()
        {
            StaticDataObject = DataObject;
            //Console.WriteLine("OrderCommand::BaseExecute");
            return base.BaseExecute();
        }

        protected override GeneticItem CreateItem(GeneticInitData initData)
        {
            return new OrderGeneticItem(initData);
        }

        public override double CalculateFitness(GeneticItem item, int processor)
        {
            var result = 1;
            var lastValue = item.Values[0];
            for (int i = 1; i < item.Values.Length; i++)
            {
                if (lastValue < item.Values[i])
                {
                    lastValue = item.Values[i];
                    result += 1;
                }
                else
                    break;
            }

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < item.Values.Length; j++)
                {
                    if (i == item.Values[j])
                    {
                        result += 1;
                        break;
                    }
                }

            }
            return result;
        }
    }
}