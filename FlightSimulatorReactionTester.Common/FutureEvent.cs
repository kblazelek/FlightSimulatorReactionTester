using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorReactionTester.Common
{
    [Serializable]
    public class FutureEvent : INotifyPropertyChanged
    {
        private string _arrow;
        private int _delay;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Arrow
        {
            get { return _arrow; }
            set
            {
                _arrow = value;
                NotifyPropertyChanged("Arrow");
            }
        }

        public int Delay
        {
            get { return _delay; }
            set
            {
                _delay = value;
                NotifyPropertyChanged("Delay");
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
