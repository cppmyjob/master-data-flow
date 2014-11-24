using System;
using System.Data;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Messages;

namespace MasterDataFlow.Tests.ExternalAssembly
{
    public class MathCommand : Command<MathCommand.MathCommandDataObject>
    {
        public class MathCommandDataObject : ICommandDataObject
        {
            public string Expression { get; set; }
        }

        public class MathCommandResult : ICommandDataObject
        {
            public string Result { get; set; }
        }

        public override BaseMessage Execute()
        {
            var dt = new DataTable();
            var v = dt.Compute(DataObject.Expression, "");
            Console.WriteLine(DataObject.Expression + " = " +v.ToString());
            return Stop(new MathCommandResult() {Result = v.ToString()});
        }
    }
}
