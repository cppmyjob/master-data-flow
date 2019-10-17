using System.Linq;
using MasterDataFlow.Trading.Data;

namespace MasterDataFlow.Trading.Shared.Data.Indicators
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

        public override float GetMax()
        {
            return 500;
        }

        public override float GetMin()
        {
            return 0;
        }
    }



}
