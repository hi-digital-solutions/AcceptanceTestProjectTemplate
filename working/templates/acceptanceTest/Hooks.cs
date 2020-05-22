using System;
using System.Diagnostics;
using System.Threading;
using TechTalk.SpecFlow;
using WireMock.Server;
using WireMock.Settings;

namespace AcceptanceTests
{
    [Binding]
    public static class Hooks
    {
        private static string functionDirectory = $"/build/DataEvents/bin/Release/netcoreapp3.1/";
        private static Process functionRuntime;

        public static FluentMockServer StubAPI;
        
        [BeforeTestRun]
        private static void BeforeTestRun()
        {
            StartFakeAPIs();
            StartFunctionHost();
        }
        private static void StartFakeAPIs()
        {
            StubAPI = FluentMockServer.Start(new FluentMockServerSettings
            {
                Urls = new[] { "http://localhost:5001" },
                StartAdminInterface = true
            });
        }

        private static void StartFunctionHost()
        {
            ProcessStartInfo functionInfo = new ProcessStartInfo()
            {
                FileName = "func",
                Arguments = "host start",
                UseShellExecute = false,
                WorkingDirectory = functionDirectory
            };

            functionRuntime = new Process
            {
                StartInfo = functionInfo
            };

            functionRuntime.Start();
            Thread.Sleep(10000);
        }

        [BeforeScenario]
        private static void BeforeScenario(ScenarioContext currentScenario)
        {
            Console.WriteLine($"Starting Scenario: {currentScenario.ScenarioInfo.Title}");
            StubAPI.Reset();
            TouchHostFile();
            Thread.Sleep(10000);
        }
        
        private static void TouchHostFile()
        {
            ProcessStartInfo info = new ProcessStartInfo()
            {
                FileName = "touch",
                Arguments = "host.json",
                UseShellExecute = false,
                WorkingDirectory = functionDirectory
            };
            
            var touch = new Process
            {
                StartInfo = info
            };

            touch.Start();
        }

        [AfterTestRun]
        private static void AfterTestRun()
        {
            StopFunctionHost();
            StubAPI.Stop();
        }

        private static void StopFunctionHost()
        {
            Console.WriteLine("Stopping function host...");
            functionRuntime.Kill();
            functionRuntime.WaitForExit();
            Console.WriteLine("Function host stopped.");
        }
    }
}
