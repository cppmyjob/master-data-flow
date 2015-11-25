using MasterDataFlow.Intelligence.Interfaces;
using MasterDataFlow.Intelligence.Neuron.Builder;
using MasterDataFlow.Intelligence.Neuron.Builder.Instructions;

namespace MasterDataFlow.Intelligence.Neuron.Dna
{
    public class Dna : DnaInOut, IDnaSerializator
    {
        public DnaSection[] Sections { get; set; }
        public void Serialize(WriteBuffer buffer)
        {
            AddSectionsInstruction.Serialize(buffer, this);

            foreach (var section in Sections)
            {
                section.Serialize(buffer);
            }

            buffer.Write(StopInstruction.Code);
        }
    }
}
