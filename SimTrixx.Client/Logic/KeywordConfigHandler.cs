using SimTrixx.Reader.Concrete;
using Newtonsoft.Json;
using System.IO;
using System;
using System.ComponentModel;

namespace SimTrixx.Client.Logic
{
    public class KeywordConfigHandler
    {
        public Root ImportV2()
        {
            var keywordPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Simtrixx", "KeywordsV2.json");
            var keywords = new Root();
            //if (System.IO.File.Exists("./Configs/KeywordsV2.json"))
            //{
            //    var jsonText = System.IO.File.ReadAllText("./Configs/KeywordsV2.json");
            //    keywords = JsonConvert.DeserializeObject<Root>(jsonText);
            //}
            //else
            //{
            //    throw new System.Exception("Can not find Keyword file");
            //}
            if (File.Exists(keywordPath))
            {
                var jsonText = File.ReadAllText(keywordPath);
                keywords = JsonConvert.DeserializeObject<Root>(jsonText);
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

                BuildKeywords(keywordPath);

                var jsonText = File.ReadAllText(keywordPath);
                keywords = JsonConvert.DeserializeObject<Root>(jsonText);
            }
            return keywords;

        }

        public void BuildKeywords(string path)
        {
            var r = new Root
            {
                Keywords = new BindingList<Word>
                {
                    new Word() { Keyword = "Contractor Shall", Replacement = "" },
                    new Word() { Keyword = "Contractors Shall", Replacement = "" },
                    new Word() { Keyword = "Contractor Will", Replacement = "" },
                    new Word() { Keyword = "Contractors Will", Replacement = "" }
                }
            };
            new KeywordConfigHandler().ExportV2(r);
        }

        public void ExportV2(Root keywordDictionary)
        {
            var keywordPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Simtrixx", "KeywordsV2.json");
            var jsonString = JsonConvert.SerializeObject(keywordDictionary);

            //if (!string.IsNullOrEmpty(jsonString))
            //{
            //    System.IO.File.WriteAllText("./Configs/KeywordsV2.json", jsonString);
            //}
            if (!string.IsNullOrEmpty(jsonString))
            {
                File.WriteAllText(keywordPath, jsonString);
            }
        }
    }
}
