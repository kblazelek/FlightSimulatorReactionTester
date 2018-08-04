using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;

namespace TCP_Reader
{
    class Program
    {
        private static long _currentArrowState;

        /// <summary>
        /// Thread-safe arrow state indicator.
        /// Arrow not visible - 0
        /// Arrow visible - 1
        /// Long type is used instead of boolean, because it is compatible with Interlocked class
        /// </summary>
        public static long CurrentArrowState
        {
            get
            {
                return Interlocked.Read(ref _currentArrowState);
            }
            set
            {
                Interlocked.Exchange(ref _currentArrowState, value);
            }
        }

        /// <summary>
        /// Counter of received numbers from TCP Writer (excluding header)
        /// </summary>
        private static int counter = 0;

        /// <summary>
        /// Chunk size - data from TCP Writer is sent in chunks of size channels * samples per channel
        /// </summary>
        private static int chunkSize;

        /// <summary>
        /// Chunk contains data from TCP Writer and is of size <see cref="chunkSize"/> 
        /// </summary>
        private static double[] chunk;

        /// <summary>
        /// Contains values of <see cref="CurrentArrowState"/> recorded each time new number is received from TCP Writer 
        /// </summary>
        private static long[] arrowStates;

        /// <summary>
        /// Arrow state from last sample of previous chunk
        /// </summary>
        private static long lastArrowStateFromPreviousChunk;

        /// <summary>
        /// Number of channels
        /// </summary>
        private static int channels;

        /// <summary>
        /// Number of samples sent per channel
        /// </summary>
        private static int samplesPerChannel;
        static void Main(params string[] args)
        {
            // Read settings from App.config
            string tempSamplesToRead = ConfigurationManager.AppSettings["SamplesToRead"];
            string hostName = ConfigurationManager.AppSettings["Hostname"];
            int port = int.Parse(ConfigurationManager.AppSettings["Port"]);
            int retryTimes = int.Parse(ConfigurationManager.AppSettings["RetryTimes"]);
            TimeSpan sleepTime = TimeSpan.FromSeconds(int.Parse(ConfigurationManager.AppSettings["SleepSeconds"]));

            // Setup data provider that received data from TCP Writer
            DataProvider dataProvider;
            if (tempSamplesToRead != null && tempSamplesToRead != "0")
            {
                int samplesToRead = int.Parse(tempSamplesToRead);
                dataProvider = new DataProvider(hostName, port, retryTimes, sleepTime, samplesToRead);
            }
            else
            {
                dataProvider = new DataProvider(hostName, port, retryTimes, sleepTime);
            }

            // Subscribe to events triggered by data provider
            dataProvider.NextValue += DataProvider_NextValue;
            dataProvider.OnHeaderReceived += DataProvider_OnHeaderReceived;

            // Start listening for data from TCP Writer
            dataProvider.Start();
            Console.ReadKey();
        }

        /// <summary>
        /// Event handler called when <see cref="DataProvider"/> has received header from TCP Writer 
        /// </summary>
        /// <param name="header"></param>
        private static void DataProvider_OnHeaderReceived(Header header)
        {
            channels = header.NumberOfChannels;
            samplesPerChannel = header.SamplesPerChunk;
            chunkSize = channels * samplesPerChannel;
            chunk = new double[chunkSize];
            arrowStates = new long[chunkSize];
            lastArrowStateFromPreviousChunk = -1;

            // Print out header information in CSV format
            Console.WriteLine(header.ToCSVString());

            // Start background thread that simulates changing of arrow state
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                while (true)
                {
                    CurrentArrowState = 0;
                    Thread.Sleep(1000);
                    CurrentArrowState = 1;
                    Thread.Sleep(1000);
                }
            }).Start();
        }

        /// <summary>
        /// Event handler called when <see cref="DataProvider"/> has received data from TCP Writer (excluding header). 
        /// </summary>
        /// <param name="e">Value received from TCP Writer</param>
        private static void DataProvider_NextValue(double e)
        {
            long outputArrowState;

            // Store received value
            chunk[counter % chunkSize] = e;

            // Store current arrow state
            arrowStates[counter % chunkSize] = CurrentArrowState;

            // If last sample from chunk was received, then output whole chunk in CSV format
            if ((counter + 1) % chunkSize == 0)
            {
                for (int i = 0; i < samplesPerChannel; i++)
                {
                    for (int j = 0; j < channels; j++)
                    {
                        Console.Write($"{chunk[j * samplesPerChannel + i]};");
                    }
                    // Determine arrow value based on row of arrow values
                    // If arrow was invisible (0) and changed to (1) => value is 1
                    // If arrow was visible (1) and changed to (0), then button must have been clicked => value is 2
                    // If arrow was constant for each channel (0 or 1) => value is equal to that constant (0 or 1)

                    // If first arrow in current chunk is different from last arrow from previous chunk
                    if (i == 0 && lastArrowStateFromPreviousChunk != -1 && (arrowStates[0] != lastArrowStateFromPreviousChunk))
                    {
                        outputArrowState = lastArrowStateFromPreviousChunk == 0 ? 1 : 2;
                    }
                    else
                    {
                        outputArrowState = arrowStates[i * samplesPerChannel];
                        for (int k = 1; k < channels; k++)
                        {
                            if (arrowStates[i * samplesPerChannel + k] != outputArrowState)
                            {
                                outputArrowState = arrowStates[i * samplesPerChannel + k];
                                break;
                            }
                        }
                    }
                    Console.Write($"{outputArrowState};");
                    Console.Write(Environment.NewLine);
                }
            }
            // Store last arrow value from current chunk
            lastArrowStateFromPreviousChunk = arrowStates[channels * samplesPerChannel - 1];

            // Increment number of values received from TCP Writer
            counter++;
        }
    }

}
