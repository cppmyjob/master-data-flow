using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Interfaces;

namespace MasterDataFlow.Intelligence.Random
{
    public class RandomFactory : IRandomFactory
    {
        public static IRandomFactory Instance = new RandomFactory();

        public IRandom Create()
        {
            return new Intelligence.Random.Random();
        }
    }
}
