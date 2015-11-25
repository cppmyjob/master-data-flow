using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Intelligence.Interfaces;
using MasterDataFlow.Intelligence.Neuron.Builder.Instructions;

namespace MasterDataFlow.Intelligence.Neuron.Builder
{
    public class DnaBuilder
    {
        private readonly Context _context;
        private readonly ReadBuffer _buffer;

        private static readonly IDnaInstruction[] Instructions = new IDnaInstruction[System.Byte.MaxValue + 1];

        static DnaBuilder()
        {
            Instructions[AddSectionsInstruction.Code] = new AddSectionsInstruction();
            Instructions[AddInputsToDnaInstruction.Code] = new AddInputsToDnaInstruction();
            Instructions[AddOutputsToDnaInstruction.Code] = new AddOutputsToDnaInstruction();
            Instructions[AddInputsToSectionInstruction.Code] = new AddInputsToSectionInstruction();
        }

        public DnaBuilder(Context context, byte[] data)
        {
            _context = context;
            _buffer = new ReadBuffer(data);
        }

        public Dna.Dna Build()
        {
            var dna = new Dna.Dna();
            var context = new Context();
            while (true)
            {
                byte nextInstruction;
                if (!_buffer.GetByte(out nextInstruction))
                    return null;

                if (nextInstruction == StopInstruction.Code)
                    break;

                var instruction = Instructions[nextInstruction];
                if (instruction == null)
                    return null;

                if (!instruction.Execute(context, _buffer, dna))
                    return null;
            }

            return dna;
        }
    }
}
