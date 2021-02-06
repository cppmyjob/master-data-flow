using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Intelligence.Interfaces;
using MasterDataFlow.Trading.Tester;

namespace MasterDataFlow.Trading.Genetic
{
    public static class FitnessCalculator
    {
        private class TrainingProgress
        {
            public TesterResult TesterResult { get; set; }
            public int ZizZagCount { get; set; }
            public IFitness Fintess { get; set; }
        }

        public static double Execute(TradingItem item, LearningData[] data, ISimpleNeuron dll)
        {
            bool[] oldValues = new bool[data[0].Indicators.Length];
            for (int i = 0; i < item.InitData.InputData.Indicators.IndicatorNumber; i++)
            {
                var index = (int)item.Values[i];
                if (oldValues[index])
                    return Double.MinValue;
                oldValues[index] = true;
            }

            var progress = new List<TrainingProgress>();
            for (int i = 0; i < data.Length; i++)
            {
                var trainingData = data[i];

                int trainingZigZagCount;
                var trainingResult = GetProfit(dll, item, trainingData, out trainingZigZagCount);

                if (trainingResult.Profit <= 0)
                {
                    return -data.Length + i;
                }

                if (item.InitData.Optimizer.Training.IsFilterBadResult && FilterBadResult(trainingResult))
                {
                    return -data.Length + i;
                }

                if (item.InitData.Optimizer.Training.IsFilterBadResultBuySell && FilterBadResultBuySell(trainingResult))
                {
                    return -data.Length + i;
                }

                progress.Add(new TrainingProgress
                {
                    TesterResult = trainingResult,
                    ZizZagCount = trainingZigZagCount
                });
            }

            //if (DataObject.ItemInitData.Optimizer.IsValidationPlusMinusRatioLessTraining)
            //{
            //    if (validationResult.MinusCount > 0 && trainingResult.MinusCount > 0)
            //    {
            //        if (((float)validationResult.PlusCount / validationResult.MinusCount) <
            //            ((float)trainingResult.PlusCount / trainingResult.MinusCount))
            //        {
            //            return Double.MinValue;
            //        }
            //    }
            //}

            for (int i = 0; i < progress.Count; i++)
            {
                progress[i].Fintess = GetFitness(progress[i].TesterResult, progress[i].ZizZagCount, item.InitData);
            }

            item.FitnessZigZag = progress.Sum(t => t.Fintess.FitnessZigZag) / progress.Count;
            item.FitnessExpectedValue = progress.Sum(t => t.Fintess.FitnessExpectedValue) / progress.Count;
            item.FitnessProfit = progress.Sum(t => t.Fintess.FitnessProfit) / progress.Count; ;
            item.FitnessPlusMinusOrdersRatio = progress.Sum(t => t.Fintess.FitnessPlusMinusOrdersRatio) / progress.Count;
            item.FitnessPlusMinusEquityRatio = progress.Sum(t => t.Fintess.FitnessPlusMinusEquityRatio) / progress.Count; ;

            item.FitnessTradingCount = progress.Sum(t => t.Fintess.FitnessTradingCount) / progress.Count;
            item.FitnessOrderCount = progress.Sum(t => t.Fintess.FitnessOrderCount) / progress.Count;
            item.FitnessProfitEquityDifferent = progress.Sum(t => t.Fintess.FitnessProfitEquityDifferent) / progress.Count;
            item.FitnessMinimumMinusEquity = progress.Sum(t => t.Fintess.FitnessMinimumMinusEquity) / progress.Count;

            var validationPercent = item.InitData.Optimizer.Fitness.ValidationPercent;
            //if (validationPercent > 0)
            //{
            //    var percentMin = 1 - (validationPercent / (double)100);
            //    var percentMax = 1 + (validationPercent / (double)100);

            //    var min = trainingFitness.Fitness * percentMin;
            //    var max = trainingFitness.Fitness * percentMax;
            //    if (!(min <= validationFitness.Fitness && validationFitness.Fitness <= max))
            //    {
            //        return Double.MinValue;
            //    }
            //}
            var std = CalculateStdDev(progress.Select(t => t.Fintess.Fitness));
            var mean = CalculateMean(progress.Select(t => t.Fintess.Fitness));
            item.FitnessOriginal = mean;

            if (validationPercent > 0)
            {
                var maxDeviation = mean * (validationPercent / (double)100);

                if (std > maxDeviation)
                {
                    return -0.5;
                }
            }

            var finalResult = new FinalResult
            {
                TrainingTesterResult = progress.Select(t => t.TesterResult).ToArray(),
                Std = std,
            };

            item.FinalResult = finalResult;

            return mean * NormalizeValue(1 / std);

        }

