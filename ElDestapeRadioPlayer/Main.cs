using ElDestapeRadioPlayer;
using NAudio.Wave;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ElDestapeRadio
{
    public partial class Main : Form
    {

        public Main()
        {
            InitializeComponent();
            
        }

        static Boolean isPlaying = false;
        static Boolean isInterrupted = false;
        static int pId = -1;
        Thread playerThread { get; set; }
        private void btnPlay_Click(object sender, EventArgs e)
        {
            try
            {
                Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Atención", "Error: " + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Play()
        {
            if (!isPlaying)
            {
                playerThread = new Thread(() => playRadio("https://ipanel.instream.audio/8004/stream"));
                isPlaying = true;
                isInterrupted = false;
                playerThread.Name = "Player";
                playerThread.IsBackground = true;
                playerThread.Start();

            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Atención", "Error: " + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Stop()
        {
            if (playerThread != null && !isInterrupted)
            {
                playerThread.Interrupt();
                isPlaying = false;
                isInterrupted = true;
            }

            isPlaying = false;
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {

            try
            {
                if (playerThread != null && !isInterrupted)
                {
                    playerThread.Interrupt();
                    isPlaying = false;
                    isInterrupted = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Atención", "Error: " + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Main_Load(object sender, EventArgs e)
        {
            unmute();
            lblVol.Text = "";
            pId = getProcessId();

            tbVolume.Value = 30;
        }

        private int getProcessId()
        {
            int pID = 0;

            foreach (Process process in Process.GetProcesses())
            {
                if (process.ProcessName == Application.ProductName)
                {
                    pID = process.Id;
                    break;
                }
            }
            return pID;
        }
        private void chkMute_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMute.Checked)
                mute();
            else
                unmute();
        }

        [DllImport("winmm.dll")]
        private static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        //mute application
        private void mute()
        {
            //int NewVolume = 0; //set 0 to unmute
            //uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));
            //waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);
            VolumeMixer.SetApplicationVolume(pId, 0);
        }


        //unmute application
        private void unmute()
        {
            {
                //int NewVolume = 65535; //set 65535 to unmute
                //uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));
                //waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);
                int.TryParse(lblVol.Text, out int level);
                VolumeMixer.SetApplicationVolume(pId, level);
            }

        }





        public void playRadio(string urlRadio)
        {
            try
            {
                string url = urlRadio;
                using (MediaFoundationReader mf = new MediaFoundationReader(url))
                using (WaveOutEvent wo = new WaveOutEvent())
                {
                    wo.Init(mf);
                    wo.Play();
                    while (wo.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (System.Threading.ThreadInterruptedException)
            {
                playerThread.Join();
            }
            catch (Exception ex)
            {
                throw new Exception("playRadio: " + ex.Message);
            }
        }


        private void tbVolume_Scroll(object sender, EventArgs e)
        {
            if (pId > 0)
            {
                int value = ((TrackBar)sender).Value;
                VolumeMixer.SetApplicationVolume(pId, value);
                lblVol.Text = value.ToString();
                lblVol.Invalidate();
            }
        }

        private void tbVolume_ValueChanged(object sender, EventArgs e)
        {
            int value = ((TrackBar)sender).Value;
            VolumeMixer.SetApplicationVolume(pId, value);
            lblVol.Text = value.ToString();
            lblVol.Invalidate();
        }

        private void lbElDestape_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process process = new Process();

            process.StartInfo.FileName = "http://www.eldestape.com";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            process.StartInfo.UseShellExecute = true;
            process.Start();

        }

        private void reproducirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Atención", "Error: " + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void detenerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Atención", "Error: " + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void muteToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (muteToolStripMenuItem.Checked)
                mute();
            else
                unmute();
        }

        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About aboutwindow = new About();
            aboutwindow.ShowDialog(this);

        }

        private void lbAbout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            About aboutwindow = new About();
            aboutwindow.ShowDialog(this);
        }

        private void Main_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
            }
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stop();
            Application.Exit();
        }
    }
}