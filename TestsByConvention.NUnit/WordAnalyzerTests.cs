using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TestsByConvention.Lib;

namespace TestsByConvention.NUnit;
public class WordAnalyzerTests
{
    [Test]
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

    [Test]
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

    [Test]
    [TestCase("NUnit", 5, "i1,n2,t1,u1")]
    [TestCase("Test", 4, "e1,s1,t2")]
    public void AnalyzerWorksForWordsFromAttributes(string word, int wordLength, string letterCount)
    {
        var expected = new WordAnalysis(
            word,
            wordLength,
            letterCount.Split(',').ToDictionary(pair => pair[0], pair => int.Parse(pair.Substring(1)))
        );

        var actual = new WordAnalyzer().Analyze(word);

        actual.Should().BeEquivalentTo(expected);
    }

    private static IEnumerable<WordAnalysis> GetAnalyzerWords()
    {
        yield return new ("NUnit", 5, new Dictionary<char, int>
        {
            ['i'] = 1,
            ['n'] = 2,
            ['t'] = 1,
            ['u'] = 1,
        });
        yield return new ("Test", 4, new Dictionary<char, int>
        {
            ['e'] = 1,
            ['s'] = 1,
            ['t'] = 2,
        });
    }

    [Test]
    [TestCaseSource(nameof(GetAnalyzerWords))]
    public void AnalyzerWorksForWordsFromMethod(WordAnalysis expected)
    {
        var actual = new WordAnalyzer().Analyze(expected.Word);

        actual.Should().BeEquivalentTo(expected);
    }

    private static IEnumerable<TestCaseData> ReadAnalyzerWords()
    {
        var testFilesFolder = Path.Combine(TestContext.CurrentContext.TestDirectory, "WordAnalysis");
        var testFiles = Directory.GetFiles(testFilesFolder);
        foreach (var testFile in testFiles)
        {
            var json = File.ReadAllText(testFile);
            var wordAnalysis = JsonSerializer.Deserialize<WordAnalysis>(json);
            if (wordAnalysis != null)
            {
                yield return new TestCaseData(Path.GetFileNameWithoutExtension(testFile), wordAnalysis);
            }
        }
    }

    [Test]
    [TestCaseSource(nameof(ReadAnalyzerWords))]
    public void AnalyzerWorksForWordsFromFiles(string word, WordAnalysis expected)
    {
        var actual = new WordAnalyzer().Analyze(word);

        actual.Should().BeEquivalentTo(expected);
    }
}