using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Advisors;

namespace MasterDataFlow.Trading.Ui.Business.Advisor
{
    public class MemoryAdvisorInfo : IAdvisorInfo
    {
        public MemoryAdvisorInfo()
        {
            Status = AdvisorStatus.Init;
            IsLoaded = false;
        }

        public bool IsLoaded { get; internal set; }

        public AdvisorStatus Status { get; private set; }

        public DateTime? LastSignalTime { get; private set; }

        public AdvisorSignal? LastSignal { get; private set; }

        public void SetLastSignal(AdvisorSignal signal, DateTime time)
        {
            LastSignalTime = time;
            LastSignal = signal;
            Save();
        }

        public void SetStatus(AdvisorStatus status)
        {
            Status = status;
            Save();
        }

        public void Load()
        {
            IsLoaded = true;
        }

        private void Save()
        {

        }
    }
}
