using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimTrixx.Common.Entities;

namespace SimTrixx.Data.Context
{
    public class LicenseContext : DbContext
    {
        public LicenseContext() : base("name=MainConnection")
        {

        }

        public DbSet<License> Licenses { get; set; }
        public DbSet<Customer> Customers {  get; set; }
    }
}
