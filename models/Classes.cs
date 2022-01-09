using Newtonsoft.Json;

namespace logic.Classes
{
    public class HateSpeechObject
    {
        public int count;
        public int hate_speech;
        public int offensive_speech;
        public int neither;

        [JsonProperty("class")]
        public int result;

        public string tweet;
    }

    public class Result
    {
        public double probability;
        public double threshold;
        public string prediction;
    }
}