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
        private static string InputPath = Path.Combine("TestData", "bigrams_test.txt");
        private static string OutputPath = Path.Combine("TestResults", "output_test.txt");

        private void CallGenerateText()
        {
            Program.GenerateText(InputPath, OutputPath);
        }

        private void CreateTestInputFile(string content)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(InputPath));
            File.WriteAllText(InputPath, content, Encoding.UTF8);
        }

        private void CleanupTestFiles()
        {
            if (File.Exists(InputPath)) File.Delete(InputPath);
            if (File.Exists(OutputPath)) File.Delete(OutputPath);
            
            var inputDir = Path.GetDirectoryName(InputPath);
            var outputDir = Path.GetDirectoryName(OutputPath);
            if (Directory.Exists(inputDir) && !Directory.EnumerateFileSystemEntries(inputDir).Any())
                Directory.Delete(inputDir);
            if (Directory.Exists(outputDir) && !Directory.EnumerateFileSystemEntries(outputDir).Any())
                Directory.Delete(outputDir);
        }

        [Fact]
        public void GenerateText_CreatesOutputFile()
        {
            CreateTestInputFile("1\tаа\t0.1\n2\tаб\t0.2\n3\tба\t0.3\n4\tбб\t0.4");
            CallGenerateText();
            Assert.True(File.Exists(OutputPath));
            CleanupTestFiles();
        }

        [Fact]
        public void GenerateText_OutputIsNotEmpty()
        {
            CreateTestInputFile("1\tаа\t0.1\n2\tаб\t0.2\n3\tба\t0.3\n4\tбб\t0.4");
            CallGenerateText();
            string output = File.ReadAllText(OutputPath, Encoding.UTF8);
            Assert.NotEmpty(output);
            CleanupTestFiles();
        }

        [Fact]
        public void GenerateText_OutputLengthAtLeast1000()
        {
            CreateTestInputFile("1\tаа\t0.1\n2\tаб\t0.2\n3\tба\t0.3\n4\tбб\t0.4");
            CallGenerateText();
            string output = File.ReadAllText(OutputPath, Encoding.UTF8);
            Assert.True(output.Length >= 1000);
            CleanupTestFiles();
        }

        [Fact]
        public void GenerateText_OnlyAllowedCharacters()
        {
            CreateTestInputFile("1\tаа\t0.1\n2\tаб\t0.2\n3\tба\t0.3\n4\tбб\t0.4");
            CallGenerateText();
            string output = File.ReadAllText(OutputPath, Encoding.UTF8);
            Assert.All(output, c => Assert.Contains(c, "аб"));
            CleanupTestFiles();
        }

        [Fact]
        public void GenerateText_StartsWithValidBigram()
        {
            CreateTestInputFile("1\tаа\t0.1\n2\tаб\t0.2\n3\tба\t0.3\n4\tбб\t0.4");
            CallGenerateText();
            string output = File.ReadAllText(OutputPath, Encoding.UTF8);
            string firstBigram = output.Substring(0, 2);
            Assert.Contains(firstBigram, new[] { "аа", "аб", "ба", "бб" });
            CleanupTestFiles();
        }

        [Fact]
        public void GenerateText_OnlyValidBigramsInText()
        {
            CreateTestInputFile("1\tаа\t0.1\n2\tаб\t0.2\n3\tба\t0.3\n4\tбб\t0.4");
            CallGenerateText();
            string output = File.ReadAllText(OutputPath, Encoding.UTF8);
            var validBigrams = new[] { "аа", "аб", "ба", "бб" };
            for (int i = 0; i < output.Length - 1; i++)
            {
                string bigram = output.Substring(i, 2);
                Assert.Contains(bigram, validBigrams);
            }
            CleanupTestFiles();
        }
    }
}
