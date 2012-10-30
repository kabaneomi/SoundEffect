using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using AxWMPLib;

namespace SoundApplication
{
    public partial class Form1 : Form
    {
        //OpenFileDialogクラスのインスタンスを作成
        OpenFileDialog ofd = new OpenFileDialog();
        SaveFileDialog sfd = new SaveFileDialog();
        System.Windows.Media.MediaPlayer mediaPlayer = new System.Windows.Media.MediaPlayer();
        AxWMPLib.AxWindowsMediaPlayer test = new AxWMPLib.AxWindowsMediaPlayer();
        cWavePcm wave_pcm = new cWavePcm();
        cWavePcm reverve_pcm = new cWavePcm();
        cMP3Data mp3_data = new cMP3Data();
        cSoundEffectResound resound = new cSoundEffectResound();

        public Form1()
        {
            InitializeComponent();
        }

        //  ファイルオープン
        private void MediaOpen_Click(object sender, EventArgs e)
        {
            //はじめのファイル名を指定する
            //はじめに「ファイル名」で表示される文字列を指定する
            ofd.FileName = "";
            //はじめに表示されるフォルダを指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            ofd.InitialDirectory = "";
            //[ファイルの種類]に表示される選択肢を指定する
            //指定しないとすべてのファイルが表示される
            ofd.Filter = "WAVEファイル(*.wav)|*.wav|mp3ファイル(*.mp3)|*.mp3";
            //[ファイルの種類]ではじめに
            //「すべてのファイル」が選択されているようにする
            ofd.FilterIndex = 1;
            //タイトルを設定する
            ofd.Title = "開くファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;
            //存在しないファイルの名前が指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            ofd.CheckFileExists = true;
            //存在しないパスが指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            ofd.CheckPathExists = true;

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                mediaPlayer.Open(new System.Uri(ofd.FileName));
                string test = Path.GetExtension(ofd.FileName);
                if (".wav" == Path.GetExtension(ofd.FileName))
                {
                    cWaveHeader wave = new cWaveHeader();
                    wave.readWaveData(ref wave_pcm, ofd.FileName);
                }
                else if (".mp3" == Path.GetExtension(ofd.FileName))
                {
                    cMP3Header mp3 = new cMP3Header();
                    mp3.readMp3Data(ref mp3_data, ofd.FileName);
                }
            }
        }

        //  再生
        private void MediaPlay_Click(object sender, EventArgs e)
        {
            if (MediaPlay.Text.Contains("再生"))
            {
                mediaPlayer.Play();
                MediaPlay.Text = "一時停止";
            }
            else if (MediaPlay.Text.Contains("一時停止"))
            {
                mediaPlayer.Pause();
                MediaPlay.Text = "再生";
            }
        }

        //  停止
        private void MediaStop_Click(object sender, EventArgs e)
        {
            mediaPlayer.Stop();
        }

        //  リピート
        private void MediaRepeat_CheckedChanged(object sender, EventArgs e)
        {
            if (MediaRepeat.Checked)
            {
            }
            else
            {
            }
        }

        //  減衰率
        private void Gensui_TextChanged(object sender, EventArgs e)
        {
            Console.Write("Gensui.Text：{0}\n", Gensui.Text);
            if (Gensui.Text != "")
            {
                resound.setGensui(double.Parse(Gensui.Text));
            }
        }

        //  遅延時間
        private void LateTime_TextChanged(object sender, EventArgs e)
        {
            Console.Write("LateTime.Text：{0}\n", LateTime.Text);
            if (LateTime.Text != "")
            {
                resound.setLate(double.Parse(LateTime.Text));
            }
        }

        //  繰り返し回数
        private void Repeat_TextChanged(object sender, EventArgs e)
        {
            Console.Write("Repeat.Text：{0}\n", Repeat.Text);
            if (Repeat.Text != "")
            {
                resound.setRepeat(int.Parse(Repeat.Text));
            }
        }

        //  リバーブを行う
        private void Reverve_Click(object sender, EventArgs e)
        {
            resound.revervePcm(ref reverve_pcm, wave_pcm);
            //はじめのファイル名を指定する
            sfd.FileName = "reverve.wav";
            //はじめに表示されるフォルダを指定する
            sfd.InitialDirectory = @"C:\";
            //[ファイルの種類]に表示される選択肢を指定する
            sfd.Filter = "WAVEファイル(*.wav)|*.wav";
            //[ファイルの種類]ではじめに
            //「すべてのファイル」が選択されているようにする
            sfd.FilterIndex = 2;
            //タイトルを設定する
            sfd.Title = "保存先のファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            sfd.RestoreDirectory = true;
            //既に存在するファイル名を指定したとき警告する
            //デフォルトでTrueなので指定する必要はない
            sfd.OverwritePrompt = true;
            //存在しないパスが指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            sfd.CheckPathExists = true;
            if( sfd.ShowDialog() == DialogResult.OK )
            {
                cWaveHeader wave = new cWaveHeader();
                wave.writeWaveData(ref reverve_pcm, sfd.FileName);
            }
        }

        //  リバーブを行ったデータを再生する
        private void PlayReverve_Click(object sender, EventArgs e)
        {
            
        }

        //  ステレオ吐き出しテスト
        private void wavewrite_Click(object sender, EventArgs e)
        {
            //はじめのファイル名を指定する
            sfd.FileName = "stereo_test.wav";
            //はじめに表示されるフォルダを指定する
            sfd.InitialDirectory = @"C:\";
            //[ファイルの種類]に表示される選択肢を指定する
            sfd.Filter = "WAVEファイル(*.wav)|*.wav";
            //[ファイルの種類]ではじめに
            //「すべてのファイル」が選択されているようにする
            sfd.FilterIndex = 2;
            //タイトルを設定する
            sfd.Title = "保存先のファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            sfd.RestoreDirectory = true;
            //既に存在するファイル名を指定したとき警告する
            //デフォルトでTrueなので指定する必要はない
            sfd.OverwritePrompt = true;
            //存在しないパスが指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            sfd.CheckPathExists = true;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                cWaveHeader wave = new cWaveHeader();
                wave.writeWaveData(ref wave_pcm, sfd.FileName);
            }
        }
    }
}
