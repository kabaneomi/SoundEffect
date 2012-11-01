using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace SoundApplication
{
    //! ID-bit.
    public enum eIDBit
    {
        LSF = 0,    //! MPEG-2.
        HSF,        //! MPEG-1.
    }

    //! Layer.
    public enum eLayerBit
    {
        Reserve = 0,
        Layer3,           //! Layer3.
        Layer2,           //! Layer2.
        Layer1,           //! Layer1.
    }

    //! Protection-Bit.
    public enum eProtectionBit
    {
        ISOProt0_Layer1 = 0,  //! Layer1 : CRC Error Protect.
        ISOProt1_Layer1,      //! Layer1 : None.
        ISOProt0_Layer2or3,   //! Layer2or3 : CRC Error Protect.
        ISOProt1_Layer2or3,   //! Layer2or3 : None.
    }

    //! Mode-Bit.
    public enum eModeBit
    {
        Stereo = 0,
        JointStereo,
        DualChannel,          //! Stereo.
        SingleChannel,        //! Monoral.
    }

    //! ModeExtension-Bit.
    //! ModeBit DualChannel ON.
    public enum eModeExtension
    {
        Subband4 = 0,
        Subband8,
        Subband12,
        Subband16,
    }

    public struct sMP3Data
    {
        public int frame_bit;  //! mp3 header frame.
        public eIDBit id_bit;
        public eLayerBit layer_bit;
        public eProtectionBit protection_bit;
        public int bitrate;
        public int sampling;
        public int padding_bit;    //! 0 = None, 1 = Add.
        public int private_bit;    //! 0 = None, 1 = Use.
        public eModeBit mode_bit;
        public eModeExtension modeEx_bit;
    }

    public class cMP3Header
    {
        //! Bitrate Table.
        private int[] bitrateTableLSF = new int[] { 0, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 64, 128, 144, 160, 0 };
        private int[] bitrateTableHSF = new int[] { 0, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 0 };

        //! Sampling Frequency.
        private int[] samplingLSF = new int[] { 22050, 24000, 16000, 0 };
        private int[] samplingHSF = new int[] { 44100, 48000, 32000, 0 };

        public void readMp3Data(ref sMP3Data data, string filename)
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
            layer_bit = layer_bit >> 17;
            data.layer_bit = (eLayerBit)layer_bit;

            //! Protection-Bit.
            int protection_bit = data.frame_bit & 0x00010000;
            protection_bit = protection_bit >> 16;
            switch (data.layer_bit)
            {
                case eLayerBit.Layer3:
                case eLayerBit.Layer2:
                    if(protection_bit == 0)
                    {
                        data.protection_bit = eProtectionBit.ISOProt0_Layer2or3;
                    }
                    else
                    {
                        data.protection_bit = eProtectionBit.ISOProt1_Layer2or3;
                    }
                    break;
                case eLayerBit.Layer1:
                    if (protection_bit == 0)
                    {
                        data.protection_bit = eProtectionBit.ISOProt0_Layer1;
                    }
                    else
                    {
                        data.protection_bit = eProtectionBit.ISOProt1_Layer1;
                    }
                    break;
                default:
                    Debug.Assert(false, "Protection Bit None?");
                    break;
            }

            //! Bitrate.
            int bitrate_bit = data.frame_bit & 0x0000F000;
            bitrate_bit = bitrate_bit >> 12;
            switch (data.id_bit)
            {
                case eIDBit.LSF:
                    data.bitrate = bitrateTableLSF[bitrate_bit];
                    break;
                case eIDBit.HSF:
                    data.bitrate = bitrateTableHSF[bitrate_bit];
                    break;
                default:
                    break;
            }
            Console.Write("bitrate：{0}\n", data.bitrate);

            //! Sampling Frequency.
            int sampling_bit = data.frame_bit & 0x00000C00;
            sampling_bit = sampling_bit >> 10;
            switch (data.id_bit)
            {
                case eIDBit.LSF:
                    data.sampling = samplingLSF[sampling_bit];
                    break;
                case eIDBit.HSF:
                    data.sampling = samplingHSF[sampling_bit];
                    break;
                default:
                    break;
            }
            Console.Write("sampling：{0}\n", data.sampling);

            //! Padding Bit.
            int padding_bit = data.frame_bit & 0x00000300;
            padding_bit = padding_bit >> 9;
            data.padding_bit = padding_bit;

            //! Private Bit.
            int private_bit = data.frame_bit & 0x00000100;
            Console.Write("private_bit：0x{0:x8} {1}\n", private_bit, sizeof(int));
            private_bit = private_bit >> 8;
            Console.Write("private_bit shift：0x{0:x8} {1}\n", private_bit, sizeof(int));
            data.private_bit = private_bit;
            
            //! Mode Bit.
            int mode_bit = data.frame_bit & 0x000000C0;
            mode_bit = mode_bit >> 6;
            data.mode_bit = (eModeBit)mode_bit;

            //! ModeExtension Bit.
            int modeEx_bit = data.frame_bit & 0x00000030;
            Console.Write("modeEx_bit：0x{0:x8} {1}\n", modeEx_bit, sizeof(int));
            modeEx_bit = modeEx_bit >> 4;
            Console.Write("modeEx_bit shift：0x{0:x8} {1}\n", modeEx_bit, sizeof(int));
            data.modeEx_bit = (eModeExtension)modeEx_bit;
        }
    }
}
