using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Trading.Interfaces;
using MasterDataFlow.Trading.Tester;

namespace MasterDataFlow.Trading.Advisors
{
    public enum Operationtatus
    {
        Ok,
        Repeat,
        Error,
    }

    public enum AdvisorStatus
    {
        Init,
    }

    public enum AdvisorSignal
    {
        Skip = 0,
        Hold = 1,
        Close = 2,
        Sell = 3,
        Buy = 4
    }


    public interface IAdvisorInfo
    {
        bool IsLoaded { get; }

        AdvisorStatus Status { get; }
        DateTime? LastSignalTime { get; }
        AdvisorSignal? LastSignal { get; }

        void SetLastSignal(AdvisorSignal signal, DateTime time);
        void SetStatus(AdvisorStatus status);

        void Load();
    }

    public class AdvisorInfo : IAdvisorInfo
    {
        public AdvisorInfo()
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


    public abstract class BaseAdvisor
    {
        private readonly IAdvisorInfo _advisorInfo;
        private readonly ILogger _logger;
        private readonly ITrader _trader;

        protected BaseAdvisor(IAdvisorInfo advisorInfo, ITrader trader, ILogger logger)
        {
            _advisorInfo = advisorInfo;
            _logger = logger;
            _trader = trader;
        }

        public IAdvisorInfo Info
        {
            get { return _advisorInfo; }
        }

        public void Tick(DateTime time, decimal price)
        {
            if (_advisorInfo.Status == AdvisorStatus.Init)
            {
                if (!_advisorInfo.IsLoaded)
                    _advisorInfo.Load();
            }

            var action = GetSignal(time, price);
            switch (action)
            {
                case AdvisorSignal.Sell:
                    _advisorInfo.SetLastSignal(action, DateTime.Now);
                    ProcessSell();
                    break;
                case AdvisorSignal.Buy:
                    _advisorInfo.SetLastSignal(action, DateTime.Now);
                    ProcessBuy();
                    break;
                case AdvisorSignal.Hold:
                    _advisorInfo.SetLastSignal(action, DateTime.Now);
                    break;
                case AdvisorSignal.Close:
                    _advisorInfo.SetLastSignal(action, DateTime.Now);
                    break;
                case AdvisorSignal.Skip:
                    break;
            }
            
        }

        private void ProcessBuy()
        {
            if (_trader.IsSellOrderExists())
            {
                _trader.CloseSellOrder();
            }
            if (!_trader.IsBuyOrderExists())
            {
                _trader.BuyOrder();
            }
        }

        private void ProcessSell()
        {

        }

        protected abstract AdvisorSignal GetSignal(DateTime time, decimal price);

    }

}
