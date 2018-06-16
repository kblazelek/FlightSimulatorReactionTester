using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlightSimulatorReactionTester.Common
{
    /// <summary>
    /// Represents collection of <see cref="FutureEventResult"/> 
    /// </summary>
    [Serializable]
    public class FutureEventSetResult : ObservableCollection<FutureEventResult>
    {
        /// <summary>
        /// Deserializes file into <see cref="FutureEventSetResult"/>
        /// </summary>
        /// <param name="filename">Path to file containing serialized <see cref="FutureEventSetResult"/></param>
        /// <returns></returns>
        public static FutureEventSetResult Load(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(FutureEventSetResult), new XmlRootAttribute("FutureEventSetResult"));

            using (FileStream stream = new FileStream(filename, FileMode.Open))
            {
                return (FutureEventSetResult)serializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// Serializes <see cref="FutureEventSetResult"/> into file
        /// </summary>
        /// <param name="filename">Path to file which should contain serialized <see cref="FutureEventSetResult"/></param>
        public void Save(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(FutureEventSetResult), new XmlRootAttribute("FutureEventSetResult"));

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            {
                serializer.Serialize(stream, this);
            }
        }
    }
}
