using Service.Models.TestCaseExecution;
using Service.Repositories.TestCaseExecution;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace AutomationAPI.Helpers
{
    public class ProtractorService
    {
        public void ExecuteTestSuite(ITestCaseExecutionRepository testCaseExecutionRepository, TestSuite testSuite)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = "cmd.exe";

                try
                {
                    testSuite.ExecutionStatus = ExecutionStatusSetup.Fail;
                    testSuite.RowID = testCaseExecutionRepository.UpsertTestSuiteExecutionResult(testSuite);

                    if (testSuite.RowID != default(int?) || testSuite.RowID != 0)
                    {
                        StartSeleniumServer();

                        var command = String.Empty;

                        if (testSuite.ProjectName == "MAP-Internal Portal")
                        {
                            command = $@"/K cd { ConfigurationManager.AppSettings["InternalPortalPath"] }&protractor config.js --suite { testSuite.SuiteTypeName }  --params.Suite_Name { testSuite.SuiteTypeName } --params.UserName { testSuite.Username } --params.Suite_Execution_ID { testSuite.RowID }";
                        }
                        else if (testSuite.ProjectName == "MAP-External Portal")
                        {
                            command = $@"/K cd { ConfigurationManager.AppSettings["ExternalPortalPath"] }&protractor config.js --suite { testSuite.SuiteTypeName } --params.Suite_Name { testSuite.SuiteTypeName } --params.UserName { testSuite.Username } --params.Suite_Execution_ID { testSuite.RowID }";
                        }

                        process.StartInfo.Arguments = $"{ command } & timeout 5 & exit";

                        process.Start();
                        //process.WaitForExit(); 
                    }
                }
                catch (Exception ex)
                {
                    testSuite.ExecutionStatus = ExecutionStatusSetup.Fail;
                    testCaseExecutionRepository.UpsertTestSuiteExecutionResult(testSuite);
                }
            }
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
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.Arguments = "/K webdriver-manager start";
                    process.Start();
                }
                Thread.Sleep(5000);
            }
        }
    }
}
