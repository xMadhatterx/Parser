using SimTrixx.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimTrixx.Data.Interfaces
{
    public interface ILicenseRepo
    {
        License GetLicense(string Id);
    }
}
