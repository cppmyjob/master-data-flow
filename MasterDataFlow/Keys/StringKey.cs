using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Keys
{
    public class StringKey : SubscribeKey
    {
        private readonly string _value;

        public StringKey(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }
    }
}
