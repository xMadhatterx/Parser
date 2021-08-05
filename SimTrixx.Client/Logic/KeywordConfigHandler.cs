using System.Collections.Generic;
using SimTrixx.Reader.Concrete;
using Newtonsoft.Json;
namespace TestDocReader.Logic
{
    public class KeywordConfigHandler
    {
        //public void Add(List<string> keywords)
        //{
        //    var keywordDictionary = new KeywordDictionary();
        //    keywordDictionary.Keywords = new List<string>();
        //    keywordDictionary.Keywords.AddRange(keywords);
        //    Export(keywordDictionary);
        //}



        //public KeywordDictionary Import()
        //{
        //    var jsonText= System.IO.File.ReadAllText("./Configs/Keywords.json");
        //    var keywords = JsonConvert.DeserializeObject<KeywordDictionary>(jsonText);
        //    return keywords;

        //}

        public Root ImportV2()
        {
            var keywords = new Root();
            if (System.IO.File.Exists("./Configs/KeywordsV2.json"))
            {
                var jsonText = System.IO.File.ReadAllText("./Configs/KeywordsV2.json");
                keywords = JsonConvert.DeserializeObject<Root>(jsonText);
            }
            else
            {
                throw new System.Exception("Can not find Keyword file");
            }
            return keywords;

        }

        //public void Export(KeywordDictionary keywordDictionary)
        //{
        //    string jsonString = JsonConvert.SerializeObject(keywordDictionary);

        //    if (!string.IsNullOrEmpty(jsonString))
        //    {
        //        System.IO.File.WriteAllText("./Configs/Keywords.json", jsonString);
        //    }
        //}
        public void ExportV2(Root keywordDictionary)
        {
            string jsonString = JsonConvert.SerializeObject(keywordDictionary);

            if (!string.IsNullOrEmpty(jsonString))
            {
                System.IO.File.WriteAllText("./Configs/KeywordsV2.json", jsonString);
            }
        }
    }
}
