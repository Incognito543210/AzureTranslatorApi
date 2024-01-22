using AzureTranslatorApi.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;

namespace AzureTranslatorApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly FileService _fileService;

        public FilesController(FileService fileService)
        {
            _fileService = fileService;
        }
        
        [HttpGet]
        [Route("Inputs")]
        public async Task<IActionResult> ListAllInputsBlobs()
        {
            var result = await _fileService.ListInputsAsync();
            return Ok(result);
        }
        
        [HttpGet]
        [Route("Outputs")]
        public async Task<IActionResult> ListAllOutputsBlobs()
        {
            var result = await _fileService.ListOutputsAsync();
            return Ok(result);
        }

        [HttpGet]
        [Route("Upload/{filePath}")]
        public async Task<IActionResult> Upload(String filePath)
        {
            var result = await _fileService.UploadAsync(filePath);
            return Ok(result);
        }

        [HttpGet]
        [Route("Download")]
        public async Task<IActionResult> Download()
        {
            var result = await _fileService.DownloadAsync();
            if (result != null)
            {
                return Ok(result.FilePath);
            }
            return NotFound();
        }

        [HttpDelete]
        [Route("Inputs")]
        public async Task<IActionResult> DeleteInputs()
        {
            var result = await _fileService.DeleteInputsAsync();
            return Ok(result);
        }

        [HttpDelete]
        [Route("Outputs")]
        public async Task<IActionResult> DeleteOutputs()
        {
            var result = await _fileService.DeleteOutputsAsync();
            return Ok(result);
        }
    }
}
