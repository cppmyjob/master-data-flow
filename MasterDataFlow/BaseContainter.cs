using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow
{
    public delegate void OnExecuteContainer(BaseContainter container, CommandInfo info);

    public abstract class BaseContainter : IDisposable
    {
        public abstract void Execute(CommandInfo info, OnExecuteContainer onExecute);

        public abstract void Dispose();
    }
}
