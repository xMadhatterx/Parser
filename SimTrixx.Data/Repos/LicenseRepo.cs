using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimTrixx.Common.Entities;
using Dapper;
using System.Data.SqlClient;

namespace SimTrixx.Data.Repos
{
    public class LicenseRepo
    {
        public License GetLicense(string Id)
        {
            string sql = "SELECT * FROM Licenses WHERE Id = '" + Id + "'";

            using (var connection = new SqlConnection("Data Source=SQL5105.site4now.net;Initial Catalog=db_a39883_simtrixx;User Id=db_a39883_simtrixx_admin;Password=Of*w6m8TYO5"))
            {
                var orderDetails = connection.Query<License>(sql).FirstOrDefault();
                return orderDetails;
            }
        }
    }
}
