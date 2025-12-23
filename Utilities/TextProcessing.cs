using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HireZ.Utilities
{
    public static class TextProcessing
    {
        // Minimal stopwords — extend as needed
        private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "and","or","the","with","a","an","to","for","in","on","of","by","is","are","as","at","from","be","has","have","that","this","it"
        };

        /// <summary>
        /// Extract simple keyword tokens from text: remove punctuation, split on whitespace,
        /// remove stopwords and tokens shorter than 3 chars. Returns distinct tokens.
        /// </summary>
        public static HashSet<string> ExtractKeywords(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // remove punctuation and non-letter/digit characters
            var cleaned = Regex.Replace(text, @"[^\p{L}\p{N}\s\+\#\.\-]", " ");
            var tokens = cleaned
                .Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim().ToLowerInvariant())
                .Select(t => t.Trim('.', ',', ';', ':', '-', '+', '#'))
                .Where(t => t.Length >= 3 && !StopWords.Contains(t))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // extra: collapse common programming tokens (e.g., c#, .net -> dotnet)
            if (tokens.Contains("c#") || tokens.Contains("csharp")) { tokens.Remove("c#"); tokens.Remove("csharp"); tokens.Add("csharp"); }
            if (tokens.Contains(".net") || tokens.Contains("dotnet")) { tokens.Remove(".net"); tokens.Add("dotnet"); }

            return tokens;
        }
    }
}
