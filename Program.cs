using System;
using System.Configuration;
using System.Linq;
using Bede.GamingCompliance.Contracts.ComplianceSettings;
using Refit;
using static System.Convert;

namespace Bede.RefitTest
{
    internal class Program
    {
        //todo not found considered transient exception
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
}