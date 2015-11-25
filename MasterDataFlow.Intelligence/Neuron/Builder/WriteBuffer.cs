using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Intelligence.Neuron.Builder
{
    public class WriteBuffer
    {
        private readonly MemoryStream _stream = new MemoryStream(1024);

        public WriteBuffer()
        {
            
        }

        public void Write(byte value)
        {
            _stream.WriteByte(value);
        }

        public void Write(byte[] value)
        {
            _stream.Write(value, 0, value.Length);
        }

        public byte[] ToArray()
        {
            var result = new byte[_stream.Length];
            _stream.Read(result, 0, result.Length);
            return result;
        }

    }
}
