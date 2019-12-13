using AutomationAPI.Helpers;
using Service.Models.TestCaseExecution;
using Service.Repositories.TestCaseExecution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace AutomationAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
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

        [HttpGet]
        [Route("ExportTestReport")]
        public HttpResponseMessage ExportTestReport(string filePath)
        {
            try
            {
                filePath = String.Format(filePath);
                if (!File.Exists(filePath))
                {
                    return Request.CreateResponse(String.Empty);
                }
                var fileContents = File.ReadAllText(filePath);
                var response = new HttpResponseMessage();
                response.Content = new StringContent(fileContents);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                return response;
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
