using System;

namespace MasterDataFlow.Intelligence.Neuron.Builder
{
    public class ReadBuffer
    {
        private readonly byte[] _data;
        private int _offset;

        public ReadBuffer(byte[] data)
        {
            _data = data;
        }

        public bool GetByte(out byte value)
        {
            if (_offset >= _data.Length)
            {
                value = 0;
                return false;
            }
            value = _data[_offset++];
            return true;
        }


        public int GetInt()
        {
            var result = BitConverter.ToInt32(_data, _offset);
            _offset += 4;
            return result;
        }

        public float GetFloat()
        {
            var result = BitConverter.ToSingle(_data, _offset);
            _offset += 4;
            return result;
        }

    }
}
