using System;

namespace logic.ML
{
    public class ThresholdCalculator
    {
        public static double CalculateThreshold(double constant, string text)
        {
            // Get Amount of words in text
            int wordCount = text.Split(' ').Length;

            // Equation (constant * (-π * wordCount))
            double threshold = constant * (Math.PI / wordCount);
            return threshold;
        }
    }
}