namespace TestsByConvention.Lib;

public class WordAnalyzer
{
    public WordAnalysis Analyze(string word)
    {
        return new WordAnalysis(
            word,
            word.Length,
            word.ToLowerInvariant()
                .GroupBy(letter => letter)
                .OrderBy(letter => letter.Key)
                .ToDictionary(letter => letter.Key, letter => letter.Count())
        );
    }
}
