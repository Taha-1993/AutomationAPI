using Newtonsoft.Json;
using RemotableType;
using Service.Models.TestCaseExecution;
using Service.Repositories.TestCaseExecution;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;

namespace AutomationAPI.Helpers
{
    public class ProtractorService
    {
        public void ExecuteTestSuite(ITestCaseExecutionRepository testCaseExecutionRepository, TestSuite testSuite)
        {
            try
            {
                ChannelServices.RegisterChannel(new TcpChannel(), true);
            }
            catch (RemotingException ex)
            {
            }

            testSuite.ExecutionStatus = ExecutionStatusSetup.Fail;
            testSuite.RowID = testCaseExecutionRepository.UpsertTestSuiteExecutionResult(testSuite);

            if (testSuite.RowID != default(int?) && testSuite.RowID != 0)
            {
                StartSeleniumServer();

                var command = String.Empty;

                if (testSuite.ProjectName == "MAP-Internal Portal")
                {
                    command = $@"/C cd { ConfigurationManager.AppSettings["InternalPortalPath"] }&protractor config.js --suite { testSuite.SuiteTypeName }  --params.Suite_Name { testSuite.SuiteTypeName } --params.UserName { testSuite.Username } --params.Suite_Execution_ID { testSuite.RowID }";
                }
                else if (testSuite.ProjectName == "MAP-External Portal")
                {
                    command = $@"/C cd { ConfigurationManager.AppSettings["ExternalPortalPath"] }&protractor config.js --suite { testSuite.SuiteTypeName } --params.Suite_Name { testSuite.SuiteTypeName } --params.UserName { testSuite.Username } --params.Suite_Execution_ID { testSuite.RowID }";
                }

                var processName = "cmd.exe";
                command = $"{ processName }|{ command } & timeout 5 & exit";

                var remoteObject = (ProcessActivator)Activator.GetObject(typeof(ProcessActivator), "tcp://localhost:9600/InstantiateProcess");
                remoteObject.StartProcess(command);
            }
            //using (var process = new Process())
            //{
            //    process.StartInfo.FileName = "cmd.exe";

            //    try
            //    {
            //        testSuite.ExecutionStatus = ExecutionStatusSetup.Fail;
            //        testSuite.RowID = testCaseExecutionRepository.UpsertTestSuiteExecutionResult(testSuite);

            //        if (testSuite.RowID != default(int?) && testSuite.RowID != 0)
            //        {
            //            StartSeleniumServer();

            //            var command = String.Empty;

            //            if (testSuite.ProjectName == "MAP-Internal Portal")
            //            {
            //                command = $@"/C cd { ConfigurationManager.AppSettings["InternalPortalPath"] }&protractor config.js --suite { testSuite.SuiteTypeName }  --params.Suite_Name { testSuite.SuiteTypeName } --params.UserName { testSuite.Username } --params.Suite_Execution_ID { testSuite.RowID }";
            //            }
            //            else if (testSuite.ProjectName == "MAP-External Portal")
            //            {
            //                command = $@"/C cd { ConfigurationManager.AppSettings["ExternalPortalPath"] }&protractor config.js --suite { testSuite.SuiteTypeName } --params.Suite_Name { testSuite.SuiteTypeName } --params.UserName { testSuite.Username } --params.Suite_Execution_ID { testSuite.RowID }";
            //            }

            //            process.StartInfo.Arguments = $"{ command } & timeout 5 & exit";

            //            process.Start();
            //            //process.WaitForExit(); 
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        testSuite.ExecutionStatus = ExecutionStatusSetup.Fail;
            //        testCaseExecutionRepository.UpsertTestSuiteExecutionResult(testSuite);
            //    }
            //}
        }

        private void StartSeleniumServer()
        {
            try
            {
                var json = new WebClient().DownloadString(ConfigurationManager.AppSettings["SeleniumServer"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                var processName = "cmd.exe";
                var arguments = "/C webdriver-manager start";
                var command = $"{ processName }|{ arguments }";

                var remoteObject = (ProcessActivator)Activator.GetObject(typeof(ProcessActivator), "tcp://localhost:9600/InstantiateProcess");
                remoteObject.StartProcess(command);
                //using (Process process = new Process())
                //{
                //    process.StartInfo.FileName = "cmd.exe";
                //    process.StartInfo.Arguments = "/C webdriver-manager start";
                //    process.Start();
                //}
                Thread.Sleep(5000);
            }
        }
    }
}
