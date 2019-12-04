using System;
using System.Windows.Forms;
using NAudio.Vorbis;
using NAudio.Wave;

namespace ManiacEditor.Interfaces
{
    public partial class SoundLooper : Form
	{
		string currentSoundFile = "";
		UInt32 currentLoopPosition = 0;
		UInt32 currentStartPosition = 0;
		VorbisWaveReader audioStream = null;
		int loopOffset = 16;
		WaveOut waveOut = null;
		System.Windows.Forms.Timer updatePositionTimer;
		public SoundLooper()
		{
			InitializeComponent();
			updatePositionTimer = new System.Windows.Forms.Timer();
			updatePositionTimer.Interval = 1;
			updatePositionTimer.Tick += UpdatePositionTimer_Tick;
			updatePositionTimer.Start();

		}

		private void UpdatePositionTimer_Tick(object sender, EventArgs e)
		{
			if (waveOut != null && audioStream != null)
			{
				if (audioStream.Position + 8 >= audioStream.Length)
				{
					waveOut.DesiredLatency = 0;
					audioStream.Position = currentLoopPosition * 8;
				}
				if (audioStream.Position != 0) textBox3.Text = (audioStream.Position / 8).ToString();
				else textBox3.Text = "0";


			}
			else
			{
				textBox3.Text = "N/A";
			}
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
					textBox2.Text = (audioStream.Length / 8).ToString();
					textBox1.Text = Result;
				}
				catch
				{
					System.Windows.MessageBox.Show("Not a Valid File!", "ERROR", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
					currentSoundFile = "";
				}
			}
		}

		private void buttonStartStop_Click(object sender, EventArgs e)
		{
			if (waveOut == null && audioStream != null)
			{
				
				audioStream.Position = currentStartPosition * 8;
				waveOut = new WaveOut();
				waveOut.Init(audioStream);
				waveOut.Play();
				waveOut.PlaybackStopped += WaveOut_PlaybackStopped;
			}
		}

		private void WaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
		{

		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			currentLoopPosition = (UInt32)numericUpDown1.Value;
		}

		private void numericUpDown2_ValueChanged(object sender, EventArgs e)
		{
			currentStartPosition = (UInt32)numericUpDown2.Value;
		}

		private void button3_Click(object sender, EventArgs e)
		{
			if (waveOut != null)
			{
				if (waveOut.PlaybackState != PlaybackState.Paused)
				{
					waveOut.Pause();
				}
				else if (waveOut.PlaybackState != PlaybackState.Playing)
				{
					waveOut.Play();
				}
			}
		}

		private void button4_Click(object sender, EventArgs e)
		{
			if (audioStream != null && waveOut != null)
			{
					waveOut.Stop();
					waveOut.Dispose();
					waveOut = null;		
			}
		}

		private void label6_Click(object sender, EventArgs e)
		{

		}

		private void numericUpDown3_ValueChanged(object sender, EventArgs e)
		{
			loopOffset = (int)numericUpDown3.Value;
		}
	}
}	
