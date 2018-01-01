using FlightSimulatorReactionTester.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace FlightSimulatorReactionTester.FutureEventSetEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FutureEventSet futureEventSet;

        public MainWindow()
        {
            InitializeComponent();
            futureEventSet = (FutureEventSet)Resources["FutureEventSet"];
        }

        private void LoadFile()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".xml"; // Default file extension
            dlg.Filter = "XML documents (.xml)|*.xml"; // Filter files by extension
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                FutureEventSet futureEvents = FutureEventSet.Load(filename);
                foreach (var futureEvent in futureEvents)
                {
                    futureEventSet.Add(futureEvent);
                }
            }
        }

        private void SaveFile()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "FutureEventSet"; // Default file name
            dlg.DefaultExt = ".xml"; // Default file extension
            dlg.Filter = "XML documents (.xml)|*.xml"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                futureEventSet.Save(filename);
            }
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }

        private void buttonLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadFile();
        }
    }
}
