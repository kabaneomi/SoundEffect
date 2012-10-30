using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SoundApplication
{
    //! ID-bit.
    public enum eIDBit
    {
        eIDBit_LSF = 0,
        eIDBit_HSF,
    }

    //! Layer.
    public enum eLayerBit
    {
        eLayerBit_Reserve = 0,
        eLayerBit_Third,           //! Layer3.
        eLayerBit_Second,          //! Layer2.
        eLayerBit_First,           //! Layer1.
    }

    public struct cMP3Data
    {
        public int frame_bit;  //! mp3 header frame.
        public eIDBit id_bit;
        public eLayerBit layer_bit;
    }

    class cMP3Header
    {
        public void readMp3Data(ref cMP3Data data, string filename)
        {
            //! file open.
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            Console.Write("mp3ファイル名：{0}\n", filename);

            //! get header info.
            byte[] header_byte = new byte[sizeof(int)];
            br.Read(header_byte, 0, sizeof(int));
            Console.Write("header_byte0：0x{0:x2}\n", header_byte[0]);
            Console.Write("header_byte1：0x{0:x2}\n", header_byte[1]);
            Console.Write("header_byte2：0x{0:x2}\n", header_byte[2]);
            Console.Write("header_byte3：0x{0:x2}\n", header_byte[3]);
            if (BitConverter.IsLittleEndian)
            {
                data.frame_bit = BitConverter.ToInt32(header_byte.Reverse().ToArray(), 0);
            }
            else
            {
                data.frame_bit = BitConverter.ToInt32(header_byte, 0);
            }
            Console.Write("header：0x{0:x8} {1}\n", data.frame_bit, sizeof(int));

            //! get ID-bit.
            int id_bit = data.frame_bit & 0x00080000;
            id_bit = id_bit >> 19;
            data.id_bit = (eIDBit)id_bit;

            //! get Layer-bit.
            int layer_bit = data.frame_bit & 0x00060000;
            Console.Write("layer_bit：0x{0:x8} {1}\n", layer_bit, sizeof(int));
            layer_bit = layer_bit >> 17;
            Console.Write("layer_bit shift：0x{0:x8} {1}\n", layer_bit, sizeof(int));
            data.layer_bit = (eLayerBit)layer_bit;
        }
    }
}
