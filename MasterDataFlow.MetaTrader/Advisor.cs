using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Algorithms;
using MasterDataFlow.Trading.Data;
using MasterDataFlow.Trading.Genetic;
using MasterDataFlow.Trading.IO;
using MasterDataFlow.Trading.Shared.Data;
using RGiesecke.DllExport;

namespace MasterDataFlow.MetaTrader
{
    public static class Advisor
    {
        private static List<InputValues> _inputValues;
        private static DateTime _currenDateTime;
        private static int _tradeMode;

        private static string[] _indicatorNames = {
                                                      "MACD Line", "RSI 3", "MACD SignalLine", "RSI 14", "RSI 7"
                                                  };

        [DllExport("AdvisorInit", CallingConvention = CallingConvention.StdCall)]
        public static int AdvisorInit(int tradeMode)
        {
            _tradeMode = tradeMode;
            Log($"Trace Mode: {tradeMode}");
            Log($"Current Directory : {Directory.GetCurrentDirectory()}");

            if (_tradeMode == 1)
            {
                try
                {
                    var masterDataFlowPath = GetDataPath();
                    var filename = Path.Combine(masterDataFlowPath, "AFLT.csv");
                    Log(filename);
                    _inputValues = TradingComparing.LoadTrainingData(filename, true);
                }
                catch (Exception ex)
                {
                    Log(ex.Message + " : " + ex.StackTrace);
                    if (ex.InnerException != null)
                    {
                        Log(ex.InnerException.Message + " : " + ex.InnerException.StackTrace);
                    }
                }
            }
            return 0;
        }

        [DllExport("AdvisorLog", CallingConvention = CallingConvention.StdCall)]
        public static int AdvisorLog([In, Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder message)
        {
            Log(message.ToString());
            return 0;
        }

        [DllExport("AdvisorSetCurrentTime", CallingConvention = CallingConvention.StdCall)]
        public static int AdvisorSetCurrentTime([In, Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder message)
        {
            var currenDateTime = DateTime.Parse(message.ToString());
            _currenDateTime = new DateTime(currenDateTime.Year, currenDateTime.Month, currenDateTime.Day,
                currenDateTime.Hour, 0, 0);
            Log($"Set CurrentTime:{message} parsed:{_currenDateTime}");
            return 0;
        }

        [DllExport("AdvisorTick", CallingConvention = CallingConvention.StdCall)]
        public static int AdvisorTick([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
            double[] inputData, int size)
        {
            try
            {
                Log($"inputData:{inputData.Length} size:{size}");

                var bars = inputData.Select(t => new Bar() {
                        Time = _currenDateTime,
                        Close = (decimal)t,
                        High = (decimal)t,
                        Low = (decimal)t,
                        Open = (decimal)t,
                        Volume = (decimal)t,
                    }).ToArray();

                var signalData = new List<double>();

                for (int i = 0; i < _indicatorNames.Length; i++)
                {
                    Log(_indicatorNames[i]);
                    if (_tradeMode == 1)
                        PrintTraningData(_indicatorNames[i]);

                    var convertedData = ConversInputData(_indicatorNames[i], bars);
                    var data = new double[NeuronNetwork.HISTORY_WINDOW_LENGTH];
                    Array.Copy(convertedData, convertedData.Length - NeuronNetwork.HISTORY_WINDOW_LENGTH, data, 0, data.Length);
                    signalData.AddRange(data);
                    Log("MT: " + string.Join(",", data.Select(t => t.ToString("F10"))));
                }

                var result = GetSignal(signalData.ToArray());
                Log("Signal : " + result);
                return (int)result;

            }
            catch (Exception ex)
            {
                Log(ex.Message + " : " + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    Log(ex.InnerException.Message + " : " + ex.InnerException.StackTrace);
                }
                return 0;
            }
        }

        private static void PrintTraningData(string name)
        {
            var inputValue = _inputValues.Single(t => t.Name == name);
            var index = inputValue.Values.FindIndex(t => t.Time == _currenDateTime);
            if (index < 0)
            {
                Log($"No data for {_currenDateTime}");
            }
            else
            {
                var sb = new StringBuilder();
                sb.Append("TD: ");
                for (int i = index - NeuronNetwork.HISTORY_WINDOW_LENGTH; i < index; i++)
                {
                    if (i > index - NeuronNetwork.HISTORY_WINDOW_LENGTH)
                        sb.Append(",");
                    sb.Append(inputValue.Values[i].Value.ToString("F10"));
                }
                Log(sb.ToString());
            }
        }


        private static double[] ConversInputData(string indicatorName, Bar[] bars)
        {
            var indicator = (new InputDataCollection()).GetInputs()
                .Single(t => t.Name == indicatorName);

            var values = indicator.GetValues(bars);

            var scalar = new MinMaxScaler(indicator.GetMin(), indicator.GetMax());
            scalar.Transform(values);
            return values.Values.Select(t => (double) t.Value).ToArray();
        }

        private static void Log(string message)
        {
            var date = DateTime.Now.ToString("s");
            File.AppendAllLines(@"c:\tmp\masterdata.log", new[] { date + " " + message });
        }

        private static float[] _previuosOutput = new float[TradingItemInitData.OUTPUT_NUMBER];
        private static NeuralNetworkAlgorithm _algorithm = null;


        private static AlgorithmSignal GetSignal(double[] inputData)
        {
            if (_algorithm == null)
            {
                _algorithm = CreateAlgorithm();
            }

            var buffer = new float[NeuronNetwork.HISTORY_WINDOW_LENGTH * Indicators.INDICATOR_NUMBER + (TradingItemInitData.IS_RECURRENT ? TradingItemInitData.OUTPUT_NUMBER : 0)];
            for (int i = 0; i < inputData.Length; i++)
            {
                buffer[i] = (float)inputData[i];
            }

            if (TradingItemInitData.IS_RECURRENT)
            {
                for (int i = 0; i < TradingItemInitData.OUTPUT_NUMBER; i++)
                {
                    buffer[i + NeuronNetwork.HISTORY_WINDOW_LENGTH * Indicators.INDICATOR_NUMBER] = _previuosOutput[i];
                }
            }

            var result = _algorithm.GetSignal(buffer, _previuosOutput);

            return result;
        }

        private static NeuralNetworkAlgorithm CreateAlgorithm()
        {
            var masterDataFlowPath = GetDataPath();
            var savePath = Path.Combine(masterDataFlowPath, "AFLT.save");
            var reader = new Reader(new InputDataCollection());
            var initData = reader.ReadItemInitData(savePath);
            var tradingItem = reader.ReadItem(initData, savePath);

            var neuron = NeuronNetwork.CreateNeuronDll(initData.NeuronNetwork, tradingItem);

            return new NeuralNetworkAlgorithm(tradingItem, neuron);
        }

        private static string GetDataPath()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var masterDataFlowPath = Path.Combine(path, "MasterDataFlow");
            return masterDataFlowPath;
        }

        public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (predicate == null) throw new ArgumentNullException("predicate");

            int retVal = 0;
            foreach (var item in items)
            {
                if (predicate(item)) return retVal;
                retVal++;
            }
            return -1;
        }
    }
}
