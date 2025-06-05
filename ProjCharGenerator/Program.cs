using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace generator
{
    public class Program
    {
        static void Main(string[] args)
        {
            GenerateText();
            BigramAnalyzer.PlotBigramDistribution();
            WordGenerator.GenerateText();
            WordGenerator.PlotWordDistribution();
        }

        public static void GenerateText(string inputPath, string outputPath)
        {
            Dictionary<string, double> bigramProbs = new Dictionary<string, double>();
            using (StreamReader sr = new StreamReader(inputPath, Encoding.UTF8))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Trim().Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 3)
                    {
                        string bigram = parts[1];
                        double freq = double.Parse(parts[2]);
                        bigramProbs[bigram] = freq;
                    }
                }
            }

            double total = bigramProbs.Values.Sum();
            foreach (var k in bigramProbs.Keys.ToList())
            {
                bigramProbs[k] /= total;
            }

            Random rand = new Random();
            StringBuilder text = new StringBuilder();
            string currentBigram = bigramProbs.Keys.ElementAt(rand.Next(bigramProbs.Count));
            text.Append(currentBigram);

            while (text.Length < 1000)
            {
                string nextChar = currentBigram[1].ToString();
                var possibleNext = bigramProbs.Keys.Where(k => k.StartsWith(nextChar)).ToList();
                if (possibleNext.Count == 0) break;

                double[] probs = possibleNext.Select(k => bigramProbs[k]).ToArray();
                double sum = probs.Sum();
                for (int i = 0; i < probs.Length; i++)
                {
                    probs[i] /= sum;
                }

                double r = rand.NextDouble();
                double cumsum = 0;
                int selected = 0;
                for (int i = 0; i < probs.Length; i++)
                {
                    cumsum += probs[i];
                    if (r <= cumsum)
                    {
                        selected = i;
                        break;
                    }
                }

                currentBigram = possibleNext[selected];
                text.Append(currentBigram[1]);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            File.WriteAllText(outputPath, text.ToString(), Encoding.UTF8);
        }

        public static void GenerateText()
        {
            string inputPath = "ProjCharGenerator/data/bigrams_input.txt";
            string outputPath = "Results/gen-1.txt";
            GenerateText(inputPath, outputPath);
        }
    }
}

