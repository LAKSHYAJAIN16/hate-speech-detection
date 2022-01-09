using System.IO;
using System.Collections.Generic;
using logic.ML;
using logic.Classes;
using logic.Formatters;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace hate_speech_detection.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MLController : ControllerBase
    {
        private readonly ILogger<MLController> _logger;
        private readonly string jsonText;

        public MLController(ILogger<MLController> logger)
        {
            _logger = logger;
            jsonText = System.IO.File.ReadAllText(@"D:\Projects\machine-learning\hate-speech-detection\data\data.json");
        }

        [HttpGet]
        public string Get(string text)
        {
            //Create Bayes Classifier
            BayesClassifier classifier = new BayesClassifier();

            //Parse the JSON back to the object
            HateSpeechObject[] jsonData = JsonConvert.DeserializeObject<HateSpeechObject[]>(jsonText);

            //Format
            (string[], string[]) data = HateSpeechFormatter.FormatData(jsonData);

            //Feed it to Bayes Classifier
            classifier.Fit(data.Item1, data.Item2);

            //Now, predict our text
            (double, string) result = classifier.Predict(text);

            //Calculate Threshold
            double threshold = ThresholdCalculator.CalculateThreshold(-16, text);

            //Final Object
            Result finalResult = new Result();
            finalResult.prediction = result.Item2;
            finalResult.probability = result.Item1;
            finalResult.threshold = threshold;
            return JsonConvert.SerializeObject(finalResult);
        }
    }
}
