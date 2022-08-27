using NAudio.Wave;
using NAudio.Codecs;
using NAudio.CoreAudioApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace WindowsFormsApp1
{


    public partial class Form1 : Form
    {
        private WaveInEvent waveIn; 
        private int _MaxValue = 0;
        private int _keydown = 0;
        private object _lockObj = new object();
        public Form1()
        {
            InitializeComponent();
            for (var i = 0; i < WaveIn.DeviceCount; i++)
            {
                comboBox1.Items.Add(WaveIn.GetCapabilities(i).ProductName);
            }
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;    // 押せない状態に
            timer1.Enabled = true;
            waveIn = new WaveInEvent()
            {
                DeviceNumber = comboBox1.SelectedIndex,
            };
            waveIn.DataAvailable += OnDataAvailable;
            waveIn.WaveFormat = new WaveFormat(48000, 2);


            waveIn.StartRecording();


        }




        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void OnDataAvailable(object sender, WaveInEventArgs args)
        {
            lock (_lockObj)
            {
                float max = 0;
                // interpret as 16 bit audio
                for (int index = 0; index < args.BytesRecorded; index += 2)
                {
                    short sample = (short)((args.Buffer[index + 1] << 8) |
                                            args.Buffer[index + 0]);
                    // to floating point
                    var sample32 = sample / 32768f;
                    // absolute value 
                    if (sample32 < 0) sample32 = -sample32;
                    // is this the max value?
                    if (sample32 > max) max = sample32;
                }
                _MaxValue = (int)(100 * max);

            }

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            this.progressBar1.Value = _MaxValue;

            if (_MaxValue > 50)
            {
               
                    win32api.keybd_event((byte)0x4D, 0, 0, (UIntPtr)0);
                
            }
            else
            {
                
                    win32api.keybd_event((byte)0x4D, 0, 2, (UIntPtr)0); ;
                

            }
        
        



        }

        private void button2_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{m down}");
        }
    }
    public class win32api
    {
        [DllImport("user32.dll")]
        public static extern uint keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
    }

}
