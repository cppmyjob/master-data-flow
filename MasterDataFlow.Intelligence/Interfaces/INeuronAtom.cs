using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Intelligence.Interfaces
{
    public interface INeuronAtom<TValue>
    {
        TValue[] NetworkCompute(TValue[] inputs);
    }
}
