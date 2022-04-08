using Microsoft.AspNetCore.Mvc;
using WebLibrary;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileManagerController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public FileManagerController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [DisableRequestSizeLimit]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FilePostModel viewModel)
        {
            using (var fs = new FileStream($@"C:\Users\arahk\source\repos\ktb.centralize\WebApi\bin\Debug\net6.0\{viewModel.FileName}{viewModel.FileExtension}", FileMode.CreateNew))
            {
                await fs.WriteAsync(viewModel.Content.AsMemory(0, viewModel.Content.Length));
            }

            return Ok();
        }
    }
}