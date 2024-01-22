using AzureTranslatorApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AzureTranslatorApi.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class TranslatorController : ControllerBase
    {
        private readonly ITranslator _translator;

        public TranslatorController(ITranslator translator)
        {
            _translator = translator;
        }


        [HttpGet("{inL},{outL}")]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(400)]
        public IActionResult Translate(string inL, string outL)
        {

            var Info = _translator.TranslateAsync(inL, outL);
            

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(Info.ToString());
        }





    }
}
