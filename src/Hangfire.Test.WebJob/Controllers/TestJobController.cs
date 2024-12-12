using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Test.WebJob.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestJobController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> Job1Async([FromBody] string jobName)
        {
            Console.WriteLine($"start ==> {jobName}, {DateTime.Now}");
            await Task.Delay(3000);
            Console.WriteLine($"end   ==> {jobName}, {DateTime.Now}");
            return Ok(new { Result = true, Data = new { JobName = jobName } });
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok();
        }
    }
}