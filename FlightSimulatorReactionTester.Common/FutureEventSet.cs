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
    /// Represents collection of <see cref="FutureEvent"/> 
    /// </summary>
    [Serializable]
    public class FutureEventSet : ObservableCollection<FutureEvent>
    {
        /// <summary>
        /// Deserializes file into <see cref="FutureEventSet"/>
        /// </summary>
        /// <param name="filename">Path to file containing serialized <see cref="FutureEventSet"/></param>
        /// <returns></returns>
        public static FutureEventSet Load(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(FutureEventSet), new XmlRootAttribute("FutureEventSet"));

            using (FileStream stream = new FileStream(filename, FileMode.Open))
            {
                return (FutureEventSet)serializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// Serializes <see cref="FutureEventSet"/> into file
        /// </summary>
        /// <param name="filename">Path to file which should contain serialized <see cref="FutureEventSet"/></param>
        public void Save(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(FutureEventSet), new XmlRootAttribute("FutureEventSet"));

            using (FileStream stream = new FileStream(filename, FileMode.Create))
            {
                serializer.Serialize(stream, this);
            }
        }
    }
}
