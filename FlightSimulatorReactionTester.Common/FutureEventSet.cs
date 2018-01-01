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
    [Serializable]
    public class FutureEventSet : ObservableCollection<FutureEvent>
    {
        public static FutureEventSet Load(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(FutureEventSet), new XmlRootAttribute("FutureEventSet"));

            using (FileStream stream = new FileStream(filename, FileMode.Open))
            {
                return (FutureEventSet)serializer.Deserialize(stream);
            }
        }

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
