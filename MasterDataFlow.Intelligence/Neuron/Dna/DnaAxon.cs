using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Intelligence.Neuron.Dna
{
    public class DnaAxon
    {
        public DnaAxon()
        {
            
        }

        public DnaAxon(int id, int[] indexes)
        {
            Id = id;
            Indexes = indexes;
        }

        public int Id { get; set; }
        public int[] Indexes { get; set; }
    }
}
