using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SimTrixx.Reader.Concrete;
using System.IO;

namespace SimTrixx.Client.Logic
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
            var keywordPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Simtrixx", "Replacements.json");
            //var jsonText = System.IO.File.ReadAllText("./Configs/Replacements.json");
            var jsonText = System.IO.File.ReadAllText(keywordPath);
            var replacements = JsonConvert.DeserializeObject<ReplacementDictionary>(jsonText);
            return replacements;

        }

        public void Export(ReplacementDictionary replacementDictionary)
        {
            string jsonString = JsonConvert.SerializeObject(replacementDictionary);
            var keywordPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Simtrixx", "Replacements.json");
            if (!string.IsNullOrEmpty(jsonString))
            {
                //System.IO.File.WriteAllText("./Configs/Replacements.json", jsonString);
                System.IO.File.WriteAllText(keywordPath, jsonString);
            }
        }
    }
}
