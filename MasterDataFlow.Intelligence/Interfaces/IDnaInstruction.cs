using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Neuron.Builder;

namespace MasterDataFlow.Intelligence.Interfaces
{
    public interface IDnaInstruction
    {
        bool Execute(Context context, ReadBuffer buffer, Neuron.Dna.Dna dna);
    }
}
