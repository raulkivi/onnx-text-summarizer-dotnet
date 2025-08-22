using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TextSummarizer
{
    public class TextSummarizer
    {
        private readonly int _maxSentences;
        
        public TextSummarizer(int maxSentences = 3)
        {
            _maxSentences = maxSentences;
        }

        public string SummarizeText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Split text into sentences
            var sentences = SplitIntoSentences(text);
            
            if (sentences.Count <= _maxSentences)
                return text;

            // Score sentences based on word frequency and position
            var sentenceScores = ScoreSentences(sentences);
            
            // Select top sentences
            var topSentences = sentenceScores
                .OrderByDescending(kvp => kvp.Value)
                .Take(_maxSentences)
                .OrderBy(kvp => sentences.IndexOf(kvp.Key))
                .Select(kvp => kvp.Key)
                .ToList();

            return string.Join(" ", topSentences);
        }

        private List<string> SplitIntoSentences(string text)
        {
            // Simple sentence splitting on periods, exclamation marks, and question marks
            var sentences = Regex.Split(text, @"[.!?]+")
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s) && s.Length > 10)
                .ToList();

            return sentences;
        }

        private Dictionary<string, double> ScoreSentences(List<string> sentences)
        {
            var wordFrequency = CalculateWordFrequency(sentences);
            var scores = new Dictionary<string, double>();

            for (int i = 0; i < sentences.Count; i++)
            {
                var sentence = sentences[i];
                var words = GetWords(sentence);
                
                if (words.Count == 0)
                {
                    scores[sentence] = 0;
                    continue;
                }

                // Calculate average word frequency score
                double totalScore = words.Sum(word => wordFrequency.GetValueOrDefault(word.ToLower(), 0));
                double avgScore = totalScore / words.Count;
                
                // Boost score for sentences at the beginning (position bias)
                double positionBoost = 1.0 - (double)i / sentences.Count * 0.3;
                
                // Boost score for sentences with certain keywords
                double keywordBoost = ContainsKeywords(sentence) ? 1.2 : 1.0;
                
                scores[sentence] = avgScore * positionBoost * keywordBoost;
            }

            return scores;
        }

        private Dictionary<string, int> CalculateWordFrequency(List<string> sentences)
        {
            var frequency = new Dictionary<string, int>();
            var stopWords = GetStopWords();

            foreach (var sentence in sentences)
            {
                var words = GetWords(sentence);
                
                foreach (var word in words)
                {
                    var lowerWord = word.ToLower();
                    if (!stopWords.Contains(lowerWord) && lowerWord.Length > 2)
                    {
                        frequency[lowerWord] = frequency.GetValueOrDefault(lowerWord, 0) + 1;
                    }
                }
            }

            return frequency;
        }

        private List<string> GetWords(string sentence)
        {
            return Regex.Matches(sentence, @"\b\w+\b")
                .Cast<Match>()
                .Select(m => m.Value)
                .ToList();
        }

        private bool ContainsKeywords(string sentence)
        {
            var keywords = new[] { "important", "significant", "key", "main", "primary", "essential", "critical", "major", "conclusion", "result" };
            var lowerSentence = sentence.ToLower();
            return keywords.Any(keyword => lowerSentence.Contains(keyword));
        }

        private HashSet<string> GetStopWords()
        {
            return new HashSet<string>
            {
                "a", "an", "and", "are", "as", "at", "be", "by", "for", "from",
                "has", "he", "in", "is", "it", "its", "of", "on", "that", "the",
                "to", "was", "will", "with", "the", "this", "but", "they", "have",
                "had", "what", "said", "each", "which", "she", "do", "how", "their",
                "if", "up", "out", "many", "then", "them", "these", "so", "some",
                "her", "would", "make", "like", "into", "him", "time", "two", "more",
                "go", "no", "way", "could", "my", "than", "first", "been", "call",
                "who", "oil", "sit", "now", "find", "down", "day", "did", "get",
                "come", "made", "may", "part"
            };
        }
    }
}
