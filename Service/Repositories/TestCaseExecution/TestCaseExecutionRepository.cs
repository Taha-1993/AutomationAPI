using Service.Helpers;
using Service.Models.TestCaseExecution;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Service.Repositories.TestCaseExecution
{
    public class TestCaseExecutionRepository : ITestCaseExecutionRepository
    {
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["TransReAutomation"].ToString();

        #region Constants
        // Input parameters
        private const string PARAM_USERNAME = "@Username";
        private const string PARAM_SUITE_EXECUTION_ID = "@Suite_Execution_ID";
        private const string PARAM_PROJECT_NAME = "@Project_Name";
        private const string PARAM_SUITE_NAME = "@Suite_Name";
        private const string PARAM_EXECUTION_STATUS = "@Execution_Status";
        private const string PARAM_ROW_ID = "@Row_ID";

        // Stored Procedure names
        private const string USP_GET_TEST_SUITE_DETAILS = "usp_Get_Test_Suite_Details";
        private const string USP_GET_TEST_SUITE_RESULTS = "usp_Get_Test_Suite_Results";
        private const string USP_GET_TEST_SCENARIO_RESULTS = "usp_Get_Test_Scenario_Results";
        private const string USP_INSERT_TEST_SUITE_EXECUTION_RESULT = "usp_insert_test_suite_execution_result";
        #endregion

        public IEnumerable<dynamic> GetTestSuiteDetails(string username)
        {
            var sqlParams = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = PARAM_USERNAME, SqlDbType = SqlDbType.VarChar, Value = username }
            };

            return Utilities.Get(ConnectionString, USP_GET_TEST_SUITE_DETAILS, sqlParams);
        }

        public IEnumerable<dynamic> GetTestSuiteResults()
        {
            return Utilities.Get(ConnectionString, USP_GET_TEST_SUITE_RESULTS);
        }

        public IEnumerable<dynamic> GetTestScenarioResults(int suiteExecutionID)
        {
            var sqlParams = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = PARAM_SUITE_EXECUTION_ID, SqlDbType = SqlDbType.Int, Value = suiteExecutionID }
            };

            return Utilities.Get(ConnectionString, USP_GET_TEST_SCENARIO_RESULTS, sqlParams);
        }

        public int UpsertTestSuiteExecutionResult(TestSuite testSuite)
        {
            var sqlParams = new List<SqlParameter>()
            {
                new SqlParameter() { ParameterName = PARAM_PROJECT_NAME, SqlDbType = SqlDbType.VarChar, Value = testSuite.ProjectName },
                new SqlParameter() { ParameterName = PARAM_SUITE_NAME, SqlDbType = SqlDbType.VarChar, Value = testSuite.SuiteTypeName },
                new SqlParameter() { ParameterName = PARAM_EXECUTION_STATUS, SqlDbType = SqlDbType.VarChar, Value = testSuite.ExecutionStatus },
                new SqlParameter() { ParameterName = PARAM_ROW_ID, SqlDbType = SqlDbType.Int, Value = testSuite.RowID.ToDBNull() },
                new SqlParameter() { ParameterName = PARAM_USERNAME, SqlDbType = SqlDbType.VarChar, Value = testSuite.Username }
            };

            return Utilities.UpsertHelper(ConnectionString, USP_INSERT_TEST_SUITE_EXECUTION_RESULT, sqlParams);
        }
    }
}
