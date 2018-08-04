using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Reader
{
    /// <summary>
    /// Class that stores header information from TCP Writer
    /// </summary>
    public class Header
    {
        public int Size { get; } = 8;
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Format version\t{this.FormatVersion}");
            sb.AppendLine($"Endianness\t{this.Endianness}");
            sb.AppendLine($"Sampling frequency\t{this.SamplingFrequency}");
            sb.AppendLine($"Number of channels\t{this.NumberOfChannels}");
            sb.AppendLine($"Samples per chunk\t{this.SamplesPerChunk}");
            sb.AppendLine($"Reserved0\t{this.Reserved0}");
            sb.AppendLine($"Reserved1\t{this.Reserved1}");
            sb.AppendLine($"Reserved2\t{this.Reserved2}");
            return sb.ToString();
        }

        public string ToCSVString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Format version;{this.FormatVersion}");
            sb.AppendLine($"Endianness;{this.Endianness}");
            sb.AppendLine($"Sampling frequency;{this.SamplingFrequency}");
            sb.AppendLine($"Number of channels;{this.NumberOfChannels}");
            sb.AppendLine($"Samples per chunk;{this.SamplesPerChunk}");
            sb.AppendLine($"Reserved0;{this.Reserved0}");
            sb.AppendLine($"Reserved1;{this.Reserved1}");
            sb.AppendLine($"Reserved2;{this.Reserved2}");
            return sb.ToString();
        }

        /// <summary>
        /// Adds next value to header. Expecter values of counter: 0-7
        /// </summary>
        /// <param name="counter"></param>
        /// <param name="value"></param>
        public void AddValue(int counter, int value)
        {
            switch(counter)
            {
                case 0:
                    this.FormatVersion = value;
                    break;
                case 1:
                    this.Endianness = value;
                    break;
                case 2:
                    this.SamplingFrequency = value;
                    break;
                case 3:
                    this.NumberOfChannels = value;
                    break;
                case 4:
                    this.SamplesPerChunk = value;
                    break;
                case 5:
                    this.Reserved0 = value;
                    break;
                case 6:
                    this.Reserved1 = value;
                    break;
                case 7:
                    this.Reserved2 = value;
                    break;
                default:
                    throw new Exception($"Counter out of range. Expected 0-7, was {counter}");
            }
        }

        public int FormatVersion { get; set; }
        public int Endianness { get; set; }
        public int SamplingFrequency { get; set; }
        public int NumberOfChannels { get; set; }
        public int SamplesPerChunk { get; set; }
        public int Reserved0 { get; set; }
        public int Reserved1 { get; set; }
        public int Reserved2 { get; set; }
    }
}
