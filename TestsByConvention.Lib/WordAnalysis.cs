namespace TestsByConvention.Lib;

public record WordAnalysis(string Word, int Length, IReadOnlyDictionary<char, int> LetterCount);
