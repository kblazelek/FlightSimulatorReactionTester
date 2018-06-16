using FlightSimulatorReactionTester.Common;
using FlightSimulatorReactionTester.Common.Enums;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
        public List<Screen> ScreensArrow = new List<Screen>();
        public List<Screen> ScreensSquare = new List<Screen>();
        public SettingsWindow()
        {
            InitializeComponent();
            foreach(var screen in Screen.AllScreens)
            {
                ScreensArrow.Add(screen);
                ScreensSquare.Add(screen);
            }
            comboBoxArrowScreen.DataSource = ScreensArrow;
            comboBoxArrowScreen.DisplayMember = "DeviceName";
            comboBoxArrowScreen.SelectedIndex = 0;
            comboBoxSquareScreen.DataSource = ScreensSquare;
            comboBoxSquareScreen.DisplayMember = "DeviceName";
            comboBoxSquareScreen.SelectedIndex = ScreensSquare.Count > 1 ? 1 : 0;
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
            labelLoadedFutureEventSet.Text = "No Future Event Set selected";
            var outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Results");
            labelOutputDirectory.Text = outputDirectory;
            //var filename = @"C:\Users\Traxx\Desktop\FES.xml";
            //futureEventSet = FutureEventSet.Load(filename);
            //labelLoadedFutureEventSet.Text = filename;
        }

        private void LoadFutureEventSet()
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

        private void ChangeOutputDirectory()
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowserDialog.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    labelOutputDirectory.Text = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private void buttonLoadFutureEventSet_Click(object sender, EventArgs e)
        {
            try
            {
                LoadFutureEventSet();
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
                    throw new Exception("FutureEventSet file contains no Future Events");
                }
                if(String.IsNullOrEmpty(labelOutputDirectory.Text))
                {
                    throw new Exception("No output directory specified");
                }
                if(Program.FlightSimulatorWindow == null)
                {
                    Program.FlightSimulatorWindow = new FlightSimulatorWindow();
                    Program.FlightSimulatorWindow.SimulationEnding += delegate {
                        var reactionTimes = Program.FlightSimulatorWindow.GetSimulationResult();
                        if(Directory.Exists(labelOutputDirectory.Text) == false)
                        {
                            Directory.CreateDirectory(labelOutputDirectory.Text);
                        }
                        var filePath = Path.Combine(labelOutputDirectory.Text, DateTime.Now.ToString("yyyy-dd-M_HH-mm-ss") + ".xml");
                        AppendToRichTextBox($"Saved result to: {filePath}\n");
                        reactionTimes.Save(filePath);
                        foreach(var reactionTime in reactionTimes)
                        {
                            AppendToRichTextBox($"Reaction time: {reactionTime.ReactionTimeMilliseconds}\n");
                        }
                        this.Show();
                        this.TopMost = true;
                        this.BringToFront();
                    };
                }

                // Display arrows on selected screen
                Screen arrowScreen = (Screen)comboBoxArrowScreen.SelectedItem;
                Program.FlightSimulatorWindow.StartPosition = FormStartPosition.Manual;
                Program.FlightSimulatorWindow.Bounds = arrowScreen.Bounds;

                // Tell FlightSimulatorWindow which screen should display square indicator
                Screen squareScreen = (Screen)comboBoxSquareScreen.SelectedItem;
                Program.FlightSimulatorWindow.SquareScreen = squareScreen;
                Program.FlightSimulatorWindow.Show();
                this.Hide();
                Program.FlightSimulatorWindow.StartSimulation(futureEventSet);
            }
            catch(Exception ex)
            {
                AppendToRichTextBox($"Could not load start simulation. {ex.Message}");
            }
        }

        private void buttonChangeOutputDirectory_Click(object sender, EventArgs e)
        {
            ChangeOutputDirectory();
        }
    }
}
