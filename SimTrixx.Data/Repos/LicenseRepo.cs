using System.Linq;
using SimTrixx.Data.Contexts;

namespace SimTrixx.Data.Repos
{
    public class LicenseRepo
    {
        public License GetLicense(string licenseKey)
        {
            var dbContext = new simtrixxEntities(RepoService.GetEntityConnectionString(Properties.Resources.ConnectionString));

            var userLicense = dbContext.Licenses.FirstOrDefault(x => x.Key == licenseKey);
            return userLicense;
        }
    }
}
