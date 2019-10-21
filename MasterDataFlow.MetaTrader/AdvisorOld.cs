//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;
//using MasterDataFlow.Intelligence.Neuron.SimpleNeuron;
//using MasterDataFlow.Trading.Algorithms;
//using MasterDataFlow.Trading.Data;
//using MasterDataFlow.Trading.Genetic;
//using MasterDataFlow.Trading.IO;
//using MasterDataFlow.Trading.Shared.Data;
//using RGiesecke.DllExport;
//using Trady.Analysis.Indicator;

//namespace MasterDataFlow.MetaTrader
//{
//    public static class AdvisorOld
//    {
//        private static List<InputValues> _inputValues;
//        private static DateTime _currenDateTime;

//        private static string[] _indicatorNames = {
//            "MACD Line", "RSI 3", "MACD SignalLine", "RSI 14", "RSI 7"
//        };

//        [DllExport("OldAdvisorInit", CallingConvention = CallingConvention.StdCall)]
//        public static int AdvisorInit([In, Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder response)
//        {
//            response[0] = 't';
//            response[1] = 'r';
//            response[2] = 'u';
//            response[3] = 'e';
//            Log(Directory.GetCurrentDirectory());

//            try
//            {
//                var masterDataFlowPath = GetDataPath();
//                var filename = Path.Combine(masterDataFlowPath, "AFLT.csv");
//                Log(filename);
//                _inputValues = TradingComparing.LoadTrainingData(filename, true);
//            }
//            catch (Exception ex)
//            {
//                Log(ex.Message + " : " + ex.StackTrace);
//                if (ex.InnerException != null)
//                {
//                    Log(ex.InnerException.Message + " : " + ex.InnerException.StackTrace);
//                    if (ex.InnerException.InnerException != null)
//                    {
//                        Log(ex.InnerException.InnerException.Message + " : " + ex.InnerException.InnerException.StackTrace);
//                    }

//                }

//            }
//            return 0;
//        }


//        [DllExport("OldAdvisorLog", CallingConvention = CallingConvention.StdCall)]
//        public static int AdvisorLog([In, Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder message)
//        {
//            Log(message.ToString());
//            return 0;
//        }

//        [DllExport("OldAdvisorSetCurrentTime", CallingConvention = CallingConvention.StdCall)]
//        public static int AdvisorSetCurrentTime([In, Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder message)
//        {
//            var currenDateTime = DateTime.Parse(message.ToString());
//            _currenDateTime = new DateTime(currenDateTime.Year, currenDateTime.Month, currenDateTime.Day,
//                currenDateTime.Hour, 0,0);
//            Log($"Set CurrentTime:{message} parsed:{_currenDateTime}");
//            return 0;
//        }

//        [DllExport("OldAdvisorTick", CallingConvention = CallingConvention.StdCall)]
//        public static int AdvisorTick([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] double[] inputData, int size)
//        {
//            try
//            {
//                Log($"inputData:{inputData.Length} size:{size}");

//                for (int i = 0; i < _indicatorNames.Length; i++)
//                {
//                    var startIndex = i * NeuronNetwork.HISTORY_WINDOW_LENGTH;

//                    Log(_indicatorNames[i]);
//                    PrintTraningData(_indicatorNames[i]);

//                    if (i == 2)
//                    {
//                        var signalLines = PrintSignalLine();
//                        for (int j = startIndex, k = 0; j < startIndex + NeuronNetwork.HISTORY_WINDOW_LENGTH; j++, k++)
//                        {
//                            inputData[j] = signalLines[k];
//                        }
//                    }
//                    else
//                    {
//                        var indicator = (new InputDataCollection()).GetInputs()
//                            .Single(t => t.Name == _indicatorNames[i]);


//                        var mtInputValues = new InputValues(_indicatorNames[i],
//                            inputData.Skip(startIndex).Take(NeuronNetwork.HISTORY_WINDOW_LENGTH)
//                                .Select(t => new InputValue(_currenDateTime, (float) t)).ToArray());

//                        var scalar = new MinMaxScaler(indicator.GetMin(), indicator.GetMax());
//                        scalar.Transform(mtInputValues);

//                        for (int j = startIndex, k = 0; j < startIndex + NeuronNetwork.HISTORY_WINDOW_LENGTH; j++, k++)
//                        {
//                            inputData[j] = mtInputValues.Values[k].Value;
//                        }

//                    }

//                    var sb = new StringBuilder();
//                    sb.Append("MT: ");
//                    for (int j = startIndex; j < startIndex + NeuronNetwork.HISTORY_WINDOW_LENGTH; j++)
//                    {
//                        if (j > startIndex)
//                            sb.Append(",");
//                        sb.Append(inputData[j].ToString("F10"));
//                    }

//                    Log(sb.ToString());

//                }

