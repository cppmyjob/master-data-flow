using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MasterDataFlow.Network;
using Microsoft.CodeAnalysis.CSharp;

namespace MasterDataFlow.Trading.Ui.Business
{
    public class BaseCommandController
    {
        public async Task Execute()
        {
            using (var remote = new RemoteEnvironment())
            {
                await ExecuteCommand(remote.CommandWorkflow);
            }
        }

        private Task ExecuteCommand(CommandWorkflow remoteCommandWorkflow)
        {
            throw new NotImplementedException();
        }
    }
}