        private static IFitness GetFitness(TesterResult testerResult, double zigZagCount, TradingItemInitData itemInitData)
        {
            var result = new FitnessData();

            // ExpectedValue
            {
                // https://www.mql5.com/ru/blogs/post/651765
                var sdelki = (double)(testerResult.Orders.Count);
                if (sdelki > 0)
                {
                    var pplus = (testerResult.PlusCount) / sdelki;
                    var vplus = (double)(testerResult.Orders.Where(t => t.Profit >= 0).Sum(t => t.Profit)) / sdelki;
                    var pminus = (testerResult.MinusCount) / sdelki;
                    var vminus = (double)Math.Abs(testerResult.Orders.Where(t => t.Profit < 0).Sum(t => t.Profit)) /
                                 sdelki;
                    var m = pplus * vplus - pminus * vminus;

                    if (m < 0)
                        m = 0.000001;
                    result.FitnessExpectedValue = NormalizeValue(m);
                }
                else
                    result.FitnessExpectedValue = 0;
            }

            // Profit
            {
                result.FitnessProfit = NormalizeValue((double)(testerResult.Profit));
            }

            // ZigZag
            {
                if (zigZagCount < 0)
                    zigZagCount = 1 / Math.Abs(zigZagCount);

                result.FitnessZigZag = NormalizeValue(zigZagCount);
            }

            // PlusMinusOrdersRatio
            {
                var pmRatio = ((double)(testerResult.PlusCount) /
                               ((testerResult.MinusCount) > 0 ? (testerResult.MinusCount) : 1));
                result.FitnessPlusMinusOrdersRatio = NormalizeValue(pmRatio);
            }

            // PlusMinusEquityRatio
            {
                var ecRation = ((double)(testerResult.PlusEquityCount) /
                                ((testerResult.MinusEquityCount) > 0 ? (testerResult.MinusEquityCount) : 1));
                result.FitnessPlusMinusEquityRatio = NormalizeValue(ecRation);
            }

            // TradingCount
            {
                result.FitnessTradingCount = NormalizeValue(testerResult.TradingCount);
            }

            // Orders.Count
            {
                if (testerResult.Orders.Count > 0)
                {
                    result.FitnessOrderCount = NormalizeValue(1 / (double)testerResult.Orders.Count);
                }
                else
                    result.FitnessOrderCount = 0;
            }

            // MinimumMinusEquity
            {
                if (testerResult.MinEquity < 0)
                    result.FitnessMinimumMinusEquity = NormalizeValue(1 / (double)-testerResult.MinEquity);
                else
                    result.FitnessMinimumMinusEquity = 0;
            }

            // ProfitEquityDifferent
            {
                var differents = testerResult.Orders.Where(t => t.MaxEquity > 0).Select(t => (double)(t.MaxEquity - t.Profit)).ToArray();
                if (differents.Length > 1)
                {
                    var stdev = CalculateStdDev(differents);
                    if (stdev <= 0)
                        result.FitnessProfitEquityDifferent = 0;
                    else
                        result.FitnessProfitEquityDifferent = NormalizeValue(1 / stdev);
                }
                else
                {
                    result.FitnessProfitEquityDifferent = 0;
                }
            }

            var fitness = 1.0;


            if (itemInitData.Optimizer.Fitness.IsZigZag)
            {
                fitness *= result.FitnessZigZag;
            }

            if (itemInitData.Optimizer.Fitness.IsExpectedValue)
            {
                fitness *= result.FitnessExpectedValue;
            }

            if (itemInitData.Optimizer.Fitness.IsProfit)
            {
                fitness *= result.FitnessProfit;
            }

            if (itemInitData.Optimizer.Fitness.IsPlusMinusOrdersRatio)
            {
                fitness *= result.FitnessPlusMinusOrdersRatio;
            }

            if (itemInitData.Optimizer.Fitness.IsPlusMinusEquityRatio)
            {
                fitness *= result.FitnessPlusMinusEquityRatio;
            }

            if (itemInitData.Optimizer.Fitness.IsTradingCount)
            {
                fitness *= result.FitnessTradingCount;
            }

            if (itemInitData.Optimizer.Fitness.IsOrderCount)
            {
                fitness *= result.FitnessOrderCount;
            }

            if (itemInitData.Optimizer.Fitness.IsProfitEquityDifferent)
            {
                fitness *= result.FitnessProfitEquityDifferent;
            }

            if (itemInitData.Optimizer.Fitness.IsMinimumMinusEquity)
            {
                fitness *= result.FitnessMinimumMinusEquity;
            }

            result.Fitness = fitness;

            return result;
        }


        private static double NormalizeValue(double value)
        {
            if (value <= 0)
                return 0;
            return Math.Log(value + 1);
        }

        private static bool FilterBadResult(TesterResult testerResult)
        {
            if (testerResult.Profit <= 0)
            {
                return true;
            }
            if (testerResult.PlusCount < testerResult.MinusCount)
            {
                return true;
            }
            if ((float)testerResult.MinusCount / (testerResult.PlusCount + testerResult.MinusCount) > 0.5)
            {
                return true;
            }

            if (testerResult.BuyCount == 0 || testerResult.SellCount == 0)
            {
                return true;
            }

            //if (testerResult.MinusEquityCount > testerResult.PlusEquityCount)
            //{
            //    return true;
            //}

            //if (FilterBadResultBuySell(testerResult))
            //    return true;

            return false;
        }


        public static bool FilterBadResultBuySell(TesterResult testerResult)
        {
            if (testerResult.BuyCount > testerResult.SellCount)
            {
                if (testerResult.BuyCount / (float)testerResult.SellCount > 2)
                {
                    return true;
                }
            }
            else
            {
                if (testerResult.SellCount / (float)testerResult.BuyCount > 2)
                {
                    return true;
                }
            }
            return false;
        }

        private static TesterResult GetProfit(ISimpleNeuron neuron, TradingItem item, LearningData learningData, out int zigZagCount)
        {
            var tester = new DirectionTester(neuron, item, learningData);
            TesterResult result = tester.Run();
            zigZagCount = tester.ZigZagCount;
            return result;
        }

        private static double CalculateStdDev(IEnumerable<double> values)
        {
            double ret = 0;
            var enumerable = values as double[] ?? values.ToArray();
            if (enumerable.Any())
            {
                //Compute the Average      
                double avg = enumerable.Average();
                //Perform the Sum of (value-avg)_2_2      
                double sum = enumerable.Sum(d => Math.Pow(d - avg, 2));
                //Put it all together      
                ret = Math.Sqrt((sum) / (enumerable.Count() - 1));
            }
            return ret;
        }

        private static double CalculateMean(IEnumerable<double> values)
        {
            return values.Average();
        }

    }
}
