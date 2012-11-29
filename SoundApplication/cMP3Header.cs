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
        public byte[] mb_HeaderID3v2;
        public short mh_ID3v2Version;
        public short mh_ID3v2Flag;
        public int mh_ID3v2Size;
        public byte[] mb_ID3v2TagInfo;

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

        //! common func.
        cCommonFunc common_func = new cCommonFunc();


        //! analyze start mp3.
        public void readMp3Data(ref sMP3Data data, string filename)
        {
            //! file open.
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            Console.Write("mp3 file name：{0}\n", filename);

            //! analyze ID3v2tag.
            FileStream tag_fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            analyzeID3v2(ref tag_fs, ref br, ref data);

            //! get header info.
            analyzeFrameHeader(ref br, ref data);
        }

        //! analyze ID3v2tag.
        private void analyzeID3v2(ref FileStream fs, ref BinaryReader br, ref sMP3Data data)
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

            data.mb_HeaderID3v2 = new byte[10];
            br.Read(data.mb_HeaderID3v2, 0, 10);
            Console.Write("header_byte：0x");
            for (int count = 0; count < 10; count++)
            {
                Console.Write("{0:x2}", data.mb_HeaderID3v2[count]);
            }
            Console.Write("\n");

            //! header name.
            string header_name = Encoding.ASCII.GetString(data.mb_HeaderID3v2, 0, 3);
            Console.Write("header_name：{0}\n", header_name);
            //! header version.
            data.mh_ID3v2Version = BitConverter.ToInt16(data.mb_HeaderID3v2, 3);
            Console.Write("header_version：0x{0:x8}\n", data.mh_ID3v2Version);
            //! header flag.
            Console.Write("header_flag：0x{0:x8}\n", data.mb_HeaderID3v2[5]);
            //! header size.
            byte[] byte_switch = new byte[4];
            byte byte_set_offset = 0;
            for (int count = 0; count < 4; count++)
            {
                byte_switch[count] = (byte)(data.mb_HeaderID3v2[count + 6] & 0x7f);
            }
            for (int count = 0; count < 3; count++)
            {
                byte_set_offset = (byte)(byte_switch[2-count] << (7-count));
                byte_switch[3-count] = (byte)(byte_switch[3-count] | byte_set_offset);
                byte_switch[2-count] = (byte)(byte_switch[2-count] >> (1+count));
            }

            common_func.converIntValue(ref data.mh_ID3v2Size, byte_switch, 0);
            Console.Write("header_size：{0}\n", data.mh_ID3v2Size);
            Console.Write("header_size  ：0x{0:x2}{1:x2}{2:x2}{3:x2}\n", data.mb_HeaderID3v2[6], data.mb_HeaderID3v2[7], data.mb_HeaderID3v2[8], data.mb_HeaderID3v2[9]);
            Console.Write("header_switch：0x{0:x2}{1:x2}{2:x2}{3:x2}\n", byte_switch[0], byte_switch[1], byte_switch[2], byte_switch[3]);

            data.mb_ID3v2TagInfo = new byte[data.mh_ID3v2Size];
            br.Read(data.mb_ID3v2TagInfo, 0, data.mh_ID3v2Size);
        }

        //! analyze frame header.
        private void analyzeFrameHeader(ref BinaryReader br, ref sMP3Data data)
        {
            byte[] header_byte = new byte[sizeof(int)];
            br.Read(header_byte, 0, sizeof(int));
            common_func.converIntValue(ref data.frame_bit, header_byte, 0);
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
                    if (protection_bit == 0)
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


            //! frame tag checktest.
            byte[] get_info = null;
            int info_size = 0;
            bool ret = searchID3v2(ref get_info, ref info_size, data, "TSS");
            if (ret)
            {
                string tss_name;
                tss_name = Encoding.ASCII.GetString(get_info, 1, info_size-1);
                Console.Write("string_code_bit：0x{0:x2}\n", get_info[0]);
                Console.Write("tss_name：{0}\n", tss_name);
            }
        }

        //! search ID3v2tag info.
        public bool searchID3v2(ref byte[] byte_info, ref int info_size, sMP3Data data, string tag)
        {
            int frame_check = 0;
            byte[] byte_frame = new byte[6];
            string frame_name;
            byte[] byte_switch = new byte[4];
            int frame_size = 0;
            int compare = 0;
            while (true)
            {
                //! frame size over break.
                if (frame_check >= data.mh_ID3v2Size) break;
                Console.Write("frame_check：{0}\n", frame_check);
                //! frame name & size check.
                for (int count = 0; count < 6; count++)
                {
                    byte_frame[count] = data.mb_ID3v2TagInfo[frame_check + count];
                }
                Console.Write("byte_frame：0x{0:x2}{1:x2}{2:x2}{3:x2}{4:x2}{5:x2}\n", byte_frame[0], byte_frame[1], byte_frame[2], byte_frame[3], byte_frame[4], byte_frame[5]);
                //! frame name check.
                frame_name = Encoding.ASCII.GetString(byte_frame, 0, 3);
                Console.Write("frame_name：{0}\n", frame_name);
                byte_frame[2] = (byte)(byte_frame[2] & 0x00);
                for (int count = 0; count < 4; count++)
                {
                    byte_switch[count] = byte_frame[count + 2];
                }
                common_func.converIntValue(ref frame_size, byte_switch, 0);
                Console.Write("frame_size：{0}\n", frame_size);
                compare = string.Compare(tag, frame_name);
                if (compare == 0)
                {
                    byte_info = new byte[frame_size];
                    info_size = frame_size;
                    Array.Copy(data.mb_ID3v2TagInfo, frame_check + 6, byte_info, 0, frame_size);
                    return true;
                }
                frame_check += frame_size + 6;
            }
            return false;
        }
    }
}
