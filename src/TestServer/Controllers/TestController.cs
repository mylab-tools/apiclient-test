using Microsoft.AspNetCore.Mvc;

namespace TestServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly StringProcessorService _stringProcessorService;

        public TestController(StringProcessorService stringProcessorService)
        {
            _stringProcessorService = stringProcessorService;
        }

        [HttpGet("add-salt")]
        public IActionResult AddSalt([FromBody] string val)
        {
            return Ok(_stringProcessorService.Process(val));
        }

        [HttpGet("add-salt-to-header")]
        public IActionResult AddSaltToHeader([FromHeader(Name = "ArgumentHeader")] string val)
        {
            return Ok(_stringProcessorService.Process(val));
        }
    }
}
