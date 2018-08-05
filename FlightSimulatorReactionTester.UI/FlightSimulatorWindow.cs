using FlightSimulatorReactionTester.Common;
using FlightSimulatorReactionTester.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TCP.Common;

namespace FlightSimulatorReactionTester.UI
{
    public partial class FlightSimulatorWindow : Form
    {
        FutureEventSetResult _futureEventSetResult;
        TCPReader tcpReader;
        PictureBox _pictureBox;
        FutureEventSet _futureEventSet;
        IEnumerator<FutureEvent> _futureEventEnumerator;
        bool _watchForMouseClicks = false;
        readonly object _watchForMouseClicksLock = new object();
        static MultimediaTimer _changeArrowTimer;
        static Stopwatch _reactionTimer;
        public event Action SimulationEnding;
        public Screen SquareScreen;
        SquareIndicatorWindow squareIndicatorWindow = new SquareIndicatorWindow();
        bool WatchForMouseClicks
        {
            get
            {
                lock (_watchForMouseClicksLock)
                {
                    return _watchForMouseClicks;
                }
            }
            set
            {
                lock (_watchForMouseClicksLock)
                {
                    _watchForMouseClicks = value;
                }
            }
        }

        public void ChangeArrow()
        {
            Arrow currentArrow;
            if (Enum.TryParse(_futureEventEnumerator.Current.Arrow, out currentArrow))
            {
                this.Invoke((MethodInvoker)delegate
                {
                    ShowArrow(currentArrow);
                });
                tcpReader.CurrentArrowState = 1;
                _reactionTimer = new Stopwatch();
                _reactionTimer.Start();
                MouseHook.ChangeButtonEvent(ArrowToButtonEvent(currentArrow));
                WatchForMouseClicks = true;
            }
            else
            {
                throw new Exception($"Unknown arrow {_futureEventEnumerator.Current.Arrow}");
            }
        }

        private void MouseHook_MouseAction(object sender, EventArgs e)
        {
            if (WatchForMouseClicks)
            {
                WatchForMouseClicks = false;
                _reactionTimer.Stop();
                tcpReader.CurrentArrowState = 0;
                this.Invoke((MethodInvoker)delegate
                {
                    if (_pictureBox.Image != null)
                    {
                        _pictureBox.Image = null;
                        _pictureBox.Refresh();
                    }
                    squareIndicatorWindow.Hide();
                    _futureEventSetResult.Add(new FutureEventResult(_reactionTimer.Elapsed, _futureEventEnumerator.Current));
                });
                if (_futureEventEnumerator.MoveNext())
                {
                    _changeArrowTimer = new MultimediaTimer(TimeSpan.FromMilliseconds(_futureEventEnumerator.Current.Delay), ChangeArrow, TimerEventType.TIME_ONESHOT);
                    _changeArrowTimer.Start();
                }
                else
                {
                    StopSimulation();
                }
            }
        }

        public void StopSimulation()
        {
            // Program.FlightSimulatorWindow.Close();
            MouseHook.Action -= MouseHook_MouseAction;
            MouseHook.Stop();
            _reactionTimer.Stop();
            _changeArrowTimer.Stop();
            SimulationEnding?.Invoke();
            this.Invoke((MethodInvoker)delegate
            {
                this.Hide();
            });
        }

        public void StartSimulation(FutureEventSet futureEventSet, string hostName, int port, int retryTimes, TimeSpan sleepTime, string outputFilePath)
        {
            tcpReader = new TCPReader(hostName, port, retryTimes, sleepTime, outputFilePath);
            // Start TCP Reader in separate thread
            new Thread(() =>
            {
                tcpReader.Start();
            }).Start();
            tcpReader.CurrentArrowState = 0;
            squareIndicatorWindow.StartPosition = FormStartPosition.Manual;
            squareIndicatorWindow.Bounds = new Rectangle(0, SquareScreen.Bounds.Height - 100, 100, 100);
            squareIndicatorWindow.FormBorderStyle = FormBorderStyle.None;
            squareIndicatorWindow.TopMost = true;
            _futureEventSetResult = new FutureEventSetResult();
            this._futureEventSet = futureEventSet;
            _futureEventEnumerator = futureEventSet.GetEnumerator();
            _futureEventEnumerator.MoveNext();
            var futureEvent = _futureEventEnumerator.Current;
            Arrow currentArrow;
            if (Enum.TryParse(_futureEventEnumerator.Current.Arrow, out currentArrow))
            {
            }
            else
            {
                throw new Exception($"Unknown arrow {_futureEventEnumerator.Current.Arrow}");
            }
            MouseHook.Start(ArrowToButtonEvent(currentArrow));
            MouseHook.Action += MouseHook_MouseAction;
            _changeArrowTimer = new MultimediaTimer(TimeSpan.FromMilliseconds(futureEvent.Delay), ChangeArrow, TimerEventType.TIME_ONESHOT);
            _changeArrowTimer.Start();
        }

        private ButtonEvent ArrowToButtonEvent(Arrow arrow)
        {
            switch (arrow)
            {
                case Arrow.Up:
                    return ButtonEvent.ForwardButtonDown;
                case Arrow.Right:
                    return ButtonEvent.RightButtonDown;
                case Arrow.Down:
                    return ButtonEvent.BackButtonDown;
                case Arrow.Left:
                    return ButtonEvent.LeftButtonDown;
                default:
                    throw new Exception($"Unknown arrow {arrow}");
            }
        }

        public FlightSimulatorWindow()
        {
            InitializeComponent();
        }
        private void FlightSimulatorWindow_Load(object sender, EventArgs e)
        {
            this.Controls.Clear();
            this.BackColor = Color.Black;
            //this.TransparencyKey = Color.LimeGreen;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            _pictureBox = new PictureBox
            {
                BackColor = Color.Black,
                Name = "pictureBox1",
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.CenterImage
            };
            this.Controls.Add(_pictureBox);
        }

        public FutureEventSetResult GetSimulationResult()
        {
            return _futureEventSetResult;
        }

        public void ShowArrow(Arrow arrow)
        {
            switch (arrow)
            {
                case Arrow.Down:
                    this._pictureBox.Image = Properties.Resources.arrow_down;
                    break;
                case Arrow.Up:
                    this._pictureBox.Image = Properties.Resources.arrow_up;
                    break;
                case Arrow.Left:
                    this._pictureBox.Image = Properties.Resources.arrow_left;
                    break;
                case Arrow.Right:
                    this._pictureBox.Image = Properties.Resources.arrow_right;
                    break;
            }
            this._pictureBox.Refresh();
            squareIndicatorWindow.Show();
        }
    }
}
