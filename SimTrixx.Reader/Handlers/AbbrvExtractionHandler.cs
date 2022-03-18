using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using SimTrixx.Reader.Concrete;
namespace SimTrixx.Reader.Handlers
{
    public class AbbrvExtractionHandler
    {
        public  List<Tuple<string, string, string>> GetAbbreviations(string sentence)
        {
            var tupleList = new List<Tuple<string, string, string>>();
            var t = Regex.Matches(sentence, @"\(([^)]*)\)");
            //var t = Regex.Matches(sentence, @"\(([a - zA - Z)]*)\)");
            for (var i = 0; i < t.Count; i++)
            {
                if (!CheckForInt(t[i].Groups[1].Value))
                {
                    if (t[i].Groups[1].Value.Count() > 1 && t[i].Groups[1].Value.Count() < 10)
                    {
                        tupleList.Add(Tuple.Create(t[i].Groups[0].Value, t[i].Groups[1].Value, sentence));
                    }
                }
            }
            return tupleList;
        }

        public bool CheckForInt(string theString)
        {
            bool result = false;
            foreach (var i in theString)
            {

                if (int.TryParse(i.ToString(), out int value))
                {
                    result = true;
                }
            }
            return result;
        }
        public  List<AbbrvContainer> CreateAbbrvEntity(List<Tuple<string, string, string>> ret)
        {
            var abbrvList = new List<AbbrvContainer>();
            var lastIndex = 0;
            foreach (var tup in ret)
            {
                string[] defStringArray;
                string definition = string.Empty;
                var startIndex = tup.Item3.IndexOf(tup.Item1);
                Console.WriteLine(lastIndex + " | " + startIndex);
                var sub = tup.Item3.Substring(lastIndex, startIndex - lastIndex).ToString();

                Console.WriteLine("Full Sentence => " + tup.Item3);
                Console.WriteLine("Substring =>" + sub);
                var split = sub.Split(' ');
                if (split.Count() >= tup.Item2.Length)
                {
                    var wordsToGrab = tup.Item2.Length;
                    defStringArray = split.Reverse().Take(tup.Item2.Length + 1).Reverse().ToArray();
                }
                else
                {
                    defStringArray = split.Reverse().Take(split.Count()).Reverse().ToArray();
                }
                foreach (var k in defStringArray)
                {
                    definition += k + " ";
                }
                Console.WriteLine("Abbrv=> " + tup.Item2);
                Console.WriteLine("Definition=> " + definition.Trim() + "\n");
                var abbrvItem = new AbbrvContainer();
                abbrvItem.Abbrv = tup.Item2;
                abbrvItem.Count = 1;
                abbrvItem.Definition = definition.Trim();
                abbrvItem.Location = new List<string>() { tup.Item3 };
                abbrvList.Add(abbrvItem);
                lastIndex = startIndex + tup.Item1.Length;

            }
            return abbrvList;
        }
    }
}
