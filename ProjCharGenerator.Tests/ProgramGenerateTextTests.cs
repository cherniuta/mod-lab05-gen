using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Xunit;
using generator;

namespace ProjCharGenerator.Tests
{
    public class ProgramGenerateTextTests
    {
        private string GetTestFilePath(string testName)
        {
            return Path.Combine("TestResults", $"output_{testName}_{Guid.NewGuid()}.txt");
        }

        private string CallGenerateText(string testName)
        {
            string inputPath = Path.Combine("TestData", "bigrams_test.txt");
            string outputPath = GetTestFilePath(testName);
            Program.GenerateText(inputPath, outputPath);
            return outputPath;
        }

        private void CreateTestInputFile(string content)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine("TestData", "bigrams_test.txt")));
            File.WriteAllText(Path.Combine("TestData", "bigrams_test.txt"), content, Encoding.UTF8);
        }

        private void CleanupTestFiles(string outputPath)
        {
            try
            {
                if (File.Exists(outputPath))
                {
                    File.Delete(outputPath);
                }
            }
            catch (Exception)
            {
                // Ignore cleanup errors
            }
        }

        [Fact]
        public void GenerateText_CreatesOutputFile()
        {
            CreateTestInputFile("1\tаа\t0.1\n2\tаб\t0.2\n3\tба\t0.3\n4\tбб\t0.4");
            string outputPath = CallGenerateText(nameof(GenerateText_CreatesOutputFile));
            Assert.True(File.Exists(outputPath));
            CleanupTestFiles(outputPath);
        }

        [Fact]
        public void GenerateText_OutputIsNotEmpty()
        {
            CreateTestInputFile("1\tаа\t0.1\n2\tаб\t0.2\n3\tба\t0.3\n4\tбб\t0.4");
            string outputPath = CallGenerateText(nameof(GenerateText_OutputIsNotEmpty));
            string output = File.ReadAllText(outputPath, Encoding.UTF8);
            Assert.NotEmpty(output);
            CleanupTestFiles(outputPath);
        }

        [Fact]
        public void GenerateText_OutputLengthAtLeast1000()
        {
            CreateTestInputFile("1\tаа\t0.1\n2\tаб\t0.2\n3\tба\t0.3\n4\tбб\t0.4");
            string outputPath = CallGenerateText(nameof(GenerateText_OutputLengthAtLeast1000));
            string output = File.ReadAllText(outputPath, Encoding.UTF8);
            Assert.True(output.Length >= 1000);
            CleanupTestFiles(outputPath);
        }

        [Fact]
        public void GenerateText_OnlyAllowedCharacters()
        {
            CreateTestInputFile("1\tаа\t0.1\n2\tаб\t0.2\n3\tба\t0.3\n4\tбб\t0.4");
            string outputPath = CallGenerateText(nameof(GenerateText_OnlyAllowedCharacters));
            string output = File.ReadAllText(outputPath, Encoding.UTF8);
            var allowedChars = new HashSet<char>("абвгдеёжзийклмнопрстуфхцчшщъыьэюя");
            Assert.All(output, c => Assert.Contains(c, allowedChars));
            CleanupTestFiles(outputPath);
        }

        [Fact]
        public void GenerateText_StartsWithValidBigram()
        {
            CreateTestInputFile("1\tаа\t0.1\n2\tаб\t0.2\n3\tба\t0.3\n4\tбб\t0.4");
            string outputPath = CallGenerateText(nameof(GenerateText_StartsWithValidBigram));
            string output = File.ReadAllText(outputPath, Encoding.UTF8);
            var validBigrams = new[] { "аа", "аб", "ба", "бб" };
            Assert.Contains(output.Substring(0, 2), validBigrams);
            CleanupTestFiles(outputPath);
        }

        [Fact]
        public void GenerateText_OnlyValidBigramsInText()
        {
            CreateTestInputFile("1\tаа\t0.1\n2\tаб\t0.2\n3\tба\t0.3\n4\tбб\t0.4");
            string outputPath = CallGenerateText(nameof(GenerateText_OnlyValidBigramsInText));
            string output = File.ReadAllText(outputPath, Encoding.UTF8);
            var validBigrams = new[] { "аа", "аб", "ба", "бб" };
            
            for (int i = 0; i < output.Length - 1; i++)
            {
                string bigram = output.Substring(i, 2);
                Assert.Contains(bigram, validBigrams);
            }
            CleanupTestFiles(outputPath);
        }
    }
}