using System;
using MasterDataFlow.Intelligence.Interfaces;
using MasterDataFlow.Intelligence.Neuron;
using MasterDataFlow.Intelligence.Neuron.SimpleNeuron;
using MasterDataFlow.Trading.Interfaces;
using MasterDataFlow.Trading.Tester;

namespace MasterDataFlow.Trading.Advisors
{
    public enum NeuralNetworkAdvisorStatus
    {
        Init,

    } 

    public class NeuralNetworkAdvisorInfo
    {
        public NeuralNetworkAdvisorInfo()
        {
            Status = NeuralNetworkAdvisorStatus.Init;
            IsLoaded = false;
        }

        internal bool IsLoaded { get; set; }

        public NeuralNetworkAdvisorStatus Status { get; set; }

        public void Load()
        {

            IsLoaded = true;
        }
    }


    public  class NeuralNetworkAdvisor : BaseAdvisor
    {
        private readonly ISimpleNeuron _neuronNetwork;
        private readonly NeuralNetworkAdvisorInfo _info = new NeuralNetworkAdvisorInfo();

        public NeuralNetworkAdvisor(ITrader trader, ISimpleNeuron neuronNetwork) : base(trader)
        {
            _neuronNetwork = neuronNetwork;
        }

        protected override AdvisorAction GetAction(DateTime time, decimal price)
        {
            switch (_info.Status)
            {
                case NeuralNetworkAdvisorStatus.Init:
                    return ProcessInit(time, price);
                    
            }

            return AdvisorAction.Nothing;
        }

        private AdvisorAction ProcessInit(DateTime time, decimal price)
        {
            if (!_info.IsLoaded)
            {
                _info.Load();
                return GetAction(time, price);
            }

            return CheckSignal(time, price);
        }

        private AdvisorAction CheckSignal(DateTime time, decimal price)
        {
            throw new NotImplementedException();
        }
    }

}
