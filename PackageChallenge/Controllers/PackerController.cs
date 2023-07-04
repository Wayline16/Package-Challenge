using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

///////////////////////////////////////////////////////Wayline Jeffries/////////////////////////////////////////////////////////////

namespace PackageChallenge.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PackerController : ControllerBase
    {
        private readonly ILogger<PackerController> _logger;

        public PackerController(ILogger<PackerController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetPackerController")]
        public string Get()
        {
            /// string filepath = "C:/Users/user/Documents/stt_skeleton (2) (1).tar/stt_skeleton/resources/example_input";
            string filepath = "C:/Users/user/Pictures/API/Package Challenge/PackageChallenge/ExampleInput/example_input";
            string result = Packer.Pack(filepath);

            return result;
        }

    }
}
