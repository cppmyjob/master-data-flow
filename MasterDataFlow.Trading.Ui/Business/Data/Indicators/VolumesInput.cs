using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Data;
using Trady.Analysis;
using Trady.Analysis.Extension;
using Trady.Core.Infrastructure;

namespace MasterDataFlow.Trading.Ui.Business.Data.Indicators
{
    public class VolumesInput : BaseInput
    {
        public VolumesInput() : base("Volumes")
        {
        }

        public override InputValues GetValues(Bar[] bars)
        {
            var values = bars.Select(t => new InputValue(t.Time, (float)t.Volume)).ToArray();
            var result = new InputValues(Name, values);
            return result;

        }
    }



}
