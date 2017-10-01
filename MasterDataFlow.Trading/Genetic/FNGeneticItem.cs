using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MasterDataFlow.Intelligence.Genetic;
using MasterDataFlow.Intelligence.Interfaces;
using MasterDataFlow.Trading.Tester;

namespace MasterDataFlow.Trading.Genetic
{
    public class FNGeneticItem : GeneticItem<double>
    {
        public const int OFFSET_AVG1 = 0;
        public const int OFFSET_AVG2 = 1;
        public const int OFFSET_AVG3 = 2;
        public const int OFFSET_ALPHA = 3;
        public const int OFFSET_STOPLOSS = 4;
        public const int OFFSET_NEXT = 5;

        public FNGeneticItem(GeneticItemInitData initData)
            : base(initData)
        {

        }

        private FxTesterResult _testerResult;
        private FxTesterResult _predictionTesterResult;

        private FxTesterResult _oldTesterResult;
        private FxTesterResult _oldPredictionTesterResult;

        public FxTesterResult PredictionTesterResult
        {
            get { return _predictionTesterResult; }
            set { _predictionTesterResult = value; }
        }

        public FxTesterResult TesterResult
        {
            get { return _testerResult; }
            set { _testerResult = value; }
        }

        public override double CreateValue(IRandom random)
        {
            return random.NextDouble();
        }

        protected override void SaveValues()
        {
            base.SaveValues();
            _oldTesterResult = _testerResult;
            _oldPredictionTesterResult = _predictionTesterResult;
        }

        protected override void RestoreValues()
        {
            base.RestoreValues();
            _testerResult = _oldTesterResult;
            _predictionTesterResult = _oldPredictionTesterResult;
        }

        public double Alpha
        {
            get
            {
                return Values[OFFSET_ALPHA] * 5;
            }
        }

        public int StopLoss
        {
            get { return (int)(Values[OFFSET_STOPLOSS] * 100); }
        }

        public int GetPeriod(int index)
        {
            return (int)(Values[index] * (FNGeneticManager.MAX_PERIOD - 1));
            //switch (index)
            //{
            //    case OFFSET_AVG1: return 8 + 1;
            //    case OFFSET_AVG2: return 18 + 1;
            //    case OFFSET_AVG3: return 3 + 1;
            //}
            //return 23;
        }


        //public void SaveToMQL(TextWriter writer)
        //{
        //    writer.WriteLine("CppGo v1.0");

        //    writer.WriteLine("Alpha");
        //    writer.WriteLine(DoubleToString(Alpha));

        //    writer.WriteLine("StopLoss");
        //    writer.WriteLine(IntToString(StopLoss));

        //    writer.WriteLine("Avg");
        //    writer.WriteLine("3");
        //    writer.Write(IntToString(GetPeriod(OFFSET_AVG1) + 1));
        //    writer.Write(", ");
        //    writer.Write(IntToString(GetPeriod(OFFSET_AVG2) + 1));
        //    writer.Write(", ");
        //    writer.Write(IntToString(GetPeriod(OFFSET_AVG3) + 1));
        //    writer.WriteLine("");

        //    writer.WriteLine("AvgBarCount");
        //    writer.WriteLine(FNGeneticManager.BAR_COUNT);

        //    writer.WriteLine("Neurons");
        //    int[] neurons = FNGeneticManager.GetNeuronsConfig();
        //    writer.WriteLine(IntToString(neurons.Length));
        //    String line = "";
        //    for (int i = 0; i < neurons.Length; i++)
        //    {
        //        line += IntToString(neurons[i]);
        //        if (i != neurons.Length - 1)
        //            line += ", ";
        //    }
        //    writer.WriteLine(line);


        //    writer.WriteLine("Weights");
        //    writer.WriteLine(IntToString(Values.Length - OFFSET_NEXT));
        //    for (int i = OFFSET_NEXT; i < Values.Length; i++)
        //    {
        //        writer.WriteLine(DoubleToString(Values[i] * 100.0 - 50.0));
        //    }

        //}

        private static string DoubleToString(double value)
        {
            string result = value.ToString();
            return result.Replace(',', '.');
        }

        private static string IntToString(int value)
        {
            string result = value.ToString();
            return result;
        }

    }
}
