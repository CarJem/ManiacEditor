using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio;
using NAudio.Vorbis;
using NAudio.Wave;

namespace ManiacEditor.Interfaces
{
    public partial class SoundLooper : Form
    {
        string currentSoundFile = "";
        int currentLoopPosition = 0;
        int currentStartPosition = 0;
        VorbisWaveReader audioStream = null;
        WaveOut waveOut = null;
        public SoundLooper()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string Result = null;
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Sound File|*.ogg";
            if (open.ShowDialog() != DialogResult.Cancel)
            {
                Result = open.FileName;
                currentSoundFile = open.FileName;
            }

            if (Result != null)
            {
                try
                {
                    audioStream = new NAudio.Vorbis.VorbisWaveReader(currentSoundFile);
                    textBox2.Text = audioStream.Length.ToString();
                    textBox1.Text = Result;
                }
                catch
                {
                    MessageBox.Show("Not a Valid File!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    currentSoundFile = "";
                }
            }
        }

        private void buttonStartStop_Click(object sender, EventArgs e)
        {
            if (waveOut == null)
            {
                audioStream.Position = currentStartPosition;
                waveOut = new WaveOut();
                waveOut.Init(audioStream);
                waveOut.Play();
            }
            else
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            currentLoopPosition = (int)numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            currentStartPosition = (int)numericUpDown2.Value;
        }
    }
    }
