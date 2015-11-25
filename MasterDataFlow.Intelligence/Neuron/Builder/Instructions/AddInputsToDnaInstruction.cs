using MasterDataFlow.Intelligence.Interfaces;

namespace MasterDataFlow.Intelligence.Neuron.Builder.Instructions
{
    /// <summary>
    /// 
    /// </summary>
    public class AddInputsToDnaInstruction : IDnaInstruction
    {
        public const byte Code = 0x02;

        public bool Execute(Context context, ReadBuffer buffer, Dna.Dna dna)
        {

            return true;
        }
    }
}
