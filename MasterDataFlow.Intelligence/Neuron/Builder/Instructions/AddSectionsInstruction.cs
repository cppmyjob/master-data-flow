using System;
using MasterDataFlow.Intelligence.Interfaces;
using MasterDataFlow.Intelligence.Neuron.Dna;

namespace MasterDataFlow.Intelligence.Neuron.Builder.Instructions
{
    /// <summary>
    /// 0x00 Code
    /// 0x01 Section Number
    /// </summary>
    public class AddSectionsInstruction : IDnaInstruction
    {
        public const byte Code = 0x01;

        public bool Execute(Context context, ReadBuffer buffer, Dna.Dna dna)
        {
            byte b;
            if (!buffer.GetByte(out b))
                return false;
            var sectionNumber = b % context.MaxSectionsNumber;
            if (dna.Sections == null)
            {
                dna.Sections = new DnaSection[sectionNumber];
                for (var i = 0; i < sectionNumber; i++)
                {
                    dna.Sections[i] = new DnaSection();
                }
            }
            else
            {
                var newSectionsNumber = Math.Min(dna.Sections.Length + sectionNumber, context.MaxSectionsNumber);
                var newSections = new DnaSection[newSectionsNumber];
                Array.Copy(dna.Sections, 0, newSections, 0, dna.Sections.Length);
                for (var i = dna.Sections.Length; i < newSectionsNumber - dna.Sections.Length; i++)
                {
                    newSections[i] = new DnaSection();
                }
            }
            return true;
        }

        public static void Serialize(WriteBuffer buffer, Dna.Dna dna)
        {
            buffer.Write(Code);
            buffer.Write((byte)dna.Sections.Length);
        }
    }
}
