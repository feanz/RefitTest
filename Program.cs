using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Bede.GamingCompliance.Contracts.ComplianceSettings;
using Refit;
using static System.Convert;

namespace Bede.RefitTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //var client = new HttpClient(new LoggingHandler(new RetryPolicyHandler(new NoOpHandler())))
            //{
            //    Timeout = TimeSpan.FromSeconds(5),
            //    BaseAddress = new Uri("http://localhost:3638/api")
            //};

            var api = RestService.For<IComplianceSettingsApi>("http://localhost:3638/api",
                new DefaultRefitSettings());

            while (true)
            {
                try
                {
                    var result = api.GetComplianceSettings(123, Guid.NewGuid().ToString()).Result;
                }
                catch (Exception ex)
                {
                }
            }

            Console.ReadLine();
        }
    }

    //todo not found considered transient exception
    //todo add actual logging
    public class DefaultRefitSettings : RefitSettings
    {
        private readonly string _configSettingPrefix;
        private bool _retryPolicyEnabled;
        private bool _loggingEnabled;
        private bool _circuitBreakerEnabled;
        private int _exceptionAllowedBeforeBreaking = 3;
        private int _durationOfCircuitBreak = 3000;
        private IEnumerable<TimeSpan> _retryPolicyTimes;
        
        public DefaultRefitSettings(string configSettingPrefix = "")
        {
            _configSettingPrefix = configSettingPrefix;

            HttpMessageHandlerFactory = () =>
            {
               var handlers = new LoggingHandler(
                   LoggingEnabled, 
                   new CircuitBreakerPolicyHandler(CircuitBreakerEnabled,
                        ExceptionAllowedBeforeCircuitBroken,
                        DurationOfCircuitBreakMiliseconds,
                        new RetryPolicyHandler(
                            RetryPolicyEnabled,
                            RetryPolicyIntervals,
                            new NoOpHandler())));

                return handlers;
            };
        }
    
        public bool LoggingEnabled
        {
            get
            {
                var appSetting = GetAppSetting<bool>("ClientSettings.LoggingEnabled");

                return appSetting || _loggingEnabled;
            }
            set { _loggingEnabled = value; }
        }

        public bool CircuitBreakerEnabled
        {
            get
            {
                var appSetting = GetAppSetting<bool>("ClientSettings.CircuitBreakerEnabled");

                return appSetting || _circuitBreakerEnabled;
            }
            set { _circuitBreakerEnabled = value; }
        }

        public int ExceptionAllowedBeforeCircuitBroken
        {
            get
            {
                var appSetting = GetAppSetting<int>("ClientSettings.ExceptionAllowedBeforeCircuitBroken");

                return appSetting != 0 ? appSetting : _exceptionAllowedBeforeBreaking;
            }
            set { _exceptionAllowedBeforeBreaking = value; }
        }

        public int DurationOfCircuitBreakMiliseconds
        {
            get
            {
                var appSetting = GetAppSetting<int>("ClientSettings.DurationOfCircuitBreakMiliseconds");

                return appSetting != 0 ? appSetting : _durationOfCircuitBreak;
            }
            set { _durationOfCircuitBreak = value; }
        }

        public bool RetryPolicyEnabled
        {
            get
            {
                var appSetting = GetAppSetting<bool>("ClientSettings.RetryPolicyEnabled");

                return appSetting || _retryPolicyEnabled;
            }
            set { _retryPolicyEnabled = value; }
        }

        public IEnumerable<TimeSpan> RetryPolicyIntervals
        {
            get
            {
                var appSetting = GetAppSetting<string>("ClientSettings.DurationOfCircuitBreakMiliseconds");

                return appSetting?.Split(',').Select(x => TimeSpan.FromMilliseconds(int.Parse(x))) ?? _retryPolicyTimes;
            }
            set { _retryPolicyTimes = value; }
        } 

        private T GetAppSetting<T>(string key)
        {
            if (ConfigurationManager.AppSettings[_configSettingPrefix + key].HasValue())
            {
                return (T) ChangeType(ConfigurationManager.AppSettings[_configSettingPrefix + key], typeof (T));
            }

            return default(T);
        }
    }
}