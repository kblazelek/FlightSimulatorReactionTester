using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace TCP.Common
{
    public class TCPReader
    {
        private long _currentArrowState;

        /// <summary>
        /// Thread-safe arrow state indicator.
        /// Arrow not visible - 0
        /// Arrow visible - 1
        /// Long type is used instead of boolean, because it is compatible with Interlocked class
        /// </summary>
        public long CurrentArrowState
        {
            get
            {
                return Interlocked.Read(ref _currentArrowState);
            }
            set
            {
                Interlocked.Exchange(ref _currentArrowState, value);
                //File.AppendAllText(@"C:\Users\Traxx\Desktop\log.txt", $"CurrentArrowState={value}\n");
            }
        }

        private volatile bool stopAfterCurrentChunk = false;

        /// <summary>
        /// Number of values received in current chunk.
        /// </summary>
        private int chunkCounter = 0;

        /// <summary>
        /// Chunk size - data from TCP Writer is sent in chunks of size channels * samples per channel
        /// </summary>  
        private int chunkSize;

        /// <summary>
        /// Chunk contains data from TCP Writer and is of size <see cref="chunkSize"/> 
        /// </summary>
        private double[] chunk;

        /// <summary>
        /// Contains values of <see cref="CurrentArrowState"/> recorded each time new number is received from TCP Writer 
        /// </summary>
        private long[] arrowStates;

        /// <summary>
        /// Arrow state from last sample of previous chunk
        /// </summary>
        private long lastArrowStateFromPreviousChunk;

        /// <summary>
        /// Number of channels
        /// </summary>
        private int channels;

        /// <summary>
        /// Number of samples sent per channel
        /// </summary>
        private int samplesPerChannel;

        /// <summary>
        /// Provides data from TCP Writer
        /// </summary>
        private DataProvider dataProvider;

        /// <summary>
        /// Path where all the data will be stored in CSV format
        /// </summary>
        private readonly string _outputFilePath;

        private StringBuilder chunkBuilder = new StringBuilder();

        public TCPReader(string hostName, int port, int retryTimes, TimeSpan sleepTime, string outputFilePath)
        {
            _outputFilePath = outputFilePath;
            if (File.Exists(_outputFilePath))
            {
                File.Delete(_outputFilePath);
            }

            // Setup data provider that receives data from TCP Writer
            dataProvider = new DataProvider(hostName, port, retryTimes, sleepTime);

            // Subscribe to events triggered by data provider
            dataProvider.NextValue += DataProvider_NextValue;
            dataProvider.OnHeaderReceived += DataProvider_OnHeaderReceived;
        }

        public TCPReader(int samplesToRead, string hostName, int port, int retryTimes, TimeSpan sleepTime, string outputFilePath)
        {
            _outputFilePath = outputFilePath;
            if (File.Exists(_outputFilePath))
            {
                File.Delete(_outputFilePath);
            }

            // Setup data provider that received data from TCP Writer
            dataProvider = new DataProvider(hostName, port, retryTimes, sleepTime, samplesToRead);

            // Subscribe to events triggered by data provider
            dataProvider.NextValue += DataProvider_NextValue;
            dataProvider.OnHeaderReceived += DataProvider_OnHeaderReceived;
        }

        /// <summary>
        /// Starts listening for data from TCP Writer
        /// </summary>
        public void Start()
        {
            dataProvider.Start();
        }

        /// <summary>
        /// Stops listening for data from TCP Writer when full chunk is received and arrow state from last chunk is equal to 0
        /// </summary>
        public void StopAfterCurrentChunk()
        {
            stopAfterCurrentChunk = true;
        }

        /// <summary>
        /// Event handler called when <see cref="DataProvider"/> has received header from TCP Writer 
        /// </summary>
        /// <param name="header"></param>
        private void DataProvider_OnHeaderReceived(Header header)
        {
            channels = header.NumberOfChannels;
            samplesPerChannel = header.SamplesPerChunk;
            chunkSize = channels * samplesPerChannel;
            chunk = new double[chunkSize];
            arrowStates = new long[samplesPerChannel];
            lastArrowStateFromPreviousChunk = -1;

            // Print out header information in CSV format
            chunkBuilder.AppendLine(header.ToCSVString());
        }

        /// <summary>
        /// Event handler called when <see cref="DataProvider"/> has received data from TCP Writer (excluding header). 
        /// </summary>
        /// <param name="e">Value received from TCP Writer</param>
        private void DataProvider_NextValue(double e)
        {
            // Store received value
            chunk[chunkCounter] = e;

            // Record arrow states for first samplesPerChannel values in a chunk (samples from channel 0)
            if (chunkCounter < samplesPerChannel)
            {
                arrowStates[chunkCounter] = CurrentArrowState;
                //File.AppendAllText(@"C:\Users\Traxx\Desktop\log.txt", $"arrowStates[{chunkCounter}]={arrowStates[chunkCounter]}\n");
            }

            // Increment number of values received in current chunk
            chunkCounter++;

            // If last sample from chunk was received, then output whole chunk in CSV format
            if (chunkCounter == chunkSize)
            {
                for (int i = 0; i < samplesPerChannel; i++)
                {
                    for (int j = 0; j < channels; j++)
                    {
                        chunkBuilder.Append($"{chunk[j * samplesPerChannel + i]};");
                    }

                    // Determine output arrow value based on row of arrow values
                    // If arrow was invisible (0) and changed to (1) => value is 1
                    // If arrow was visible (1) and changed to (0), then button must have been clicked => value is 2
                    // If arrow was constant for each channel (0 or 1) => value is equal to that constant (0 or 1)
                    long outputArrowState;

                    long arrowStateFromCurrentSample = arrowStates[i];
                    long arrowStateFromPreviousSample = i == 0 ? lastArrowStateFromPreviousChunk : arrowStates[i - 1];
                    if (arrowStateFromCurrentSample == arrowStateFromPreviousSample)
                    {
                        outputArrowState = arrowStateFromCurrentSample;
                    }
                    else
                    {
                        if (lastArrowStateFromPreviousChunk != -1)
                        {
                            outputArrowState = arrowStateFromCurrentSample == 0 ? 2 : 1;
                        }
                        else
                        {
                            outputArrowState = arrowStateFromCurrentSample;
                        }
                    }
                    //File.AppendAllText(@"C:\Users\Traxx\Desktop\log.txt", $"outputArrowState={outputArrowState}\n");
                    chunkBuilder.Append($"{outputArrowState};");
                    chunkBuilder.Append(Environment.NewLine);
                }

                // Store last arrow value from current chunk
                lastArrowStateFromPreviousChunk = arrowStates[samplesPerChannel - 1];

                // Append chunk data to a file
                File.AppendAllText(_outputFilePath, chunkBuilder.ToString());

                // Clear data in StringBuilder
                chunkBuilder.Clear();

                // Reset number of values received in current chunk to 0 for next iteration
                chunkCounter = 0;

                // Stop listening for next chunks when requested
                if (stopAfterCurrentChunk && lastArrowStateFromPreviousChunk == 0)
                {
                    dataProvider.Stop();
                }
            }
        }
    }
}
