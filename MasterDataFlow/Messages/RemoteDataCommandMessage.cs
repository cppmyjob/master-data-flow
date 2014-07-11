using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterDataFlow.Interfaces;

namespace MasterDataFlow.Messages
{
    public class RemoteDataCommandMessage : ILoopCommandMessage
    {
        private readonly string _data;
        private readonly string _dataType;

        public RemoteDataCommandMessage(string dataType, string data)
        {
            _dataType = dataType;
            _data = data;
        }

        public string DataType
        {
            get { return _dataType; }
        }

        public string Data
        {
            get { return _data; }
        }
    }
}
