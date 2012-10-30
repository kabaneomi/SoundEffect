using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SoundApplication
{
    class cSoundEffectResound
    {
        double m_Gensui = 0.5;    /* 減衰率 */
        double m_Late = 0.375;    /* 遅延時間 */
        int m_Repeat = 2;         /* 繰り返し回数 */

        public void setGensui(double gensui)
        {
            m_Gensui = gensui;
        }

        public void setLate(double late)
        {
            m_Late = late;
        }

        public void setRepeat(int repeat)
        {
            m_Repeat = repeat;
        }

        public void revervePcm(ref cWavePcm reverve_pcm, cWavePcm pcm)
        {
            reverve_pcm.fs = pcm.fs; /* 標本化周波数 */
            reverve_pcm.bits = pcm.bits; /* 量子化精度 */
            reverve_pcm.length = pcm.length; /* 音データの長さ */
            reverve_pcm.channel = pcm.channel; /* チャンネル設定 */
            if (pcm.channel == 1)
            {
                reverve_pcm.data = new double[reverve_pcm.length];
            }
            else
            {
                reverve_pcm.LeftData = new double[reverve_pcm.length];
                reverve_pcm.RightData = new double[reverve_pcm.length];
            }
            double a = m_Gensui;
            double d = reverve_pcm.fs * m_Late;
            int repeat = m_Repeat;

            for (int n = 0; n < reverve_pcm.length; n++)
            {
                if (pcm.channel == 1)
                {
                    reverve_pcm.data[n] = pcm.data[n];
                }
                else
                {
                    reverve_pcm.LeftData[n] = pcm.LeftData[n];
                    reverve_pcm.RightData[n] = pcm.RightData[n];
                }

                for ( int i = 1; i <= repeat; i++)
                {
                    int m = (int)((double)n - (double)i * d);
                    if (m >= 0)
                    {
                        if (pcm.channel == 1)
                        {
                            reverve_pcm.data[n] += Math.Pow(a, (double)i) * pcm.data[m]; /* 過去の音データをミックスする */
                        }
                        else
                        {
                            reverve_pcm.LeftData[n] += Math.Pow(a, (double)i) * pcm.LeftData[m]; /* 過去の音データをミックスする */
                            reverve_pcm.RightData[n] += Math.Pow(a, (double)i) * pcm.RightData[m]; /* 過去の音データをミックスする */
                        }
                    }
                }
            }
        }
    }
}
