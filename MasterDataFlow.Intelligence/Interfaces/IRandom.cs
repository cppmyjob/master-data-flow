using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Intelligence.Interfaces
{
    public interface IRandom
    {
        double NextDouble();

        int Next(int maxValue);
    }
}
