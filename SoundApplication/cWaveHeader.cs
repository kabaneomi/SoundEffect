using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace SoundApplication
{
    //  WAVEデータ
    public class cWavePcm
    {
        public int fs;              //  標本化周波数
        public int bits;            //  量子化精度
        public int length;          //  音データの長さ
        public short channel;		//	モノラルorステレオか判別する
        public double[] data;        //  モノラル：音データ
        public double[] LeftData;    //  ステレオ：音データ（Lチャンネル）
        public double[] RightData;   //  ステレオ：音データ（Rチャンネル）

        public cWavePcm()
        {
            fs = 0;
            bits = 0;
            length = 0;
        }
    };

    //  データ格納用構造体
    public class cWaveBox
    {
        public int ID_SIZE;
        public string riff_chunk_id;
        public int riff_chunk_size;
        public string riff_form_type;
        public string fmt_chunk_id;
        public int fmt_chunk_size;
        public short fmt_wave_format_type;
        public short fmt_channel;
        public int fmt_samples_per_sec;
        public int fmt_bytes_per_sec;
        public short fmt_block_size;
        public short fmt_bits_per_sample;
        public string data_chunk_id;
        public int data_chunk_size;
        public short data;

        public cWaveBox()
        {
            ID_SIZE = 4;
            riff_chunk_id = "none";
            riff_chunk_size = 0;
            riff_form_type = "none";
            fmt_chunk_id = "none";
            fmt_chunk_size = 0;
            fmt_wave_format_type = 0;
            fmt_channel = 0;
            fmt_samples_per_sec = 0;
            fmt_bytes_per_sec = 0;
            fmt_block_size = 0;
            fmt_bits_per_sample = 0;
            data_chunk_id = "none";
            data_chunk_size = 0;
            data = 0;
        }
    };

    class cWaveHeader
    {
        public void readWaveData(ref cWavePcm pcm, string filename)
        {
            //  ファイルオープン
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            cWaveBox box = new cWaveBox();
            Console.Write("waveファイル名：{0}\n", filename);

            //  RIFF取得
            byte[] riff_str = new byte[box.ID_SIZE];
            br.Read(riff_str, 0, box.ID_SIZE);
            box.riff_chunk_id = Encoding.ASCII.GetString(riff_str);
            Console.Write("riff_chunk_id：{0}\n", box.riff_chunk_id);

            //  サイズ取得
            byte[] riff_size = new byte[sizeof(int)];
            br.Read(riff_size, 0, sizeof(int));
            box.riff_chunk_size = BitConverter.ToInt32(riff_size, 0);
            Console.Write("riff_chunk_size：{0} {1}\n", box.riff_chunk_size, sizeof(int));

            //  riff_form_type取得
            byte[] riff_type_str = new byte[box.ID_SIZE];
            br.Read(riff_type_str, 0, box.ID_SIZE);
            box.riff_form_type = Encoding.ASCII.GetString(riff_type_str);
            Console.Write("riff_form_type：{0}\n", box.riff_form_type);

            //  fmt_chunk_id取得
            byte[] fmt_chunk_str = new byte[box.ID_SIZE];
            br.Read(fmt_chunk_str, 0, box.ID_SIZE);
            box.fmt_chunk_id = Encoding.ASCII.GetString(fmt_chunk_str);
            Console.Write("fmt_chunk_id：{0}\n", box.fmt_chunk_id);

            //  fmt_chunk_size取得
            byte[] fmt_chunk_size = new byte[sizeof(int)];
            br.Read(fmt_chunk_size, 0, sizeof(int));
            box.fmt_chunk_size = BitConverter.ToInt32(fmt_chunk_size, 0);
            Console.Write("fmt_chunk_size：{0} {1}\n", box.fmt_chunk_size, sizeof(int));

            //  fmt_wave_format_type取得
            byte[] fmt_wave_format_type = new byte[sizeof(short)];
            br.Read(fmt_wave_format_type, 0, sizeof(short));
            box.fmt_wave_format_type = BitConverter.ToInt16(fmt_wave_format_type, 0);
            Console.Write("fmt_wave_format_type：{0} {1}\n", box.fmt_wave_format_type, sizeof(short));

            //  fmt_channel取得
            byte[] fmt_channel = new byte[sizeof(short)];
            br.Read(fmt_channel, 0, sizeof(short));
            box.fmt_channel = BitConverter.ToInt16(fmt_channel, 0);
            Console.Write("fmt_channel：{0} {1}\n", box.fmt_channel, sizeof(short));

            //  fmt_samples_per_sec取得
            byte[] fmt_samples_per_sec = new byte[sizeof(int)];
            br.Read(fmt_samples_per_sec, 0, sizeof(int));
            box.fmt_samples_per_sec = BitConverter.ToInt32(fmt_samples_per_sec, 0);
            Console.Write("fmt_samples_per_sec：{0} {1}\n", box.fmt_samples_per_sec, sizeof(int));

            //  fmt_bytes_per_sec取得
            byte[] fmt_bytes_per_sec = new byte[sizeof(int)];
            br.Read(fmt_bytes_per_sec, 0, sizeof(int));
            box.fmt_bytes_per_sec = BitConverter.ToInt32(fmt_bytes_per_sec, 0);
            Console.Write("fmt_bytes_per_sec：{0} {1}\n", box.fmt_bytes_per_sec, sizeof(int));

            //  fmt_block_size取得
            byte[] fmt_block_size = new byte[sizeof(short)];
            br.Read(fmt_block_size, 0, sizeof(short));
            box.fmt_block_size = BitConverter.ToInt16(fmt_block_size, 0);
            Console.Write("fmt_block_size：{0} {1}\n", box.fmt_block_size, sizeof(short));

            //  fmt_bits_per_sample取得
            byte[] fmt_bits_per_sample = new byte[sizeof(short)];
            br.Read(fmt_bits_per_sample, 0, sizeof(short));
            box.fmt_bits_per_sample = BitConverter.ToInt16(fmt_bits_per_sample, 0);
            Console.Write("fmt_bits_per_sample：{0} {1}\n", box.fmt_bits_per_sample, sizeof(short));

            //  data_chunk_id取得
            byte[] data_chunk_id = new byte[box.ID_SIZE];
            br.Read(data_chunk_id, 0, box.ID_SIZE);
            box.data_chunk_id = Encoding.ASCII.GetString(data_chunk_id);
            Console.Write("data_chunk_id：{0}\n", box.data_chunk_id);

            //  data_chunk_size取得
            byte[] data_chunk_size = new byte[sizeof(int)];
            br.Read(data_chunk_size, 0, sizeof(int));
            box.data_chunk_size = BitConverter.ToInt32(data_chunk_size, 0);
            Console.Write("data_chunk_size：{0} {1}\n", box.data_chunk_size, sizeof(int));

            //  pcmに情報を入れる
            //  標本化周波数
            pcm.fs = box.fmt_samples_per_sec;
            //  量子化精度
            pcm.bits = box.fmt_bits_per_sample;
            //	モノラルかステレオか判別
            pcm.channel = box.fmt_channel;

            //	モノラルの場合
            if (pcm.channel == 1)
            {
                //  音データの長さ
                pcm.length = box.data_chunk_size / 2;
                //  音データ取得
                pcm.data = new double[pcm.length];
            }
            //	ステレオの場合
            else
            {
                //  音データの長さ
                pcm.length = box.data_chunk_size / 4;
                //  音データ取得
                pcm.LeftData = new double[pcm.length];
                pcm.RightData = new double[pcm.length];
            }

            for (int num = 0; num < pcm.length; num++)
            {
                if (pcm.channel == 1)
                {
                    readData(ref pcm.data[num], br);
                }
                else
                {
                    readData(ref pcm.LeftData[num], br);/* 音データ（Lチャンネル）の読み取り */
                    readData(ref pcm.RightData[num], br);/* 音データ（Rチャンネル）の読み取り */
                }
            }
            //  ファイルを閉じる
            fs.Close();
        }

        //	チャンネル読み取り
        private void readData(ref double pcm_data, BinaryReader br)
        {
            byte[] data = new byte[sizeof(short)];
            short s_data = 0;

            br.Read(data, 0, sizeof(short));
            s_data = BitConverter.ToInt16(data, 0);
            pcm_data = (double)s_data / 32768.0f;/* 音データを-1以上1未満の範囲に正規化する */
        }


        //	データ書き出し
        public void writeWaveData(ref cWavePcm pcm, string filename)
        {
            //  ファイルオープン
            FileStream fs = new FileStream(filename, FileMode.Create);
            Console.Write("ファイル名：{0}\n", filename);

            //  宣言
            string riff_chunk_ID = "RIFF";
            string riff_form_ID = "WAVE";
            string fmt_chunk_ID = "fmt ";
            string data_chunk_ID = "data";
            byte[] riff_name = new byte[4];
            byte[] riff_chunk_size = new byte[sizeof(int)];
            byte[] riff_form = new byte[4];
            byte[] fmt_chunk = new byte[4];
            byte[] fmt_chunk_size = new byte[sizeof(int)];
            byte[] fmt_wave_format_type = new byte[sizeof(short)];
            byte[] fmt_channel = new byte[sizeof(short)];
            byte[] fmt_samples_per_sec = new byte[sizeof(int)];
            byte[] fmt_bytes_per_sec = new byte[sizeof(int)];
            byte[] fmt_block_size = new byte[sizeof(short)];
            byte[] fmt_bits_per_sample = new byte[sizeof(short)];
            byte[] data_chunk = new byte[4];
            byte[] data_chunk_size = new byte[sizeof(int)];

            //  文字列設定
            riff_name = Encoding.GetEncoding("ASCII").GetBytes(riff_chunk_ID);
            //  文字列指定
            riff_form = Encoding.GetEncoding("ASCII").GetBytes(riff_form_ID);
            //  文字列指定
            fmt_chunk = Encoding.GetEncoding("ASCII").GetBytes(fmt_chunk_ID);
            //  文字列指定
            data_chunk = Encoding.GetEncoding("ASCII").GetBytes(data_chunk_ID);
            //  fmt_bits_per_sample
            fmt_bits_per_sample = BitConverter.GetBytes(pcm.bits);
            //  fmt_chunk_size
            fmt_chunk_size = BitConverter.GetBytes(16);
            //  fmt_wave_format_type
            fmt_wave_format_type = BitConverter.GetBytes(1);
            //  fmt_channel
            fmt_channel = BitConverter.GetBytes(pcm.channel);
            //  fmt_samples_per_sec
            fmt_samples_per_sec = BitConverter.GetBytes(pcm.fs);

            //  riff_chunk_size
            //  fmt_bytes_per_sec
            //  fmt_block_size
            //  data_chunk_size
            if (pcm.channel == 1)
            {
                riff_chunk_size = BitConverter.GetBytes(36 + pcm.length * 2);
                fmt_bytes_per_sec = BitConverter.GetBytes(pcm.fs * pcm.bits / 8);
                fmt_block_size = BitConverter.GetBytes(pcm.bits / 8);
                data_chunk_size = BitConverter.GetBytes(pcm.length * 2);
            }
            else
            {
                riff_chunk_size = BitConverter.GetBytes(36 + pcm.length * 4);
                fmt_bytes_per_sec = BitConverter.GetBytes(pcm.fs * pcm.bits / 8 * 2);
                fmt_block_size = BitConverter.GetBytes(pcm.bits / 8 * 2);
                data_chunk_size = BitConverter.GetBytes(pcm.length * 4);
            }

            //  書き込み
            fs.Write(riff_name, 0, 4);
            fs.Write(riff_chunk_size, 0, sizeof(int));
            fs.Write(riff_form, 0, 4);
            fs.Write(fmt_chunk, 0, 4);
            fs.Write(fmt_chunk_size, 0, sizeof(int));
            fs.Write(fmt_wave_format_type, 0, sizeof(short));
            fs.Write(fmt_channel, 0, sizeof(short));
            fs.Write(fmt_samples_per_sec, 0, sizeof(int));
            fs.Write(fmt_bytes_per_sec, 0, sizeof(int));
            fs.Write(fmt_block_size, 0, sizeof(short));
            fs.Write(fmt_bits_per_sample, 0, sizeof(short));
            fs.Write(data_chunk, 0, 4);
            fs.Write(data_chunk_size, 0, sizeof(int));

            //  data
            for (int n = 0; n < pcm.length; n++)
            {
                if (pcm.channel == 1)
                {
                    writeData(ref fs, pcm.data[n]);
                }
                else
                {
                    writeData(ref fs, pcm.LeftData[n]);
                    writeData(ref fs, pcm.RightData[n]);
                }
            }

            //  ファイルを閉じる
            fs.Close();
        }

        //	チャンネル書き込み
        private void writeData(ref FileStream fs, double data)
        {
            short dummy_set = 0;
            double set_data = 0.0;
            byte[] write_data_byte = new byte[sizeof(short)];

            set_data = (data + 1.0f) / 2.0f * 65536.0f;
            if (set_data > 65535.0f)
            {
                set_data = 65535.0f;    //  クリッピング
            }
            else if (set_data < 0.0f)
            {
                set_data = 0.0f;        //  クリッピング
            }
            set_data = (set_data + 0.5f) - 32768;
            dummy_set = (short)set_data;
            write_data_byte = BitConverter.GetBytes(dummy_set);
            fs.Write(write_data_byte, 0, sizeof(short));
        }
    }
}
