using System;
using System.Collections.Generic;

namespace Bede.RefitTest
{
    public class RetryPolicySettings
    {
        public RetryPolicySettings()
        {
            RetryPolicyIntervals = new List<TimeSpan>
            {
                TimeSpan.FromMilliseconds(200),
                TimeSpan.FromMilliseconds(500),
                TimeSpan.FromMilliseconds(1500)
            };
        }

        public bool RetryPolicyEnabled { get; set; }

        public IEnumerable<TimeSpan> RetryPolicyIntervals { get; set; }
    }
}