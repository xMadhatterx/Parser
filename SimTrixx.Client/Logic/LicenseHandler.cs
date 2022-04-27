using SimTrixx.Data.Repos;
using System;

namespace SimTrixx.Client.Logic
{
    public class LicenseHandler
    {
        public bool CheckLicense(string licenseKey)
        {
            var license = new LicenseRepo().GetLicense(licenseKey);
            return license != null && license.Expiration > DateTime.Now;
        }
    }
}
