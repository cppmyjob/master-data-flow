using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Intelligence.Neuron.SimpleNeuron;
using MasterDataFlow.MetaTrader.Data;
using MasterDataFlow.Trading.Algorithms;
using MasterDataFlow.Trading.Genetic;
using MasterDataFlow.Trading.IO;
using RGiesecke.DllExport;

namespace MasterDataFlow.MetaTrader
{
    public static class Advisor
    {
        [DllExport("AdvisorInit", CallingConvention = CallingConvention.StdCall)]
        public static int AdvisorInit([In, Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder response)
        {
            response[0] = 't';
            response[1] = 'r';
            response[2] = 'u';
            response[3] = 'e';
            Log(Directory.GetCurrentDirectory());

            var masterDataFlowPath = GetDataPath();
            var csv = Path.Combine(masterDataFlowPath, "AFLT.csv");



            return 0;
        }


        [DllExport("AdvisorLog", CallingConvention = CallingConvention.StdCall)]
        public static int AdvisorLog([In, Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder message)
        {
            Log(message.ToString());
            return 0;
        }


        [DllExport("AdvisorTick", CallingConvention = CallingConvention.StdCall)]
        public static int AdvisorTick([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] double[] inputData, int size)
        {
            try
            {
                var sb = new StringBuilder();

                for (int i = 0; i < inputData.Length; i++)
                {
                    if (i > 0)
                        sb.Append(",");
                    sb.Append(inputData[i].ToString("F10"));
                }

                Log(sb.ToString());

                var result = GetSignal(inputData);
                Log("Signal : " + result);
                return (int)result;
            }
            catch (Exception ex)
            {
                Log(ex.Message + " : " + ex.StackTrace);
                return 0;
            }
        }

        private static void Log(string message)
        {
            var date = DateTime.Now.ToString("s");
            File.AppendAllLines(@"c:\tmp\masterdata.log", new []{ date + " " + message });
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
    }
}
