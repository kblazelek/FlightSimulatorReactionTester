using System;
using System.Runtime.InteropServices;

namespace FlightSimulatorReactionTester.Common
{
    public enum TimerEventType
    {
        /// <summary>
        /// Event occurs once
        /// </summary>
        TIME_ONESHOT = 0,
        /// <summary>
        /// Event occurs periodically
        /// </summary>
        TIME_PERIODIC = 1
    }

    /// <summary>
    /// High precision multimedia timer
    /// </summary>
    public class MultimediaTimer
    {
        private int _interval;
        private Action _elapsedTimerHandler;
        private int _timerId;
        private delegate void TimerEventHandler(int id, int msg, IntPtr user, int dw1, int dw2);
        private TimerEventHandler _handler;
        private TimerEventType _timerEventType;
        private readonly uint _timerResolution = 1;

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

        #region Constructors
        /// <summary>
        /// Creates instance of MultimediaTimer
        /// </summary>
        /// <param name="interval">Delay between timer ticks</param>
        /// <param name="callback">Function called after interval</param>
        /// <param name="timerEventType">Determines whether <paramref name="callback"/> should be called once or periodically</param>
        public MultimediaTimer(TimeSpan interval, Action callback, TimerEventType timerEventType)
        {
            _interval = (int)interval.TotalMilliseconds;
            _elapsedTimerHandler = callback;
            _timerEventType = timerEventType;
        }

        /// <summary>
        /// Creates instance of MultimediaTimer with given timer resolution
        /// </summary>
        /// <param name="interval">Delay between timer ticks</param>
        /// <param name="callback">Function called after interval</param>
        /// <param name="timerEventType">Determines whether <paramref name="callback"/> should be called once or periodically</param>
        /// <param name="timerResolution">Timer resulution in ms. A lower value specifies a higher (more accurate) resolution</param>
        public MultimediaTimer(TimeSpan interval, Action callback, TimerEventType timerEventType, uint timerResolution) : this(interval, callback, timerEventType)
        {
            _timerResolution = timerResolution;
        }
        #endregion

        /// <summary>
        /// Method called periodically from multimedia timer
        /// </summary>
        private void TimerHandler(int id, int msg, IntPtr user, int dw1, int dw2)
        {
            _elapsedTimerHandler();
        }

        /// <summary>
        /// Starts the timer
        /// </summary>
        public void Start()
        {
            // Begin using timer (make sure to finish with timeEndPeriod)
            timeBeginPeriod((int)_timerResolution);

            // Start the timer
            _handler = new TimerEventHandler(TimerHandler);
            _timerId = timeSetEvent(_interval, 0, _handler, IntPtr.Zero, (int)_timerEventType);
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void Stop()
        {
            // Stop the timer
            timeKillEvent(_timerId);

            // Finish using timer (should be called after timeBeginPeriod)
            timeEndPeriod((int)_timerResolution);
            _timerId = 0;
        }
    }
}
