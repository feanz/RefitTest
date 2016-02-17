namespace Bede.RefitTest
{
    public class CircuitBreakerSettings
    {
        public bool CircuitBreakerEnabled { get; set; }

        public int ExceptionAllowedBeforeCircuitBroken { get; set; }

        public int DurationOfCircuitBreakMiliseconds { get; set; }
    }
}