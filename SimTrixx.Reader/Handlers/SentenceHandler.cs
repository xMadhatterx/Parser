using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SimTrixx.Reader.Concrete;

namespace SimTrixx.Reader.Handlers
{
    public class SentenceHandler
    {
        public static List<string> GetSentences(string input)
        {
            var sentences = Regex.Split(input ?? throw new InvalidOperationException(), @"(?<=[\.!\?])\s+");
            return sentences.ToList();
        }

        public static List<string> GetSentences(List<string> input)
        {
            var listOfSentences = new List<string>();
            foreach (var sentences in input.Select(sentence => Regex.Split(sentence ?? throw new InvalidOperationException(), @"(?<=[\.!\?])\s+")))
            {
                listOfSentences.AddRange(sentences);
            }
            return listOfSentences;
        }

        public static List<Contract> GetSentences(List<Contract> input)
        {
            return (from items in input let sentences = Regex.Split(items.Data ?? throw new InvalidOperationException(), @"(?<=[\.!\?])\s+") from sentence in sentences select new Contract { Data = sentence, DocumentSection = items.DocumentSection }).ToList();
        }
    }
}
