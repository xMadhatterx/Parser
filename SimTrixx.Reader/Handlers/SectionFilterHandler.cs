using SimTrixx.Reader.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ContractReaderV2.Handlers
{
    public class SectionFilterHandler
    {
        public List<Contract> GetSectionsWithKeywords(List<Word> keywords, List<Contract> contractLines)
        {
            var keywordSection = new List<Contract>();
            foreach (var contract in contractLines)
            {
                foreach (var keyword in keywords)
                {
                    if (contract.Data.ToLower().Contains(keyword.Keyword.ToLower()))
                    {
                        keywordSection.Add(contract);
                        break;
                    }
                }
            }
            return keywordSection;
        }
        public List<Contract> SplitSectionsByKeyword(List<Contract> contracts, List<Word> keywords)
        {
            var splitSectionContract = new List<Contract>();
            foreach (var contract in contracts)
            {
                string[] sentences = Regex.Split(contract.Data, @"(?<=[\.!\?])\s+");
                var indexes = new List<int>();
                foreach (var sentence in sentences)
                {
                    foreach (var word in keywords)
                    {
                        if (sentence.ToLower().Contains(word.Keyword.ToLower()))
                        {
                            var index = contract.Data.IndexOf(sentence);
                            //Just to test the return of the index
                            var t = contract.Data.Substring(contract.Data.IndexOf(sentence), 1);
                            if (!indexes.Contains(index))
                            {
                                indexes.Add(index);
                                break;
                            }
                        }
                    }
                    if (indexes.Count > 0 && !indexes.Contains(0))
                    {
                        indexes.Add(0);
                    }
                }
                var orderedIndexes = indexes.OrderBy(x => x).ToList();
                if (orderedIndexes.Count >= 1)
                {
                    for (var i = 0; i < orderedIndexes.Count; i++)
                    {
                        var k = new Contract();
                        k.DocumentSection = contract.DocumentSection;
                        if (i != orderedIndexes.Count - 1)
                        {
                            //var m = orderedIndexes[i];
                            //var l = orderedIndexes[(i + 1)] - 1;
                            var j = (orderedIndexes[i + 1] - orderedIndexes[i]) - 1;
                            k.Data = contract.Data.Substring(orderedIndexes[i], j);
                            splitSectionContract.Add(k);
                        }
                        else
                        {
                            var j = ((contract.Data.Length - 1) - orderedIndexes[i]);
                            k.Data = contract.Data.Substring(orderedIndexes[i], j);
                            splitSectionContract.Add(k);
                        }

                    }
                }
                Contract newContract = new Contract();
                newContract.DocumentSection = contract.DocumentSection;
            }
            return splitSectionContract;
        }
    }
}
