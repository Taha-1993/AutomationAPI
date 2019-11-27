using AutomationAPI.Helpers;
using Service.Models.TestCaseExecution;
using Service.Repositories.TestCaseExecution;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;

namespace AutomationAPI.Controllers
{
    [RoutePrefix("api/TestCaseExecution")]
    public class TestCaseExecutionController : ApiController
    {
        private readonly ITestCaseExecutionRepository _testCaseExecutionRepository;

        public TestCaseExecutionController(ITestCaseExecutionRepository testCaseExecutionRepository)
        {
            _testCaseExecutionRepository = testCaseExecutionRepository;
        }

        [HttpGet]
        [Route("GetTestSuiteDetails")]
        [ResponseType(typeof(IEnumerable<dynamic>))]
        public IHttpActionResult GetTestSuiteDetails(string username)
        {
            try
            {
                return Ok(_testCaseExecutionRepository.GetTestSuiteDetails(username));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("GetTestSuiteResults")]
        [ResponseType(typeof(IEnumerable<dynamic>))]
        public IHttpActionResult GetTestSuiteResults()
        {
            try
            {
                return Ok(_testCaseExecutionRepository.GetTestSuiteResults());
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("GetTestScenarioResults")]
        [ResponseType(typeof(IEnumerable<dynamic>))]
        public IHttpActionResult GetTestScenarioResults(int suiteExecutionID)
        {
            try
            {
                return Ok(_testCaseExecutionRepository.GetTestScenarioResults(suiteExecutionID));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("ExecuteTestSuite")]
        [ResponseType(typeof(int))]
        public IHttpActionResult ExecuteTestSuite([FromBody]List<TestSuite> testSuiteList)
        {
            try
            {
                var protractorService = new ProtractorService();
                foreach (var item in testSuiteList)
                {
                    protractorService.ExecuteTestSuite(_testCaseExecutionRepository, item);
                }
                return Ok(10);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
