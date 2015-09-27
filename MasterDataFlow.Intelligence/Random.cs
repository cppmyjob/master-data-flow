using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MasterDataFlow.Intelligence.Interfaces;

namespace MasterDataFlow.Intelligence
{

    public class Random : IRandom
    {
        private RNGCryptoServiceProvider _random = new RNGCryptoServiceProvider();

        public double NextDouble()
        {
            byte[] b = new byte[4];
            _random.GetBytes(b);
            return (double)BitConverter.ToUInt32(b, 0) / UInt32.MaxValue;
        }

        public int Next(int maxValue)
        {
            return (int)(Math.Round(NextDouble() * (maxValue - 1)));
        }
    }

    //public class Random : IRandom
    //{
    //    private System.Random _random = new System.Random();

    //    public double NextDouble()
    //    {
    //        return _random.NextDouble();
    //    }

    //    public int Next(int maxValue)
    //    {
    //        return _random.Next(maxValue);
    //    }
    //}
}
