using System.Data.Entity.Core.EntityClient;

namespace SimTrixx.Data.Repos
{
    public static class RepoService
    {
        public static string GetEntityConnectionString(string connectionString)
        {
            var entityBuilder = new EntityConnectionStringBuilder();
            
            entityBuilder.Provider = "System.Data.SqlClient";
            entityBuilder.ProviderConnectionString = connectionString + ";MultipleActiveResultSets=True;App=EntityFramework;";
            entityBuilder.Metadata = @"res://*/Contexts.SimtrixxModel.csdl|res://*/Contexts.SimtrixxModel.ssdl|res://*/Contexts.SimtrixxModel.msl";

            return entityBuilder.ToString();
        }
    }
}
