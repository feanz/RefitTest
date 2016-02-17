using System;
using System.Collections.Generic;

namespace Bede.RefitTest
{
    public class RetryPolicySettings
    {
        public RetryPolicySettings()
        {
            RetryPolicyIntervals = new List<TimeSpan>();
        }

        public bool RetryPolicyEnabled { get; set; }

        public IEnumerable<TimeSpan> RetryPolicyIntervals { get; set; }
    }
}