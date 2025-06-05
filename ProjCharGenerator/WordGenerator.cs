using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;

namespace generator
{
    public class WordGenerator
    {
        public static void GenerateText()
        {
            var wordFrequencies = new Dictionary<string, double>();
            var random = new Random();

            using (var reader = new StreamReader("ProjCharGenerator/data/words_input.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split('\t');
                    if (parts.Length >= 5)
                    {
                        var word = parts[1];
                        if (double.TryParse(parts[4], out double frequency))
                        {
                            wordFrequencies[word] = frequency;
                        }
                    }
                }
            }

            var totalFrequency = wordFrequencies.Values.Sum();
            var normalizedFrequencies = wordFrequencies.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value / totalFrequency
            );

            var words = new List<string>();
            for (int i = 0; i < 100; i++)
            {
                var r = random.NextDouble();
                var cumulative = 0.0;
                foreach (var kvp in normalizedFrequencies)
                {
                    cumulative += kvp.Value;
                    if (r <= cumulative)
                    {
                        words.Add(kvp.Key);
                        break;
                    }
                }
            }

            File.WriteAllText("Results/gen-2.txt", string.Join(" ", words));
        }

        public static void PlotWordDistribution()
        {
            var expectedFrequencies = new Dictionary<string, double>();
            var actualFrequencies = new Dictionary<string, double>();
            var random = new Random();

            using (var reader = new StreamReader("ProjCharGenerator/data/words_input.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split('\t');
                    if (parts.Length >= 5)
                    {
                        var word = parts[1];
                        if (double.TryParse(parts[4], out double frequency))
                        {
                            expectedFrequencies[word] = frequency;
                        }
                    }
                }
            }

            var totalExpected = expectedFrequencies.Values.Sum();
            var normalizedExpected = expectedFrequencies.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value / totalExpected
            );

            var words = new List<string>();
            for (int i = 0; i < 100; i++)
            {
                var r = random.NextDouble();
                var cumulative = 0.0;
                foreach (var kvp in normalizedExpected)
                {
                    cumulative += kvp.Value;
                    if (r <= cumulative)
                    {
                        words.Add(kvp.Key);
                        break;
                    }
                }
            }

            foreach (var word in words)
            {
                if (!actualFrequencies.ContainsKey(word))
                {
                    actualFrequencies[word] = 0;
                }
                actualFrequencies[word]++;
            }

            var totalActual = actualFrequencies.Values.Sum();
            var normalizedActual = actualFrequencies.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value / totalActual
            );

            var topWords = normalizedExpected
                .OrderByDescending(kvp => kvp.Value)
                .Take(20)
                .Select(kvp => kvp.Key)
                .ToList();

            var width = 1200;
            var height = 800;
            var margin = 50;
            var barWidth = (width - 2 * margin) / (topWords.Count * 2);

            using (var bitmap = new Bitmap(width, height))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                graphics.DrawLine(Pens.Black, margin, height - margin, width - margin, height - margin);
                graphics.DrawLine(Pens.Black, margin, margin, margin, height - margin);

                for (int i = 0; i < topWords.Count; i++)
                {
                    var word = topWords[i];
                    var expectedHeight = (float)(normalizedExpected[word] * (height - 2 * margin));
                    var actualHeight = (float)(normalizedActual.ContainsKey(word) 
                        ? normalizedActual[word] * (height - 2 * margin) 
                        : 0);

                    var x = margin + i * barWidth * 2;
                    var y = height - margin;

                    graphics.FillRectangle(Brushes.Blue, x, y - expectedHeight, barWidth, expectedHeight);
                    graphics.FillRectangle(Brushes.Red, x + barWidth, y - actualHeight, barWidth, actualHeight);

                    graphics.DrawString(word, new Font("Arial", 8), Brushes.Black, x, y + 5);
                }

                graphics.DrawString("Ожидаемые", new Font("Arial", 10, FontStyle.Bold), Brushes.Blue, margin, margin);
                graphics.DrawString("Реальные", new Font("Arial", 10, FontStyle.Bold), Brushes.Red, margin + 100, margin);

                bitmap.Save("Results/gen-2.png", ImageFormat.Png);
            }
        }
    }
} 