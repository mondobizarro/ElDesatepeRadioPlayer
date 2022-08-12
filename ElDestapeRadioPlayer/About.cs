using NAudio.Wave;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ElDestapeRadio
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        WaveOut waveOut = new WaveOut();
        private void Form1_Load(object sender, EventArgs e)
        {
            byte[] fileByte = ElDestapeRadioPlayer.Properties.Resources.intro;
            Stream stream = new MemoryStream(fileByte);
            Mp3FileReader reader = new NAudio.Wave.Mp3FileReader(stream);

            waveOut.Init(reader);
            waveOut.Play();
        }

        private void lbContacto_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process process = new Process();

            process.StartInfo.FileName = "mailto:ThePeronistDeveloper@outlook.com";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            process.StartInfo.UseShellExecute = true;
            process.Start();
        }

        private void About_FormClosing(object sender, FormClosingEventArgs e)
        {
            waveOut.Stop();
        }
    }
}
