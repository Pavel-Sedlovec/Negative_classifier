using Api.Services.ClassifyTextService;
using Core.Model;
using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PredictController : Controller
    {
        private IClassifyText _classifyText;

        private const int NEGATIVE_SENTIMENT = 1;
        private const int POSITIVE_SENTIMENT = 0;
        public PredictController(IClassifyText classifyext)
        {
            _classifyText = classifyext;
        }

        [HttpPost("Classify")]
        public IActionResult Classify([FromBody] string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return BadRequest("Text bad");
            }

            int result = _classifyText.Classify(text);

            return Ok(new
            {
                Source = text,
                Label = result,
                Sentiment = result == NEGATIVE_SENTIMENT ? "Negative" : "Positive"
            });
        }

        [HttpPost("ClassifyWithConfidence")]
        public IActionResult ClassifyWithConfidence([FromBody] string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return BadRequest("Text bad");
            }
            int result;
            double confidence;
            (result, confidence) = _classifyText.ClassifyWithConfidence(text);
                       
            return Ok(new
            {
                Source = text,
                Label = result,
                Sentiment = result == NEGATIVE_SENTIMENT ? "Negative" : "Positive",
                Confidence = confidence
            });
        }

        [HttpPost("ClassifyBath")]
        public IActionResult ClassifyBath([FromBody] List<string> texts)
        {
            var results = _classifyText.ClassifyBatch(texts);

            var responses = new List<object>();

            for(int i = 0; i < texts.Count; i++)
            {
                responses.Add(new
                {
                    Text = texts[i],
                    Label = results[i],
                    Sentiment = results[i] == NEGATIVE_SENTIMENT ? "Negative" : "Positive"
                });
            }

            return Ok(responses);
        }
    }
}
