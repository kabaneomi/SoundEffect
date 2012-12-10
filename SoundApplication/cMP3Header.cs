using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace SoundApplication
{
    //! ID-bit.
    public enum eMPEGVer
    {
        SecondHalf = 0,    //! MPEG version 2.5.
        None,              //! None.
        Second,            //! MPEG version 2.
        First,             //! MPEG version 1.
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
        Subband4 = 0,   //! 4 - 31.
        Subband8,       //! 8 - 31.
        Subband12,      //! 12 - 31.
        Subband16,      //! 16 - 31.
    }

    //! Emphasis.
    public enum eEmphasis
    {
        None = 0,       //! emphasis None.
        FifthFifteen,   //! 50/15us.
        Reserve,        //! reserve.
        CCITT_J17,      //! CCITT revise form.
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
        public eMPEGVer mpeg_ver;
        public eLayerBit layer_bit;
        public eProtectionBit protection_bit;
        public int bitrate;
        public int samplingrate;
        public int padding_bit;    //! 0 = None, 1 = Add.
        public int private_bit;    //! 0 = None, 1 = Use.
        public eModeBit mode_bit;
        public eModeExtension modeEx_bit;
        public int copyright_bit;  //! 0 = None, 1 = Protect.
        public int original_bit;   //! 0 = copy, 1 = original.
        public eEmphasis emphasis_bit;
        public int frame_size;
    }

    public class cMP3Header
    {
        //! Bitrate Table(V = MPEG ver, L = Layer ver).
        private int[][] m_BitRateTable = new int[][] { 
            new int[]{0, 32, 64, 96, 128, 160, 192, 224, 256, 288, 320, 352, 384, 416, 448, -1},    //! V1,L1.
            new int[]{0, 32, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 384, -1},       //! V1,L2.
            new int[]{0, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, -1},        //! V1,L3.
            new int[]{0, 32, 48, 56, 64, 80, 96, 112, 128, 144, 160, 176, 192, 224, 256, -1},       //! V2,L1.
            new int[]{0, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160, -1}             //! V2,L2&L3.
        };

        //! Sampling Frequency.
        private int[][] m_SamplingRateTable = new int[][] {
            new int[]{44100, 48000, 32000, 0},  //! MPEG1.
            new int[]{22050, 24000, 16000, 0},  //! MPEG2.
            new int[]{11025, 12000, 8000, 0}    //! MPEG2.5.
        };

        //! Frame Size(ver1, ver2, ver2.5).
        private int[][] m_FrameSizeTable = new int[][] {
            new int[]{384, 384, 384},       //! Rayer1.
            new int[]{1152, 1152, 1152},    //! Rayer2.
            new int[]{1152, 576, 576}       //! Rayer3.
        };

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
            bool ver_check = analyzeID3v2(ref tag_fs, ref br, ref data);
            //! mp3 analyze version 2.2 only.
            if (!ver_check)
            {
                return;
            }

            //! get header info.
            while (true)
            {
                if (!analyzeFrameHeader(ref br, ref data))
                {
                    break;
                }
            }
        }

        //! analyze ID3v2tag.
        private bool analyzeID3v2(ref FileStream fs, ref BinaryReader br, ref sMP3Data data)
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
                return true;
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
            if (data.mh_ID3v2Version != 2)
            {
                return false;
            }
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

            return true;
        }

        //! analyze frame header.
        private bool analyzeFrameHeader(ref BinaryReader br, ref sMP3Data data)
        {
            //! header 4byte.
            byte[] header_byte = new byte[4];
            br.Read(header_byte, 0, 4);
            common_func.converIntValue(ref data.frame_bit, header_byte, 0);
            Console.Write("header：0x{0:x8} {1}\n", data.frame_bit, sizeof(int));

            //! check first bit.
            int check_bit = data.frame_bit >> 21;
            check_bit = check_bit & 0x00000FFF;
            if (check_bit == 0)
            {
                return false;
            }

            //! get ID-bit.
            int mpeg_ver = data.frame_bit >> 19;
            mpeg_ver = mpeg_ver & 0x00000003;
            data.mpeg_ver = (eMPEGVer)mpeg_ver;
            Console.Write("mpeg_ver：0x{0:x8}\n", mpeg_ver);

            //! get Layer-bit.
            int layer_bit = data.frame_bit >> 17;
            layer_bit = layer_bit & 0x00000003;
            data.layer_bit = (eLayerBit)layer_bit;
            Console.Write("layer_bit：0x{0:x8}\n", layer_bit);

            //! Protection-Bit.
            int protection_bit = data.frame_bit >> 16;
            protection_bit = protection_bit & 0x00000001;
            data.protection_bit = (eProtectionBit)protection_bit;
            Console.Write("protection_bit：0x{0:x8}\n", protection_bit);

            //! BitRateTable.
            int bitrate = data.frame_bit >> 12;
            bitrate = bitrate & 0x0000000F;
            if (data.mpeg_ver == eMPEGVer.First)
            {
                switch (data.layer_bit)
                {
                    case eLayerBit.Layer1:
                        data.bitrate = m_BitRateTable[0][bitrate];
                        break;
                    case eLayerBit.Layer2:
                        data.bitrate = m_BitRateTable[1][bitrate];
                        break;
                    case eLayerBit.Layer3:
                        data.bitrate = m_BitRateTable[2][bitrate];
                        break;
                    default:
                        Console.Write("Get Bitrate : None Layer\n");
                        break;
                }
            }
            else if (data.mpeg_ver == eMPEGVer.Second || data.mpeg_ver == eMPEGVer.SecondHalf)
            {
                switch (data.layer_bit)
                {
                    case eLayerBit.Layer1:
                        data.bitrate = m_BitRateTable[3][bitrate];
                        break;
                    case eLayerBit.Layer2:
                    case eLayerBit.Layer3:
                        data.bitrate = m_BitRateTable[4][bitrate];
                        break;
                    default:
                        Console.Write("Get Bitrate : None Layer\n");
                        break;
                }
            }
            else
            {
                Console.Write("MPEG ver None BitrateTable\n");
                return false;
            }
            Console.Write("bitrate：{0}\n", data.bitrate);

            //! Sampling Frequency.
            int sampling = data.frame_bit >> 10;
            sampling = sampling & 0x00000003;
            switch (data.mpeg_ver)
            {
                case eMPEGVer.First:
                    data.samplingrate = m_SamplingRateTable[0][sampling];
                    break;
                case eMPEGVer.Second:
                    data.samplingrate = m_SamplingRateTable[1][sampling];
                    break;
                case eMPEGVer.SecondHalf:
                    data.samplingrate = m_SamplingRateTable[2][sampling];
                    break;
                default:
                    Console.Write("MPEG ver None SamplingRateTable\n");
                    break;
            }
            Console.Write("sampling：{0}\n", data.samplingrate);

            //! Padding Bit.
            int padding_bit = data.frame_bit >> 9;
            padding_bit = padding_bit & 0x00000001;
            data.padding_bit = padding_bit;
            Console.Write("padding_bit：0x{0:x8}\n", data.padding_bit);

            //! Private Bit.
            int private_bit = data.frame_bit >> 8;
            private_bit = private_bit & 0x00000001;
            data.private_bit = private_bit;
            Console.Write("private_bit：0x{0:x8}\n", data.private_bit);

            //! Mode Bit.
            int mode_bit = data.frame_bit >> 6;
            mode_bit = mode_bit & 0x00000003;
            data.mode_bit = (eModeBit)mode_bit;
            Console.Write("mode_bit：0x{0:x8}\n", mode_bit);

            //! ModeExtension Bit.
            int modeEx_bit = data.frame_bit >> 4;
            modeEx_bit = modeEx_bit & 0x00000003;
            data.modeEx_bit = (eModeExtension)modeEx_bit;
            Console.Write("modeEx_bit：0x{0:x8}\n", modeEx_bit);

            //! Copyright.
            int copyright_bit = data.frame_bit >> 3;
            copyright_bit = copyright_bit & 0x00000001;
            data.copyright_bit = copyright_bit;
            Console.Write("copyright_bit：0x{0:x8}\n", data.copyright_bit);

            //! Original Check.
            int original_bit = data.frame_bit >> 2;
            original_bit = original_bit & 0x00000001;
            data.original_bit = original_bit;
            Console.Write("original_bit：0x{0:x8}\n", data.original_bit);

            //! Emphasis Bit.
            int emphasis_bit = data.frame_bit & 0x00000003;
            data.emphasis_bit = (eEmphasis)emphasis_bit;
            Console.Write("emphasis_bit：0x{0:x8}\n", emphasis_bit);

            //! Frame Size.
            switch (data.layer_bit)
            {
                case eLayerBit.Layer1:
                    data.frame_size = ((12 * 1000 * data.bitrate / data.samplingrate) + data.padding_bit) * 4;
                    break;
                case eLayerBit.Layer2:
                    data.frame_size = (144 * 1000 * data.bitrate / data.samplingrate) + data.padding_bit;
                    break;
                case eLayerBit.Layer3:
                    if (data.mpeg_ver == eMPEGVer.First)
                    {
                        data.frame_size = (144 * 1000 * data.bitrate / data.samplingrate) + data.padding_bit;
                    }
                    else
                    {
                        data.frame_size = (72 * 1000 * data.bitrate / data.samplingrate) + data.padding_bit;
                    }
                    break;
                default:
                    Console.Write("MPEG ver None Frame Size\n");
                    break;
            }
            Console.Write("frame_size：{0}\n", data.frame_size);

            byte[] frame_size_byte = new byte[data.frame_size-4];
            br.Read(frame_size_byte, 0, data.frame_size-4);

            return true;
#if false
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
#endif
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

        //! analyze frame header.
        private bool checkID3v1(ref BinaryReader br, ref sMP3Data data, BinaryReader tag_check)
        {
            byte[] tag_byte = new byte[3];
            tag_check.Read(tag_byte, 0, 3);
            string tag_name = Encoding.ASCII.GetString(tag_byte);
            Console.Write("tag：{0}\n", tag_name);

            //! "TAG" check.
            if (tag_name == "TAG")
            {
                return true;
            }

            return false;
        }
    }
}
