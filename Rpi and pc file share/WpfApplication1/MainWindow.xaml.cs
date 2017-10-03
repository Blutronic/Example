using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace WpfApplication1
{
    public partial class MainWindow : Window
    {
        public string SendingFilePath = string.Empty;
        private const int BufferSize = 1024;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSend_Click_1(object sender, RoutedEventArgs e)
        {
            SendingFilePath = "C:\\Users\\Riaan.VanTonder\\Desktop\\Pi scripts\\WebcamView.py";
            //"C:\\Users\\Riaan.VanTonder\\Desktop\\Pi scripts\\basicWebcam.py";
            //C:\Users\Riaan.VanTonder\Desktop\Pi scripts\basicWebcam

            if (SendingFilePath != string.Empty)
            {
                SendTCP(SendingFilePath, txtIP.Text, Int32.Parse(txtPort.Text));
            }
            else
                MessageBox.Show("Select a file", "Warning");
        }

        public void SendTCP(string M, string IPA, Int32 PortN)
        {
            byte[] SendingBuffer = null;
            TcpClient client = null;
            lblStatus.Content = "";
            NetworkStream netstream = null;
            try
            {
                client = new TcpClient(IPA, PortN);
                lblStatus.Content = "Connected to the Server...\n";
                netstream = client.GetStream();
                FileStream Fs = new FileStream(M, FileMode.Open, FileAccess.Read);
                int NoOfPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Fs.Length) / Convert.ToDouble(BufferSize)));
                int TotalLength = (int)Fs.Length, CurrentPacketLength, counter = 0;
                for (int i = 0; i < NoOfPackets; i++)
                {
                    if (TotalLength > BufferSize)
                    {
                        CurrentPacketLength = BufferSize;
                        TotalLength = TotalLength - CurrentPacketLength;
                    }
                    else
                        CurrentPacketLength = TotalLength;
                    SendingBuffer = new byte[CurrentPacketLength];
                    Fs.Read(SendingBuffer, 0, CurrentPacketLength);
                    netstream.Write(SendingBuffer, 0, (int)SendingBuffer.Length);
                }
                
                lblStatus.Content=lblStatus.Content+"Sent "+Fs.Length.ToString()+" bytes to the server";
                Fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                netstream.Close();
                client.Close();

            }
        }
    }
}
