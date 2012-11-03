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

    //! Emphasis.
    public enum eEmphasis
    {
        None = 0,       //! emphasis None.
        FifthFifteen,   //! 50/15us.
        Reserve,        //! reserve.
        CCITT_J17,      //! CCITT J17.
    }

    public struct sMP3Data
    {
        public string tag_name;
        public int tag_version;
        public int tag_flag;
        public int tag_size;

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
        public int copyright_bit;  //! 0 = None, 1 = Protect.
        public int original_bit;   //! 0 = copy, 1 = original.
        public eEmphasis emphasis_bit;
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
            Console.Write("mp3 file name：{0}\n", filename);

            //! tag check.
            FileStream tag_fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            checkTag(ref tag_fs, ref br, ref data);

            //! get header info.
            byte[] header_byte = new byte[sizeof(int)];
            br.Read(header_byte, 0, sizeof(int));
            converIntValue(ref data.frame_bit, header_byte, 0);
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
            private_bit = private_bit >> 8;
            data.private_bit = private_bit;
            
            //! Mode Bit.
            int mode_bit = data.frame_bit & 0x000000C0;
            mode_bit = mode_bit >> 6;
            data.mode_bit = (eModeBit)mode_bit;

            //! ModeExtension Bit.
            int modeEx_bit = data.frame_bit & 0x00000030;
            modeEx_bit = modeEx_bit >> 4;
            data.modeEx_bit = (eModeExtension)modeEx_bit;

            //! Copyright.
            int copyright_bit = data.frame_bit & 0x00000008;
            copyright_bit = copyright_bit >> 3;
            data.copyright_bit = copyright_bit;

            //! Original Check.
            int original_bit = data.frame_bit & 0x00000004;
            original_bit = original_bit >> 2;
            data.original_bit = original_bit;

            //! Emphasis Bit.
            int emphasis_bit = data.frame_bit & 0x00000003;
            Console.Write("emphasis_bit：0x{0:x8} {1}\n", emphasis_bit, sizeof(int));
            data.emphasis_bit = (eEmphasis)emphasis_bit;
        }

        //! tag check.
        private void checkTag(ref FileStream fs, ref BinaryReader br, ref sMP3Data data)
        {
            BinaryReader tag_br = new BinaryReader(fs);

            byte[] tag_byte = new byte[3];
            tag_br.Read(tag_byte, 0, 3);
            string tag_name = Encoding.ASCII.GetString(tag_byte);
            Console.Write("tag：{0}\n", tag_name);

            //! "ID3" check.
            if (tag_name != "ID3")
            {
                data.tag_name = "";
                return;
            }

            //! get tag info.
            br.Read(tag_byte, 0, 3);
            data.tag_name = Encoding.ASCII.GetString(tag_byte);
            Console.Write("data.tag_name：{0}\n", data.tag_name);

            //! get tag version.
            int tag_bit = 0;
            byte[] tag_version_byte = new byte[4];
            br.Read(tag_version_byte, 0, 2);
            converIntValue(ref tag_bit, tag_version_byte, 0);
            data.tag_version = tag_bit & 0x0000FFFF;
            Console.Write("data.tag_version：0x{0:x8}\n", data.tag_version);

            //! get tag flag.
            byte[] tag_flag_byte = new byte[4];
            br.Read(tag_flag_byte, 0, 1);
            converIntValue(ref tag_bit, tag_flag_byte, 0);
            data.tag_flag = tag_bit & 0x000000FF;
            Console.Write("data.tag_flag：0x{0:x8}\n", data.tag_flag);

            //! get tag size.
            byte[] tag_size_byte = new byte[4];
            br.Read(tag_size_byte, 0, 4);
            converIntValue(ref tag_bit, tag_size_byte, 0);
            data.tag_size = (int)(tag_bit & 0xf7f7f7f7);
            Console.Write("data.tag_size：0x{0:x8}, {1}\n", data.tag_size, data.tag_size);
        }

        private void converIntValue(ref int bit, byte[] info, int size)
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
