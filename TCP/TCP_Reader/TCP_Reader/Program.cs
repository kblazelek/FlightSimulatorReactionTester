using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;

namespace TCP_Reader
{
    class Program
    {
        static void Main(params string[] args)
        {
            // Read settings from App.config
            string samplesToRead = ConfigurationManager.AppSettings["SamplesToRead"];
            string hostName = ConfigurationManager.AppSettings["Hostname"];
            int port = int.Parse(ConfigurationManager.AppSettings["Port"]);
            int retryTimes = int.Parse(ConfigurationManager.AppSettings["RetryTimes"]);
            TimeSpan sleepTime = TimeSpan.FromSeconds(int.Parse(ConfigurationManager.AppSettings["SleepSeconds"]));
            string outputFile = ConfigurationManager.AppSettings["OutputFile"];
            TCPReader tcpReader;
            if (samplesToRead != null && samplesToRead != "0")
            {
                int tempSamplesToRead = int.Parse(samplesToRead);
                tcpReader = new TCPReader(tempSamplesToRead, hostName, port, retryTimes, sleepTime, outputFile);
            }
            else
            {
                tcpReader = new TCPReader(hostName, port, retryTimes, sleepTime, outputFile);
            }
            // Start background thread that simulates changing of arrow state
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                while (true)
                {
                    tcpReader.CurrentArrowState = 0;
                    Thread.Sleep(1000);
                    tcpReader.CurrentArrowState = 1;
                    Thread.Sleep(1000);
                }
            }).Start();
            tcpReader.Start();
            Console.ReadKey();
        }
    }
}
