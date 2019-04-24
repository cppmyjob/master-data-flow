using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Advisors;
using MasterDataFlow.Trading.Algorithms;

namespace MasterDataFlow.Trading.Interfaces
{
    public interface ITradingAlgorithm
    {
        AlgorithmSignal GetSignal(float[] inputs, float[] outRecurrent);
    }
}
