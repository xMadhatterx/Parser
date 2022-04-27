using SimTrixx.Reader.Concrete;
using Newtonsoft.Json;
using System.IO;
using System;

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
            if (System.IO.File.Exists(keywordPath))
            {
                var jsonText = System.IO.File.ReadAllText(keywordPath);
                keywords = JsonConvert.DeserializeObject<Root>(jsonText);
            }
            else
            {
                throw new System.Exception("Can not find Keyword file");
            }
            return keywords;

        }
        public void ExportV2(Root keywordDictionary)
        {
            var keywordPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Simtrixx", "KeywordsV2.json");
            string jsonString = JsonConvert.SerializeObject(keywordDictionary);

            //if (!string.IsNullOrEmpty(jsonString))
            //{
            //    System.IO.File.WriteAllText("./Configs/KeywordsV2.json", jsonString);
            //}
            if (!string.IsNullOrEmpty(jsonString))
            {
                System.IO.File.WriteAllText(keywordPath, jsonString);
            }
        }
    }
}
