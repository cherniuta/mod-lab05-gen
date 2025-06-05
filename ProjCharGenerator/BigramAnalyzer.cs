using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace generator
{
    public class BigramAnalyzer
    {
        public static void PlotBigramDistribution()
        {
            string inputPath = "ProjCharGenerator/data/bigrams_input.txt";
            string genPath = "Results/gen-1.txt";
            string outputPath = "Results/gen-1.png";

            Dictionary<string, double> expected = new Dictionary<string, double>();
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
                        expected[bigram] = freq;
                    }
                }
            }

            double totalExpected = expected.Values.Sum();
            foreach (var k in expected.Keys.ToList())
            {
                expected[k] /= totalExpected;
            }

            string text = File.ReadAllText(genPath, Encoding.UTF8).Trim();
            Dictionary<string, int> actual = new Dictionary<string, int>();
            for (int i = 0; i < text.Length - 1; i++)
            {
                string bg = text.Substring(i, 2);
                if (!actual.ContainsKey(bg))
                    actual[bg] = 0;
                actual[bg]++;
            }

            double totalActual = actual.Values.Sum();
            Dictionary<string, double> actualFreq = new Dictionary<string, double>();
            foreach (var k in actual.Keys)
            {
                actualFreq[k] = actual[k] / totalActual;
            }

            var topBigrams = expected.OrderByDescending(x => x.Value).Take(20).ToList();
            List<string> labels = topBigrams.Select(x => x.Key).ToList();
            List<double> expectedVals = topBigrams.Select(x => x.Value).ToList();
            List<double> actualVals = labels.Select(k => actualFreq.ContainsKey(k) ? actualFreq[k] : 0).ToList();

            int width = 1200;
            int height = 600;
            int margin = 50;
            int barWidth = 20;
            int spacing = 10;

            using (Bitmap bmp = new Bitmap(width, height))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.SmoothingMode = SmoothingMode.AntiAlias;

                g.DrawLine(Pens.Black, margin, height - margin, width - margin, height - margin);
                g.DrawLine(Pens.Black, margin, margin, margin, height - margin);

                double maxValue = Math.Max(expectedVals.Max(), actualVals.Max());
                double scale = (height - 2 * margin) / maxValue;

                for (int i = 0; i < labels.Count; i++)
                {
                    int x = margin + i * (barWidth * 2 + spacing);
                    int heightExpected = (int)(expectedVals[i] * scale);
                    int heightActual = (int)(actualVals[i] * scale);

                    g.FillRectangle(Brushes.Blue, x, height - margin - heightExpected, barWidth, heightExpected);
                    g.FillRectangle(Brushes.Red, x + barWidth, height - margin - heightActual, barWidth, heightActual);

                    g.DrawString(labels[i], new Font("Arial", 8), Brushes.Black, x, height - margin + 5);
                }

                g.FillRectangle(Brushes.Blue, margin, 20, 20, 20);
                g.DrawString("Ожидаемые", new Font("Arial", 10), Brushes.Black, margin + 30, 20);
                g.FillRectangle(Brushes.Red, margin + 150, 20, 20, 20);
                g.DrawString("Реальные", new Font("Arial", 10), Brushes.Black, margin + 180, 20);

                bmp.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }
    }
} 