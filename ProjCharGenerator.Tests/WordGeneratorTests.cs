using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Xunit;
using generator;

namespace ProjCharGenerator.Tests
{
    public class WordGeneratorTests
    {
        private string GetTestFilePath(string testName)
        {
            return Path.Combine("TestResults", $"output_{testName}_{Guid.NewGuid()}.txt");
        }

        private string CallGenerateText(string testName)
        {
            string inputPath = Path.Combine("TestData", "words_test.txt");
            string outputPath = GetTestFilePath(testName);
            WordGenerator.GenerateText(inputPath, outputPath);
            return outputPath;
        }

        private void CreateTestInputFile(string content)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine("TestData", "words_test.txt")));
            File.WriteAllText(Path.Combine("TestData", "words_test.txt"), content, Encoding.UTF8);
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
            CreateTestInputFile("1\tи\tи\tconj\t35801.8\n2\tв\tв\tpr\t31374.2\n3\tне\tне\tpart\t18028.0");
            string outputPath = CallGenerateText(nameof(GenerateText_CreatesOutputFile));
            Assert.True(File.Exists(outputPath));
            CleanupTestFiles(outputPath);
        }

        [Fact]
        public void GenerateText_OutputIsNotEmpty()
        {
            CreateTestInputFile("1\tи\tи\tconj\t35801.8\n2\tв\tв\tpr\t31374.2\n3\tне\tне\tpart\t18028.0");
            string outputPath = CallGenerateText(nameof(GenerateText_OutputIsNotEmpty));
            string output = File.ReadAllText(outputPath, Encoding.UTF8);
            Assert.NotEmpty(output);
            CleanupTestFiles(outputPath);
        }

        [Fact]
        public void GenerateText_Generates100Words()
        {
            CreateTestInputFile("1\tи\tи\tconj\t35801.8\n2\tв\tв\tpr\t31374.2\n3\tне\tне\tpart\t18028.0");
            string outputPath = CallGenerateText(nameof(GenerateText_Generates100Words));
            string output = File.ReadAllText(outputPath, Encoding.UTF8);
            var words = output.Split(' ');
            Assert.Equal(100, words.Length);
            CleanupTestFiles(outputPath);
        }

        [Fact]
        public void GenerateText_OnlyUsesWordsFromInput()
        {
            CreateTestInputFile("1\tи\tи\tconj\t35801.8\n2\tв\tв\tpr\t31374.2\n3\tне\tне\tpart\t18028.0");
            string outputPath = CallGenerateText(nameof(GenerateText_OnlyUsesWordsFromInput));
            string output = File.ReadAllText(outputPath, Encoding.UTF8);
            var words = output.Split(' ');
            var validWords = new[] { "и", "в", "не" };
            Assert.All(words, word => Assert.Contains(word, validWords));
            CleanupTestFiles(outputPath);
        }

        [Fact]
        public void GenerateText_RespectsWordFrequencies()
        {
            CreateTestInputFile("1\tи\tи\tconj\t100.0\n2\tв\tв\tpr\t50.0\n3\tне\tне\tpart\t10.0");
            string outputPath = CallGenerateText(nameof(GenerateText_RespectsWordFrequencies));
            string output = File.ReadAllText(outputPath, Encoding.UTF8);
            var words = output.Split(' ');
            var wordCounts = words.GroupBy(w => w).ToDictionary(g => g.Key, g => g.Count());
            
            Assert.True(wordCounts["и"] > wordCounts["в"]);
            Assert.True(wordCounts["в"] > wordCounts["не"]);
            CleanupTestFiles(outputPath);
        }

        [Fact]
        public void GenerateText_HandlesEmptyInput()
        {
            CreateTestInputFile("");
            string outputPath = GetTestFilePath(nameof(GenerateText_HandlesEmptyInput));
            Assert.Throws<InvalidOperationException>(() => WordGenerator.GenerateText(
                Path.Combine("TestData", "words_test.txt"), 
                outputPath));
            CleanupTestFiles(outputPath);
        }
    }
}