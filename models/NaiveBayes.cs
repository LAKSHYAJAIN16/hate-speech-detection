using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace logic.ML
{
    public class BayesClassifier
    {
        public BayesInfo Info;

        public BayesClassifier()
        {
            Info = new BayesInfo();

            Info.Vocab = new Dictionary<string, bool>();
            Info.VocabSize = 0;

            Info.TotalDocs = 0;
            Info.DocCount = new Dictionary<string, int>();

            Info.WordCount = new Dictionary<string, int>();
            Info.WordFreqCount = new Dictionary<string, Dictionary<string, int>>();

            Info.Categories = new Dictionary<string, bool>();
        }

        public void Fit(string[] input, string[] output)
        {
            for (int i = 0; i < input.Length; i++)
            {
                string target = output[i];
                string test_obj = input[i];
                LearnCategory(test_obj, target);
            }
        }

        public (double, string) Predict(string text)
        {
            //Define Max Probability
            double maxProb = double.NegativeInfinity;

            //Define Chosen Category
            string chosenCategory = "";

            //Filter Text
            string[] words = Filter(text);
            Dictionary<string, int> freqTable = GetFrequencyTable(words);

            //Seperate Keys
            string[] keys = Info.Categories.Keys.ToArray();

            //Loop through all keys
            for (int k = 0; k < keys.Length; k++)
            {
                //Calculating overall probability of category
                double categoryProb = (double)Info.DocCount[keys[k]] / Info.TotalDocs;

                //Calculate Log to avoid underflow, and use the Naive Bayes formula
                double logProb = Math.Log10(categoryProb);

                //Get The Frequency Keys
                string[] fkeys = freqTable.Keys.ToArray();

                //Calculate P(W|C) for each word according to the Naive Bayes Formula, using ANOTHER Log
                for (int l = 0; l < fkeys.Length; l++)
                {
                    double freqInText = freqTable[fkeys[l]];
                    double wordProb = TokenProbability(fkeys[l], keys[k]);
                    logProb += freqInText * Math.Log10(wordProb);
                }

                //If our probability beats our max prob, set it as our category
                if (logProb > maxProb)
                {
                    maxProb = logProb;
                    chosenCategory = keys[k];
                }
            }

            return (maxProb, chosenCategory);
        }

        private void LearnCategory(string text, string category)
        {
            //Initializes Category if it doesn't exist
            InitalizeCategory(category);

            //Increase the amount of documents for the individual category.
            Info.DocCount[category]++;

            //Increase the amount of documents for the total classifier.
            Info.TotalDocs++;

            //Splits the words up into a list of strings, as well as removing all special characters.
            string[] words = Filter(text);

            //Gets the frequency table for all words.
            Dictionary<string, int> freqTable = GetFrequencyTable(words);

            //Turn Keys into an array
            string[] keys = freqTable.Keys.ToArray();

            //Cycle through and increment frequency
            for (int k = 0; k < keys.Length; k++)
            {
                if (!Info.Vocab.ContainsKey(keys[k]))
                {
                    Info.Vocab[keys[k]] = true;
                    Info.VocabSize++;
                }

                int freqInText = freqTable[keys[k]];

                if (!Info.WordFreqCount[category].ContainsKey(keys[k]))
                {
                    Info.WordFreqCount[category].Add(keys[k], freqInText);
                }
                else
                {
                    Info.WordFreqCount[category][keys[k]] += freqInText;
                }

                Info.WordCount[category] += freqInText;
            }
        }

        public double TokenProbability(string word, string category, double k = 0.0001)
        {
            //Define word Frequency Count
            double wordFreqCount;

            //If we Don't have the key, make the frequency 0;
            if (!Info.WordFreqCount[category].ContainsKey(word))
            {
                wordFreqCount = 0;
            }

            //If we have it, check the count
            else
            {
                wordFreqCount = Info.WordFreqCount[category][word];
            }

            //Calculate Word Count
            double wordCount = Info.WordCount[category];

            //Return Percentage
            return (wordFreqCount + k) / (wordCount + Info.VocabSize);
        }

        public string[] Filter(string text)
        {   // Create String Builder
            StringBuilder sb = new StringBuilder();

            //Remove unwanted strings
            foreach (char c in text)
            {
                if (!char.IsPunctuation(c))
                    if (c == '\t' || c == '\n')
                    {
                        sb.Append(' ');
                    }
                    else
                    {
                        sb.Append(c);
                    }

            }

            //Get Output
            string output = sb.ToString();

            //Format Output
            return output.ToLower().Split(' ').Where(x => !string.IsNullOrEmpty(x) && !(x == " ")).ToArray();
        }

        public void InitalizeCategory(string name)
        {
            // Initializes category if the category does not already exist.
            if (!Info.Categories.ContainsKey(name))
            {
                Info.DocCount.Add(name, 0);
                Info.WordCount.Add(name, 0);
                Info.WordFreqCount.Add(name, new Dictionary<string, int>());
                Info.Categories.Add(name, true);
            }
        }

        public Dictionary<string, int> GetFrequencyTable(string[] words)
        {
            //Create new Dictionary
            Dictionary<string, int> freqTable = new Dictionary<string, int>();

            //Loop through and check whether that word has occured before
            for (int k = 0; k < words.Length; k++)
            {
                if (!freqTable.ContainsKey(words[k]))
                {
                    freqTable.Add(words[k], 1);
                }
                else
                {
                    freqTable[words[k]]++;
                }
            }

            //Return freqTable
            return freqTable;
        }
    }

    public class BayesInfo
    {
        public Dictionary<string, bool> Categories;
        public Dictionary<string, int> DocCount;
        public int TotalDocs;

        public Dictionary<string, bool> Vocab;
        public int VocabSize;

        public Dictionary<string, int> WordCount;
        public Dictionary<string, Dictionary<string, int>> WordFreqCount;
    }
}