using System;
using System.Data;
using MasterDataFlow;
using MasterDataFlow.Interfaces;
using MasterDataFlow.Messages;

namespace Examples.Wcf.Client
{
    public class MathCommand : Command<MathCommand.MathCommandDataObject>
    {
        [Serializable]
        public class MathCommandDataObject : ICommandDataObject
        {
            public string Expression { get; set; }
        }

        [Serializable]
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
