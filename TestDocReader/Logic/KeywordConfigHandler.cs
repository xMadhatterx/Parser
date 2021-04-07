using System.Collections.Generic;
using TestDocReader.Concrete;
using Newtonsoft.Json;
namespace TestDocReader.Logic
{
    public class KeywordConfigHandler
    {
        public void Add(List<string> keywords)
        {
            var keywordDictionary = new KeywordDictionary();
            keywordDictionary.Keywords = new List<string>();
            keywordDictionary.Keywords.AddRange(keywords);
            Export(keywordDictionary);
        }

      

        public KeywordDictionary Import()
        {
            var jsonText= System.IO.File.ReadAllText("./Configs/Keywords.json");
            var keywords = JsonConvert.DeserializeObject<KeywordDictionary>(jsonText);
            return keywords;
            
        }

        public void Export(KeywordDictionary keywordDictionary)
        {
            string jsonString =JsonConvert.SerializeObject(keywordDictionary);

            if (!string.IsNullOrEmpty(jsonString))
            {
                System.IO.File.WriteAllText("./Configs/Keywords.json", jsonString);
            }
        }
    }
}
