using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Configuration;

namespace TCP_Reader
{
    public delegate void GenericEvent<T>(T value);

    public class DataProvider : IDataProvider<double>
    {
        private readonly int? _samplesToRead;
        private readonly Header _header;
        private readonly string _hostName;
        private readonly int _port;
        private readonly int _retryTimes;
        private int attempts;
        private readonly TimeSpan _sleepTime;

        public DataProvider(string hostName, int port, int retryTimes, TimeSpan sleepTime)
        {
            this._hostName = hostName;
            this._port = port;
            this._header = new Header();
            this._retryTimes = retryTimes;
            this._sleepTime = sleepTime;
        }
        public DataProvider(string hostName, int port, int retryTimes, TimeSpan sleepTime, int samplesToRead) : this(hostName, port, retryTimes, sleepTime)
        {
            this._samplesToRead = samplesToRead;
        }

        /// <summary>
        /// Event raised when new value was received from TCP Writer
        /// </summary>
        public event GenericEvent<double> NextValue;

        /// <summary>
        /// Event raised when whole header was received from TCP Writer
        /// </summary>
        public event GenericEvent<Header> OnHeaderReceived;

        /// <summary>
        /// Starts listening for data from TCP Writer
        /// </summary>
        public void Start()
        {
            bool endTransmission = false;
            attempts = 0;
            while (endTransmission == false)
            {
                try
                {
                    using (var tcpClient = new TcpClient())
                    {
                        tcpClient.Connect(_hostName, _port);
                        using (var networkStream = tcpClient.GetStream())
                        {
                            var counter = 0;
                            var reader = new BinaryReader(networkStream);
                            while (true)
                            {
                                if (counter < _header.Size)
                                {
                                    _header.AddValue(counter, (int)reader.ReadUInt32());
                                    counter++;
                                }
                                // When whole header was received
                                else if (counter == _header.Size)
                                {
                                    OnHeaderReceived?.Invoke(_header);
                                    counter++;
                                }
                                // When sample was received from TCP Writer and we have no limit on received samples
                                else if (_samplesToRead == null)
                                {
                                    NextValue?.Invoke(reader.ReadDouble());
                                }
                                // When sample was received from TCP Writer and we have limit on received samples
                                else if (counter < _samplesToRead + _header.Size)
                                {
                                    NextValue?.Invoke(reader.ReadDouble());
                                    counter++; // We know that the counter won't overflow, since transmission will stop after reaching _samplesToRead + _header.Size samples
                                }
                                // When we have received desired amount of samples
                                else
                                {
                                    endTransmission = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    attempts++;
                    if (attempts > _retryTimes)
                    {
                        Console.WriteLine($"{e}\nCouldn't recover after {_retryTimes} attempts. Exiting.");
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"{e}\nRetry {attempts}/{_retryTimes}");
                    }
                }
            }
        }
    }
}
