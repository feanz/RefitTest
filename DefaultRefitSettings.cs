using Refit;

namespace Bede.RefitTest
{
    public class DefaultRefitSettings : RefitSettings
    {
        public DefaultRefitSettings()
        {
            LoggingSettings = new LoggingSettings();
            CircuitBreakerSettings = new CircuitBreakerSettings();
            RetryPolicySettings = new RetryPolicySettings();

            HttpMessageHandlerFactory = () =>
            {
                var handlers = new LoggingHandler(
                    LoggingSettings,
                    new CircuitBreakerPolicyHandler(LoggingSettings, CircuitBreakerSettings,
                        new RetryPolicyHandler(LoggingSettings, RetryPolicySettings,
                            new NoOpHandler())));

                return handlers;
            };
        }

        public LoggingSettings LoggingSettings { get; set; }

        public CircuitBreakerSettings CircuitBreakerSettings { get; set; }

        public RetryPolicySettings RetryPolicySettings { get; set; }
    }
}