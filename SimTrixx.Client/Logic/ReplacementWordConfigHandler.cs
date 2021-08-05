using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SimTrixx.Reader.Concrete;

namespace TestDocReader.Logic
{
    public class ReplacementWordConfigHandler
    {
        public void Add(List<string> replacements)
        {
            var replacementDictionary = new ReplacementDictionary();
            replacementDictionary.ReplaceWords = new List<string>();
            replacementDictionary.ReplaceWords.AddRange(replacements);
            Export(replacementDictionary);
        }

        public ReplacementDictionary Import()
        {
            var jsonText = System.IO.File.ReadAllText("./Configs/Replacements.json");
            var replacements = JsonConvert.DeserializeObject<ReplacementDictionary>(jsonText);
            return replacements;

        }

        public void Export(ReplacementDictionary replacementDictionary)
        {
            string jsonString = JsonConvert.SerializeObject(replacementDictionary);

            if (!string.IsNullOrEmpty(jsonString))
            {
                System.IO.File.WriteAllText("./Configs/Replacements.json", jsonString);
            }
        }
    }
}
