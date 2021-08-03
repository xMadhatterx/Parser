using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimTrixx.Data.Interfaces;
using SimTrixx.Common.Entities;
using SimTrixx.Data.Context;

namespace SimTrixx.Data.Repos
{
    public class LicenseRepo : ILicenseRepo
    {
        public License GetLicense(string Id)
        {
            LicenseContext db = new LicenseContext();
            return db.Licenses.Find(Id);
        }
    }
}
