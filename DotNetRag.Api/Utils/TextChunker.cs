namespace DotNetRag.Api.Utils
{
    public static class TextChunker
    {
        public static List<string> SplitTextIntoWordChunks(string text, int wordChunkSize)
        {
            List<string> chunks = new List<string>();
            if (string.IsNullOrWhiteSpace(text))
            {
                return chunks;
            }

            // Split the text into individual words, considering common word separators
            string[] words = text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            int currentWordIndex = 0;
            while (currentWordIndex < words.Length)
            {
                List<string> currentChunkWords = new List<string>();
                int wordsInCurrentChunk = 0;

                // Add words to the current chunk until the word count limit is reached
                while (currentWordIndex < words.Length && wordsInCurrentChunk < wordChunkSize)
                {
                    currentChunkWords.Add(words[currentWordIndex]);
                    wordsInCurrentChunk++;
                    currentWordIndex++;
                }

                // Join the words in the current chunk back into a string
                chunks.Add(string.Join(" ", currentChunkWords));
            }

            return chunks;
        }

    }
}
