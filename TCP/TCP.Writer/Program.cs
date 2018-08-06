using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCP.Writer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // We set our local IP address as server's address
                TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 5678);
                server.Start();
                int channels = 14;
                int samplesPerChannel = 1;
                while (true)
                {
                    // Wait for client to connect
                    TcpClient client = server.AcceptTcpClient();

                    // Get NetworkStream in order to send/receive messages
                    NetworkStream ns = client.GetStream();

                    // Send header information
                    byte[] b = BitConverter.GetBytes(16777216);
                    ns.Write(b, 0, b.Length);
                    b = BitConverter.GetBytes(16777216);
                    ns.Write(b, 0, b.Length);
                    b = BitConverter.GetBytes(0);
                    ns.Write(b, 0, b.Length);
                    b = BitConverter.GetBytes(channels);
                    ns.Write(b, 0, b.Length);
                    b = BitConverter.GetBytes(samplesPerChannel);
                    ns.Write(b, 0, b.Length);
                    b = BitConverter.GetBytes(0);
                    ns.Write(b, 0, b.Length);
                    b = BitConverter.GetBytes(0);
                    ns.Write(b, 0, b.Length);
                    b = BitConverter.GetBytes(0);
                    ns.Write(b, 0, b.Length);
                    byte[] sampleBytes;
                    double sampleValue = 0;
                    while (client.Connected)  // while the client is connected, we look for incoming messages
                    {
                        // Send chunk of data
                        for (int i = 0; i < channels; i++)
                        {
                            for (int j = 0; j < samplesPerChannel; j++)
                            {
                                // Generate sample value
                                sampleValue = i * 1000 + j;

                                // Convert sample value to bytes
                                sampleBytes = BitConverter.GetBytes(sampleValue);

                                // Send the sample bytes to TCP Reader
                                ns.Write(sampleBytes, 0, sampleBytes.Length);

                                // Sleep between sending next sample
                                Thread.Sleep(8);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
