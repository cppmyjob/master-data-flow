using System;
using System.Collections.Generic;
using System.Text;
using MasterDataFlow.Trading.Common;
using MasterDataFlow.Trading.Tester;

namespace MasterDataFlow.Trading.Tests.Mocks
{
    public class TesterMock : FxDirectionTester
    {
        private readonly FxDirection[] _directions;

        public TesterMock(FxDirection[] directions, double deposit, Bar[] prices, int @from, int length) 
            : base(deposit, prices, @from, length)
        {
            _directions = directions;
        }

        protected override FxDirectionGetDirection GetDirectionDelegate()
        {
            return (int index) => _directions[index];
        }

        protected override int GetStopLoss()
        {
            return 0;
        }
    }
}
