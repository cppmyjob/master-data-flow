//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;
//using RGiesecke.DllExport;

//namespace MasterDataFlow.MetaTrader
//{
//    // https://www.mql5.com/ru/articles/249
//    public static class Functions
//    {
//        [DllExport("Add", CallingConvention = CallingConvention.StdCall)]
//        public static int Add(int left, int right)
//        {
//            return left + right;
//        }

//        [DllExport("Sub", CallingConvention = CallingConvention.StdCall)]
//        public static int Sub(int left, int right)
//        {
//            return left - right;
//        }

//        [DllExport("AddDouble", CallingConvention = CallingConvention.StdCall)]
//        public static double AddDouble(double left, double right)
//        {
//            return left + right;
//        }

//        [DllExport("AddFloat", CallingConvention = CallingConvention.StdCall)]
//        public static float AddFloat(float left, float right)
//        {
//            return left + right;
//        }

//        [DllExport("Get1DInt", CallingConvention = CallingConvention.StdCall)]
//        public static int Get1DInt([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]  int[] tab, int i, int idx)
//        {
//            return tab[idx];
//        }

//        [DllExport("Get1DFloat", CallingConvention = CallingConvention.StdCall)]
//        public static float Get1DFloat([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]  float[] tab, int i, int idx)
//        {
//            return tab[idx];
//        }

//        [DllExport("Get1DDouble", CallingConvention = CallingConvention.StdCall)]
//        public static double Get1DDouble([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]  double[] tab, int i, int idx)
//        {
//            return tab[idx];
//        }

//        [DllExport("SetFiboArray", CallingConvention = CallingConvention.StdCall)]
//        public static int SetFiboArray([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
//            int[] tab, int len, [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] int[] res)
//        {
//            res[0] = 0;
//            res[1] = 1;

//            if (len < 3) return -1;
//            for (int i = 2; i < len; i++)
//                res[i] = res[i - 1] + res[i - 2];
//            return 0;
//        }

//        [DllExport("ReplaceString", CallingConvention = CallingConvention.StdCall)]
//        public static int ReplaceString([In, Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder str,
//            [MarshalAs(UnmanagedType.LPWStr)]string a, [MarshalAs(UnmanagedType.LPWStr)]string b)
//        {
//            str.Replace(a, b);

//            if (str.ToString().Contains(a)) return 1;
//            else return 0;
//        }
//    }
//}
