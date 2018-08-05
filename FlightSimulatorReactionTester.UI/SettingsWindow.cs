using FlightSimulatorReactionTester.Common;
using FlightSimulatorReactionTester.Common.Enums;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
        public List<FileInfo> FutureEventSets = new List<FileInfo>();
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

        private void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedItem = comboBoxFutureEventSets.SelectedItem;
                if (selectedItem == null)
                {
                    throw new Exception("No future event set selected");
                }
                var pathToFutureEventSet = ((FileInfo)selectedItem).FullName;
                try
                {
                    futureEventSet = FutureEventSet.Load(pathToFutureEventSet);
                }
                catch(Exception ex)
                {
                    throw new Exception($"Couldn't load FutureEventSet from {pathToFutureEventSet}");
                }
                if (futureEventSet == null)
                {
                    throw new Exception("Select Future Event Set file first");
                }
                if (futureEventSet.Count == 0)
                {
                    throw new Exception("FutureEventSet file contains no Future Events");
                }
                if (String.IsNullOrEmpty(labelOutputDirectory.Text))
                {
                    throw new Exception("No output directory specified");
                }
                if (Program.FlightSimulatorWindow == null)
                {
                    Program.FlightSimulatorWindow = new FlightSimulatorWindow();
                    Program.FlightSimulatorWindow.SimulationEnding += delegate
                    {
                        var reactionTimes = Program.FlightSimulatorWindow.GetSimulationResult();
                        if (Directory.Exists(labelOutputDirectory.Text) == false)
                        {
                            Directory.CreateDirectory(labelOutputDirectory.Text);
                        }
                        var reactionTimesFilePath = Path.Combine(labelOutputDirectory.Text, DateTime.Now.ToString("yyyy-dd-M_HH-mm-ss") + "_ReactionTimes.xml");
                        AppendToRichTextBox($"Saved result to: {reactionTimesFilePath}\n");
                        reactionTimes.Save(reactionTimesFilePath);
                        foreach (var reactionTime in reactionTimes)
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

                // Read settings from App.config
                string samplesToRead = ConfigurationManager.AppSettings["SamplesToRead"];
                string hostName = ConfigurationManager.AppSettings["Hostname"];
                int port = int.Parse(ConfigurationManager.AppSettings["Port"]);
                int retryTimes = int.Parse(ConfigurationManager.AppSettings["RetryTimes"]);
                TimeSpan sleepTime = TimeSpan.FromSeconds(int.Parse(ConfigurationManager.AppSettings["SleepSeconds"]));
                var outputFile = Path.Combine(labelOutputDirectory.Text, DateTime.Now.ToString("yyyy-dd-M_HH-mm-ss") + "_EEG.csv");

                Program.FlightSimulatorWindow.StartSimulation(futureEventSet, hostName, port, retryTimes, sleepTime, outputFile);
            }
            catch (Exception ex)
            {
                AppendToRichTextBox($"Could not load start simulation. {ex.Message}");
            }
        }

        private void buttonChangeOutputDirectory_Click(object sender, EventArgs e)
        {
            ChangeOutputDirectory();
        }

        private void SettingsWindow_Load(object sender, EventArgs e)
        {
            foreach (var screen in Screen.AllScreens)
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
            var outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Results");
            labelOutputDirectory.Text = outputDirectory;
            var futureEventSetsDir = Path.Combine(Directory.GetCurrentDirectory(), "FutureEventSets");
            if (Directory.Exists(futureEventSetsDir))
            {
                var paths = Directory.GetFiles(futureEventSetsDir, "*.xml").ToList();
                if (paths.Count == 0)
                {
                    AppendToRichTextBox($"There are no FutureEventSets in {futureEventSetsDir}");
                }
                foreach (var path in paths)
                {
                    FutureEventSets.Add(new FileInfo(path));
                }
            }
            comboBoxFutureEventSets.DataSource = FutureEventSets;
            comboBoxFutureEventSets.DisplayMember = "Name";
        }
    }
}
