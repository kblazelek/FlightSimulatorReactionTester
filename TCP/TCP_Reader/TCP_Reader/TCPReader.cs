using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace TCP_Reader
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
            }
        }

        /// <summary>
        /// Counter of received numbers from TCP Writer (excluding header)
        /// </summary>
        private int counter = 0;

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
            if(File.Exists(_outputFilePath))
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
        /// Event handler called when <see cref="DataProvider"/> has received header from TCP Writer 
        /// </summary>
        /// <param name="header"></param>
        private void DataProvider_OnHeaderReceived(Header header)
        {
            channels = header.NumberOfChannels;
            samplesPerChannel = header.SamplesPerChunk;
            chunkSize = channels * samplesPerChannel;
            chunk = new double[chunkSize];
            arrowStates = new long[chunkSize];
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
                        chunkBuilder.Append($"{chunk[j * samplesPerChannel + i]};");
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
                        outputArrowState = arrowStates[i * channels];
                        for (int k = 1; k < channels; k++)
                        {
                            if (arrowStates[i * channels + k] != outputArrowState)
                            {
                                outputArrowState = arrowStates[i * channels + k] == 0 ? 2 : 1;
                                break;
                            }
                        }
                    }
                    chunkBuilder.Append($"{outputArrowState};");
                    chunkBuilder.Append(Environment.NewLine);
                }

                // Store last arrow value from current chunk
                lastArrowStateFromPreviousChunk = arrowStates[channels * samplesPerChannel - 1];
                File.AppendAllText(_outputFilePath, chunkBuilder.ToString());
                chunkBuilder.Clear();
            }

            // Increment number of values received from TCP Writer
            counter++;
        }
    }
}
