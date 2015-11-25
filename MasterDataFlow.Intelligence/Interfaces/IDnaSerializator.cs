using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Neuron.Builder;

namespace MasterDataFlow.Intelligence.Interfaces
{
    public interface IDnaSerializator
    {
        void Serialize(WriteBuffer buffer);
    }
}
