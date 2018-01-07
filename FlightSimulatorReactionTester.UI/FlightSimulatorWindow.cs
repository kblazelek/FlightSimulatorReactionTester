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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlightSimulatorReactionTester.UI
{
    public partial class FlightSimulatorWindow : Form
    {
        List<TimeSpan> _reactionTimes;
        PictureBox _pictureBox;
        FutureEventSet _futureEventSet;
        IEnumerator<FutureEvent> _futureEventEnumerator;
        bool _watchForMouseClicks = false;
        readonly object _watchForMouseClicksLock = new object();
        static MultimediaTimer _changeArrowTimer;
        static Stopwatch _reactionTimer;
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
                _reactionTimer = new Stopwatch();
                _reactionTimer.Start();
                WatchForMouseClicks = true;
            }
            else
            {
                throw new Exception("Unknown arrow");
            }
        }

        private void MouseHook_MouseAction(object sender, EventArgs e)
        {
            if (WatchForMouseClicks)
            {
                WatchForMouseClicks = false;
                _reactionTimer.Stop();
                // Consider thread safety
                this.Invoke((MethodInvoker)delegate
                {
                    if (_pictureBox.Image != null)
                    {
                        _pictureBox.Image = null;
                        _pictureBox.Refresh();
                    }
                    _reactionTimes.Add(_reactionTimer.Elapsed);
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
            MouseHook.MouseAction -= MouseHook_MouseAction;
            MouseHook.Stop();
            _reactionTimer.Stop();
            _changeArrowTimer.Stop();
            this.Invoke((MethodInvoker)delegate
            {
                this.Close();
            });
        }

        public void StartSimulation(FutureEventSet futureEventSet)
        {
            _reactionTimes = new List<TimeSpan>();
            this._futureEventSet = futureEventSet;
            _futureEventEnumerator = futureEventSet.GetEnumerator();
            _futureEventEnumerator.MoveNext();
            var futureEvent = _futureEventEnumerator.Current;
            MouseHook.Start();
            MouseHook.MouseAction += MouseHook_MouseAction;
            _changeArrowTimer = new MultimediaTimer(TimeSpan.FromMilliseconds(futureEvent.Delay), ChangeArrow, TimerEventType.TIME_ONESHOT);
            _changeArrowTimer.Start();
        }

        public FlightSimulatorWindow()
        {
            InitializeComponent();
        }
        private void FlightSimulatorWindow_Load(object sender, EventArgs e)
        {
            this.Controls.Clear();
            this.BackColor = Color.LimeGreen;
            this.TransparencyKey = Color.LimeGreen;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            _pictureBox = new PictureBox
            {
                BackColor = Color.LimeGreen,
                Name = "pictureBox1",
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.CenterImage
            };
            this.Controls.Add(_pictureBox);
        }

        public List<TimeSpan> GetReactionTimes()
        {
            return _reactionTimes;
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
        }
    }
}
