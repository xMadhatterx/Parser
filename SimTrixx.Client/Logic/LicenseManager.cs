using SimTrixx.Data.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTrixx.Client.Logic
{
    public class LicenseManager
    {
        public bool CheckLicense(string licenseKey)
        {
            var license = new LicenseRepo().GetLicense(licenseKey);
            if(license != null && license.Expiration > DateTime.Now)
            {
                return true;
            } 
            else
            {
                return false;
            }
        }
    }
}
