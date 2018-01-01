using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlightSimulatorReactionTester.Common
{
    /// <summary>
    /// High precision multimedia timer
    /// </summary>
    public class MultimediaTimer
    {
        private int _interval;
        public delegate void ElapsedTimerDelegate();
        private ElapsedTimerDelegate _elapsedTimerHandler;
        private int _timerId;
        private delegate void TimerEventHandler(int id, int msg, IntPtr user, int dw1, int dw2);
        private TimerEventHandler _handler;

        private enum TimerEventType
        {
            TIME_ONESHOT = 0,       // Event occurs once
            TIME_PERIODIC = 1,      // Event occurs periodically
        }

        #region P/Invoke Statements
        [DllImport("winmm.dll")]
        private static extern int timeSetEvent(int delay, int resolution, TimerEventHandler handler, IntPtr user, int eventType);
        [DllImport("winmm.dll")]
        private static extern int timeKillEvent(int id);
        [DllImport("winmm.dll")]
        private static extern int timeBeginPeriod(int msec);
        [DllImport("winmm.dll")]
        private static extern int timeEndPeriod(int msec);
        #endregion

        public MultimediaTimer(int interval, ElapsedTimerDelegate callback)
        {
            _interval = interval;
            _elapsedTimerHandler = callback;
        }

        private void TimerHandler(int id, int msg, IntPtr user, int dw1, int dw2)
        {
            _elapsedTimerHandler();
        }

        public void Start()
        {
            timeBeginPeriod(1);
            _handler = new TimerEventHandler(TimerHandler);
            _timerId = timeSetEvent(_interval, 0, _handler, IntPtr.Zero, (int)TimerEventType.TIME_PERIODIC);
        }

        public void Stop()
        {
            int err = timeKillEvent(_timerId);
            timeEndPeriod(1);
            _timerId = 0;
        }
    }
}
