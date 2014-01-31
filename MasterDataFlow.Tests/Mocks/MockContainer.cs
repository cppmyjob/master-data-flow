using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow.Tests.Mocks
{
    public class MockContainer : BaseContainter
    {
        volatile private bool _isExecuted;

        public bool IsExecuted {
            get { return _isExecuted; }
            set { _isExecuted = value; }
        }

        public override void Execute(CommandInfo info, OnExecuteContainer onExecute)
        {
            IsExecuted = true;
            onExecute(this, info);
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
