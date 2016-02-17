using System;

namespace Bede.RefitTest
{
    public class LoggingSettings
    {
        public LoggingSettings()
        {
            LogInformation = (s, s1) => { };
            LogError = (s, exception, arg3) => { };
        }

        public bool LoggingEnabled { get; set; }

        public Action<string, string> LogInformation { get; set; }

        public Action<string, Exception, string> LogError { get; set; }
    }
}