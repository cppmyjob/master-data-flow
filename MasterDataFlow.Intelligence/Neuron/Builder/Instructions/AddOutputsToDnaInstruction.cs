using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Interfaces;

namespace MasterDataFlow.Intelligence.Neuron.Builder.Instructions
{
    public class AddOutputsToDnaInstruction : IDnaInstruction
    {
        public const byte Code = 0x03;

        public bool Execute(Context context, ReadBuffer buffer, Dna.Dna dna)
        {

            return true;
        }
    }
}
