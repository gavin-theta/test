using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WelNetworks.BidWel.Portal.Core.Services;

namespace WelNetworks.BidWel.Core.Services
{
    public class DataAccessBase
    {
        //TODO: store connection in property for reuse

        public DataAccessBase(IConfiguration configuration, ILogger<DataAccessService> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
            PageSize = 25;//TODO:from config

        }

        public int PageSize { get; set; }

        public async Task<T> GetValueByStoredProcedureAsync<T>(string storedProcedureName, DynamicParameters parameters)
        {
            var results = await GetValuesByStoredProcedureAsync<T>(storedProcedureName, parameters);
            return results.FirstOrDefault();

        }
        public async Task<List<T>> GetValuesByStoredProcedureAsync<T>(string storedProcedureName, DynamicParameters parameters)
        {
            try
            {
                using var connection = GetConnection();

                connection.Open();
                var result = await connection.QueryAsync<T>(
                    storedProcedureName,
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure
                    ).ConfigureAwait(true);

                return result.ToList();

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetValuesByStoredProcedure failure");
            }

            return new List<T>(0);
        }

        protected SqlConnection GetConnection()
        {
            var connectionString = configuration.GetConnectionString("PORTAL");
            return new SqlConnection(connectionString);
        }

        internal readonly IConfiguration configuration;
        internal readonly ILogger<DataAccessService> logger;


    }
}
