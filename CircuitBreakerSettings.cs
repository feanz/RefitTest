namespace Bede.RefitTest
{
    public class CircuitBreakerSettings
    {
        public CircuitBreakerSettings()
        {
            ExceptionAllowedBeforeCircuitBroken = 3;
            DurationOfCircuitBreakMiliseconds = 3000;
        }

        public bool CircuitBreakerEnabled { get; set; }

        public int ExceptionAllowedBeforeCircuitBroken { get; set; }

        public int DurationOfCircuitBreakMiliseconds { get; set; }
    }
}