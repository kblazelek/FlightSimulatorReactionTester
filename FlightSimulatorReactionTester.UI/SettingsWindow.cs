using FlightSimulatorReactionTester.Common;
using FlightSimulatorReactionTester.Common.Enums;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlightSimulatorReactionTester.UI
{
    public partial class SettingsWindow : Form
    {
        private FutureEventSet futureEventSet;
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void AppendToRichTextBox(string textToAppend)
        {
            this.Invoke((MethodInvoker)delegate
            {
                richTextBoxReactionTimes.AppendText(textToAppend);
                richTextBoxReactionTimes.SelectionStart = richTextBoxReactionTimes.Text.Length;
                richTextBoxReactionTimes.ScrollToCaret();
            });
        }

        private void ClientWindow_Load(object sender, EventArgs e)
        {
            this.labelLoadedFutureEventSet.Text = "No Future Event Set selected";
            var filename = @"C:\Users\Traxx\Desktop\FES.xml";
            futureEventSet = FutureEventSet.Load(filename);
            labelLoadedFutureEventSet.Text = filename;
        }

        private void LoadFile()
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Select Future Event Set file";
            fdlg.Filter = "XML documents (.xml)|*.xml";
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                string filename = fdlg.FileName;
                futureEventSet = FutureEventSet.Load(filename);
                labelLoadedFutureEventSet.Text = filename;
            }
        }

        private void buttonLoadFutureEventSet_Click(object sender, EventArgs e)
        {
            try
            {
                LoadFile();
            }
            catch(Exception ex)
            {
                AppendToRichTextBox($"Could not load selected Future Event Set file. {ex.Message}");
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (futureEventSet == null)
                {
                    throw new Exception("Select Future Event Set file first");
                }
                if(futureEventSet.Count == 0)
                {
                    throw new Exception("FutureEventSet contains no Future Events");
                }
                if(Program.FlightSimulatorWindow == null)
                {
                    Program.FlightSimulatorWindow = new FlightSimulatorWindow();
                    Program.FlightSimulatorWindow.SimulationEnding += delegate {
                        var rectionTimes = Program.FlightSimulatorWindow.GetReactionTimes();
                        foreach(var reactionTime in rectionTimes)
                        {
                            AppendToRichTextBox($"Reaction time: {reactionTime}\n");
                        }
                        this.Show();
                        this.TopMost = true;
                        this.BringToFront();
                    };
                }
                Program.FlightSimulatorWindow.Show();
                this.Hide();
                Program.FlightSimulatorWindow.StartSimulation(futureEventSet);
            }
            catch(Exception ex)
            {
                AppendToRichTextBox($"Could not load start simulation. {ex.Message}");
            }
        }
    }
}
