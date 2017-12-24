using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Data;

namespace MasterDataFlow.Trading.Indicators
{
    public class ZigZag
    {
        public static List<int> Calculate(Bar[] quotes, int obsStart, int obsEnd, decimal minSwingPct)
        {
            bool swingHigh = false, swingLow = false;
            var obsLow = obsStart;
            var obsHigh = obsStart;
            var zigZag = new List<int>();
            for (int obs = obsStart; obs <= obsEnd; obs++)
            {
                if (quotes[obs].High > quotes[obsHigh].High)
                {
                    obsHigh = obs;
                    if (!swingLow &&
                        ((quotes[obsHigh].High - quotes[obsLow].Low) / quotes[obsLow].Low) * 100M >= minSwingPct)
                    {
                        zigZag.Add(obsLow);  // new swinglow
                        swingHigh = false; swingLow = true;
                    }
                    if (swingLow) obsLow = obsHigh;
                }
                else if (quotes[obs].Low < quotes[obsLow].Low)
                {
                    obsLow = obs;
                    if (!swingHigh &&
                        ((quotes[obsHigh].High - quotes[obsLow].Low) / quotes[obsLow].Low) * 100M >= minSwingPct)
                    {
                        zigZag.Add(obsHigh);  // new swinghigh
                        swingHigh = true; swingLow = false;
                    }
                    if (swingHigh) obsHigh = obsLow;
                }
            }
            return zigZag;
        }

    }
}
