using Xunit;

namespace TextSummarizer.Tests
{
    using TextSummarizer = global::TextSummarizer.TextSummarizer;

    public class TextSummarizerTests
    {
        [Fact]
        public void SummarizeText_EmptyInput_ReturnsEmptyString()
        {
            var summarizer = new TextSummarizer();

            var result = summarizer.SummarizeText("");

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void SummarizeText_WhitespaceInput_ReturnsEmptyString()
        {
            var summarizer = new TextSummarizer();

            var result = summarizer.SummarizeText("   \n\t  ");

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void SummarizeText_FewerSentencesThanLimit_ReturnsOriginalText()
        {
            var summarizer = new TextSummarizer(maxSentences: 3);
            var text = "This is one short sentence. Here is another one.";

            var result = summarizer.SummarizeText(text);

            Assert.Equal(text, result);
        }

        [Fact]
        public void SummarizeText_MoreSentencesThanLimit_ReturnsAtMostMaxSentences()
        {
            var summarizer = new TextSummarizer(maxSentences: 2);
            var text = "The important result was significant. " +
                       "A cat sat on a mat. " +
                       "Many other unrelated words fill this sentence. " +
                       "The key conclusion is critical for everyone. " +
                       "Another filler sentence goes here for padding.";

            var result = summarizer.SummarizeText(text);
            var sentenceCount = result.Split('.', System.StringSplitOptions.RemoveEmptyEntries).Length;

            Assert.True(sentenceCount <= 2);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void SummarizeText_PreservesOriginalSentenceOrder()
        {
            var summarizer = new TextSummarizer(maxSentences: 2);
            var text = "Important key first sentence with critical content. " +
                       "A short filler in the middle without any notable weight. " +
                       "The significant main conclusion appears at the very end.";

            var result = summarizer.SummarizeText(text);
            var firstSentenceIndex = result.IndexOf("first sentence", System.StringComparison.Ordinal);
            var lastSentenceIndex = result.IndexOf("very end", System.StringComparison.Ordinal);

            Assert.True(firstSentenceIndex >= 0 && lastSentenceIndex >= 0);
            Assert.True(firstSentenceIndex < lastSentenceIndex);
        }
    }
}
