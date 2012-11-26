using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundApplication
{
    class cCommonFunc
    {
        public void converIntValue(ref int bit, byte[] info, int size)
        {
            if (BitConverter.IsLittleEndian)
            {
                bit = BitConverter.ToInt32(info.Reverse().ToArray(), size);
            }
            else
            {
                bit = BitConverter.ToInt32(info, size);
            }
        }
    }
}
