using System;

namespace Bede.RefitTest
{
    public class LoggingSettings
    {
        public bool LoggingEnabled { get; set; }

        public Action<string, string> LogInformation { get; set; }

        public Action<string, Exception, string> LogError { get; set; }
    }
}