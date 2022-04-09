using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services.Dispute;
using WebApi.Services.Excel;
using WebLibrary;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileManagerController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IFileByteImporter fileByteImporter;

        public FileManagerController(ILogger<WeatherForecastController> logger,
                                     IFileByteImporter fileByteImporter)
        {
            _logger = logger;
            this.fileByteImporter = fileByteImporter;
        }

        [DisableRequestSizeLimit]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FilePostModel viewModel)
        {
            var tupleDataTables = fileByteImporter.Import(viewModel.Content);   
            var disputeAtmTransformer = new Transformer(DisputeTypes.ATM, "Branch", "TERM_ID", "CREATE_DATE");
            var disputeRcmTransformer = new Transformer(DisputeTypes.RCM, "Branch", "TERM_ID", "CREATE_DATE");
            var disputeAtm = disputeAtmTransformer.Transform(tupleDataTables.Item1).ToList();
            var disputeRcm = disputeRcmTransformer.Transform(tupleDataTables.Item2).ToList();
            var disputes = disputeAtm.Concat(disputeRcm).ToArray();

            return new JsonResult(disputes, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}