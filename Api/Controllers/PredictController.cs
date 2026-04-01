using Core.Model;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PredictController : Controller
    {
        private DataModel _dataModel;
        private double[] _vectorData;
        public PredictController(DataModel dataModel)
        {
            _dataModel = dataModel;
        }

        [HttpPost("Classify")]
        public IActionResult Classify([FromBody] string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return BadRequest("Text bad");
            }

            string cleanText = CleanerData.FullClean(text);
            _vectorData = VectorizationData.VectorizeSingle(cleanText, _dataModel);

            int result = SVM.PredictStatic(_vectorData, _dataModel);

            return Ok(new
            {
                Source = text,
                Lable = result,
                Sentiment = result == 1 ? "Negative" : "Positive"
            });
        }
    }
}
