using Service.Models.TestCaseExecution;
using System.Collections.Generic;

namespace Service.Repositories.TestCaseExecution
{
    public interface ITestCaseExecutionRepository
    {
        IEnumerable<dynamic> GetTestSuiteDetails(string username);

        IEnumerable<dynamic> GetTestSuiteResults();

        IEnumerable<dynamic> GetTestScenarioResults(int suiteExecutionID);

        int UpsertTestSuiteExecutionResult(TestSuite testSuite);
    }
}
