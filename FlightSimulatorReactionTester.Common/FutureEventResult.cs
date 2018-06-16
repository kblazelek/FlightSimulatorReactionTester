using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorReactionTester.Common
{
    /// <summary>
    /// Contains information about <see cref="FutureEvent"/> with reaction time to it
    /// </summary>
    [Serializable]
    public class FutureEventResult
    {
        public double ReactionTimeMilliseconds { get; set; }
        public FutureEvent FutureEvent { get; set; }
        public FutureEventResult()
        {

        }
        public FutureEventResult(TimeSpan reactionTime, FutureEvent futureEvent)
        {
            this.ReactionTimeMilliseconds = reactionTime.TotalMilliseconds;
            this.FutureEvent = futureEvent;
        }
    }
}
