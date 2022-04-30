using SimTrixx.Data.Repos;
using System;
using System.IO;
using Newtonsoft.Json;
using SimTrixx.Reader.Concrete;

namespace SimTrixx.Client.Logic
{
    public class LicenseHandler
    {
        public LocalLicense GetLicense()
        {
            var licensePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Simtrixx", "License.json");
            LocalLicense license;
            if (File.Exists(licensePath))
            {
                var jsonText = File.ReadAllText(licensePath);
                license = JsonConvert.DeserializeObject<LocalLicense>(jsonText);
            }
            else
            {
                var jsonDir = Path.GetDirectoryName(licensePath);
                if (!Directory.Exists(jsonDir))
                {
                    Directory.CreateDirectory(jsonDir);
                }

                using (var stream = File.Create(licensePath))
                {
                }

                var jsonText = File.ReadAllText(licensePath);
                license = JsonConvert.DeserializeObject<LocalLicense>(jsonText);
            }

            return license ?? new LocalLicense();
        }

        public bool CheckLicense(string licenseKey)
        {
            var license = new LicenseRepo().GetLicense(licenseKey);
            return license != null && license.Expiration > DateTime.Now;
        }

        public void UpdateLicense(LocalLicense license)
        {
            var licensePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Simtrixx", "License.json");
            var jsonString = JsonConvert.SerializeObject(license);
            if (!string.IsNullOrEmpty(jsonString))
            {
                File.WriteAllText(licensePath, jsonString);
            }
        }
    }
}
