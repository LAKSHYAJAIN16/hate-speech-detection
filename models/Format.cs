using logic.Classes;
using System.Text.RegularExpressions;

namespace logic.Formatters
{
    public class HateSpeechFormatter
    {
        public static (string[], string[]) FormatData(HateSpeechObject[] data)
        {
            //Initialize Final Data arrays
            string[] finalInput = new string[data.Length];
            string[] finalOutput = new string[data.Length];

            //Loop through all of the data
            for (int i = 0; i < data.Length; i++)
            {
                //Create Regex
                string regex = @"[^\w\s]";
                Regex RegexObject = new Regex(regex);

                //Use regex to format tweet
                string tweet = data[i].tweet;
                string formattedTweet = RegexObject.Replace(tweet, string.Empty);

                //Add it to our finalInput
                finalInput[i] = formattedTweet;

                //Hate Speech = 0, Offensive Speech = 1, Neither = 2
                int result = data[i].result;
                string stringResult = "Neither";

                //Check all Scenarios
                if (result == 0)
                {
                    stringResult = "Hate Speech";
                }
                else if (result == 1)
                {
                    stringResult = "Offensive Speech";
                }
                else if (result == 2)
                {
                    stringResult = "Neither";
                }

                //Add it to output array
                finalOutput[i] = stringResult;
            }

            return (finalInput, finalOutput);
        }
    }
}