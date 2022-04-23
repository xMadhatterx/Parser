using System.Linq;
using SimTrixx.Data.Contexts;

namespace SimTrixx.Data.Repos
{
    public class LicenseRepo
    {
        public License GetLicense(string licenseKey)
        {
            var dbContext = new simtrixxEntities();

            var userLicense = dbContext.Licenses.Where(x => x.Key == licenseKey).FirstOrDefault();
            return userLicense;
        }
    }
}