//                var result = GetSignal(inputData);
//                Log("Signal : " + result);
//                return (int)result;
//            }
//            catch (Exception ex)
//            {
//                Log(ex.Message + " : " + ex.StackTrace);
//                return 0;
//            }
//        }

//        private static double[] PrintSignalLine()
//        {
//            var inputValue = _inputValues.Single(t => t.Name == "MACD Line");
//            var index = inputValue.Values.FindIndex(t => t.Time == _currenDateTime);
//            if (index < 0)
//            {
//                Log($"No data for {_currenDateTime}");
//                return new double[NeuronNetwork.HISTORY_WINDOW_LENGTH];
//            }

//            var macdFloat = inputValue.Values.Skip(index - 3 * NeuronNetwork.HISTORY_WINDOW_LENGTH)
//                .Take(3*NeuronNetwork.HISTORY_WINDOW_LENGTH).Select(t => t.Value).ToArray();
//            var macd = macdFloat.Select(t => (decimal) t).ToArray();

//            var signal = new GenericMovingAverage(
//                i => macd[i],
//                Smoothing.Ema(9),
//                macd.Length);

//            var sb = new StringBuilder();
//            sb.Append("SL: ");
//            var result = new List<double>();
//            for (int i = 2 * NeuronNetwork.HISTORY_WINDOW_LENGTH; i < macd.Length; i++)
//            {
//                if (i > 2 * NeuronNetwork.HISTORY_WINDOW_LENGTH)
//                    sb.Append(",");
//                var value = signal[i].Value;
//                result.Add((double)value);
//                sb.Append(value.ToString("F10"));

//            }
//            Log(sb.ToString());
//            return result.ToArray();
//        }

//        private static void PrintTraningData(string name)
//        {
//            var inputValue = _inputValues.Single(t => t.Name == name);
//            var index = inputValue.Values.FindIndex(t => t.Time == _currenDateTime);
//            if (index < 0)
//            {
//                Log($"No data for {_currenDateTime}");
//            }
//            else
//            {
//                var sb = new StringBuilder();
//                sb.Append("TD: ");
//                for (int i = index - NeuronNetwork.HISTORY_WINDOW_LENGTH; i < index; i++)
//                {
//                    if (i > index - NeuronNetwork.HISTORY_WINDOW_LENGTH)
//                        sb.Append(",");
//                    sb.Append(inputValue.Values[i].Value.ToString("F10"));
//                }
//                Log(sb.ToString());
//            }
//        }

//        public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
//        {
//            if (items == null) throw new ArgumentNullException("items");
//            if (predicate == null) throw new ArgumentNullException("predicate");

//            int retVal = 0;
//            foreach (var item in items)
//            {
//                if (predicate(item)) return retVal;
//                retVal++;
//            }
//            return -1;
//        }

//        private static void Log(string message)
//        {
//            var date = DateTime.Now.ToString("s");
//            File.AppendAllLines(@"c:\tmp\masterdata.log", new []{ date + " " + message });
//        }

//        private static float[] _previuosOutput = new float[TradingItemInitData.OUTPUT_NUMBER];
//        private static NeuralNetworkAlgorithm _algorithm = null;


//        private static AlgorithmSignal GetSignal(double[] inputData)
//        {
//            if (_algorithm == null)
//            {
//                _algorithm = CreateAlgorithm();
//            }

//            var buffer = new float[NeuronNetwork.HISTORY_WINDOW_LENGTH * Indicators.INDICATOR_NUMBER + (TradingItemInitData.IS_RECURRENT ? TradingItemInitData.OUTPUT_NUMBER : 0)];
//            for (int i = 0; i < inputData.Length; i++)
//            {
//                buffer[i] = (float)inputData[i];
//            }

//            if (TradingItemInitData.IS_RECURRENT)
//            {
//                for (int i = 0; i < TradingItemInitData.OUTPUT_NUMBER; i++)
//                {
//                    buffer[i + NeuronNetwork.HISTORY_WINDOW_LENGTH * Indicators.INDICATOR_NUMBER] = _previuosOutput[i];
//                }
//            }

//            var result = _algorithm.GetSignal(buffer, _previuosOutput);

//            return result;
//        }

//        private static NeuralNetworkAlgorithm CreateAlgorithm()
//        {
//            var masterDataFlowPath = GetDataPath();
//            var savePath = Path.Combine(masterDataFlowPath, "AFLT.save");
//            var reader = new Reader(new InputDataCollection());
//            var initData = reader.ReadItemInitData(savePath);
//            var tradingItem = reader.ReadItem(initData, savePath);

//            var neuron = NeuronNetwork.CreateNeuronDll(initData.NeuronNetwork, tradingItem);

//            return new NeuralNetworkAlgorithm(tradingItem, neuron);
//        }

//        private static string GetDataPath()
//        {
//            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
//            var masterDataFlowPath = Path.Combine(path, "MasterDataFlow");
//            return masterDataFlowPath;
//        }
//    }
//}
