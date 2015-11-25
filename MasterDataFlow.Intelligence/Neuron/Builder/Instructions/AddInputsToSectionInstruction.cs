using MasterDataFlow.Intelligence.Interfaces;

namespace MasterDataFlow.Intelligence.Neuron.Builder.Instructions
{
    /// <summary>
    /// 
    /// </summary>
    public class AddInputsToSectionInstruction : IDnaInstruction
    {
        public const byte Code = 0x04;

        public bool Execute(Context context, ReadBuffer buffer, Dna.Dna dna)
        {

            return true;
        }
    }
}
