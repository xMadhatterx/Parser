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
            ReplacementDictionary replacements;
            var keywordPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Simtrixx", "Replacements.json");
            //var jsonText = System.IO.File.ReadAllText("./Configs/Replacements.json");
            if (File.Exists(keywordPath))
            {
                var jsonText = File.ReadAllText(keywordPath);
                replacements = JsonConvert.DeserializeObject<ReplacementDictionary>(jsonText);
            }
            else
            {
                var jsonDir = Path.GetDirectoryName(keywordPath);
                if (!Directory.Exists(jsonDir))
                {
                    Directory.CreateDirectory(jsonDir);
                }

                using (var stream = File.Create(keywordPath))
                {
                }

                //Build Keywords
                BuildReplacements(keywordPath);

                var jsonText = File.ReadAllText(keywordPath);
                replacements = JsonConvert.DeserializeObject<ReplacementDictionary>(jsonText);
            }

            return replacements;

        }

        public void BuildReplacements(string path)
        {
            var r = new ReplacementDictionary
            {
                ReplaceWords = new List<string>()
            };
            r.ReplaceWords.Add("contractor");
            r.ReplaceWords.Add("contractors");
            r.ReplaceWords.Add("Contractor");
            r.ReplaceWords.Add("Contractors");
            var jsonString = JsonConvert.SerializeObject(r);
            File.WriteAllText(path, jsonString);
        }

        public void Export(ReplacementDictionary replacementDictionary)
        {
            var jsonString = JsonConvert.SerializeObject(replacementDictionary);
            var keywordPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Simtrixx", "Replacements.json");
            if (!string.IsNullOrEmpty(jsonString))
            {
                //System.IO.File.WriteAllText("./Configs/Replacements.json", jsonString);
                File.WriteAllText(keywordPath, jsonString);
            }
        }
    }
}
