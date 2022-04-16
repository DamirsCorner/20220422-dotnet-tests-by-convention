using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TestsByConvention.Lib;

namespace TestsByConvention.MSTest;

[TestClass]
public class WordAnalyzerTests
{
    [TestMethod]
    public void AnalyzeWorksForNUnit()
    {
        var word = "NUnit";
        var expected = new WordAnalysis(word, 5, new Dictionary<char, int>
        {
            ['i'] = 1,
            ['n'] = 2,
            ['t'] = 1,
            ['u'] = 1,
        });

        var actual = new WordAnalyzer().Analyze(word);

        actual.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void AnalyzeWorksForTest()
    {
        var word = "Test";
        var expected = new WordAnalysis(word, 4, new Dictionary<char, int>
        {
            ['e'] = 1,
            ['s'] = 1,
            ['t'] = 2,
        });

        var actual = new WordAnalyzer().Analyze(word);

        actual.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    [DataRow("NUnit", 5, "i1,n2,t1,u1")]
    [DataRow("Test", 4, "e1,s1,t2")]
    public void AnalyzerWorksForWOrdsFromAttributes(string word, int wordLength, string letterCount)
    {
        var expected = new WordAnalysis(
            word,
            wordLength,
            letterCount.Split(',').ToDictionary(pair => pair[0], pair => int.Parse(pair.Substring(1)))
        );

        var actual = new WordAnalyzer().Analyze(word);

        actual.Should().BeEquivalentTo(expected);
    }

    private static IEnumerable<object[]> GetAnalyzerWords()
    {
        yield return new object[]
        {
            new WordAnalysis("NUnit", 5, new Dictionary<char, int>
            {
                ['i'] = 1,
                ['n'] = 2,
                ['t'] = 1,
                ['u'] = 1,
            })
        };
        yield return new object[]
        {
            new WordAnalysis("Test", 4, new Dictionary<char, int>
            {
                ['e'] = 1,
                ['s'] = 1,
                ['t'] = 2,
            })
        };
    }

    [TestMethod]
    [DynamicData(nameof(GetAnalyzerWords), DynamicDataSourceType.Method)]
    public void AnalyzerWorksForWordsFromMethod(WordAnalysis expected)
    {
        var actual = new WordAnalyzer().Analyze(expected.Word);

        actual.Should().BeEquivalentTo(expected);
    }

    private static TestContext _testContext = null!;

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
        _testContext = testContext;
    }

    private static IEnumerable<object[]> ReadAnalyzerWords()
    {
        var testFilesFolder = Path.Combine(_testContext.DeploymentDirectory, "WordAnalysis");
        var testFiles = Directory.GetFiles(testFilesFolder);
        foreach (var testFile in testFiles)
        {
            var json = File.ReadAllText(testFile);
            var wordAnalysis = JsonSerializer.Deserialize<WordAnalysis>(json);
            if (wordAnalysis != null)
            {
                yield return new object[] { Path.GetFileNameWithoutExtension(testFile), wordAnalysis };
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(ReadAnalyzerWords), DynamicDataSourceType.Method)]
    public void AnalyzerWorksForWordsFromFiles(string word, WordAnalysis expected)
    {
        var actual = new WordAnalyzer().Analyze(word);

        actual.Should().BeEquivalentTo(expected);
    }
}