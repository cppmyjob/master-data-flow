using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Intelligence.Interfaces;

namespace MasterDataFlow.Intelligence.Random
{
    public class SimpleRandom : IRandom
    {
        private readonly System.Random _random = new System.Random();

        public double NextDouble()
        {
            lock (_random)
            {
                return _random.NextDouble();
            }
        }

        public int Next(int maxValue)
        {
            lock (_random)
            {
                return _random.Next(maxValue);
            }
        }
    }
}
