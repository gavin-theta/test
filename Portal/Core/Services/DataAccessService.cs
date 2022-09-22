using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WelNetworks.BidWel.Core.Services;
using WelNetworks.BidWel.Portal.Core.Contracts;
using WelNetworks.BidWel.Portal.Core.Models;

namespace WelNetworks.BidWel.Portal.Core.Services
{
    public class DataAccessService : DataAccessBase, IDataAccess
    {

        public DataAccessService(IConfiguration configuration, ILogger<DataAccessService> logger) :
          base(configuration, logger)
        {
         
        }

        public async Task<IList<DispatchInstruction>> GetSearchAsync(DateTime from, DateTime to, string node, string product)
        {

            to = (to == DateTime.MinValue) ? DateTime.MaxValue : to;
            from = (from == DateTime.MinValue) ? new DateTime(1800, 1, 1) : from;
            var parameters = new DynamicParameters();
            parameters.Add("@From", from, DbType.DateTime);
            parameters.Add("@To", to, DbType.DateTime);
            parameters.Add("@Node", node, DbType.String);
            parameters.Add("@Product", product, DbType.String);

            var result = await GetValuesByStoredProcedureAsync<DispatchInstruction>("WTPWeb.DispatchInstructionSearch", parameters);

            return result;
        }

        public async Task<DispatchInstructionDetail> GetInstructionAsync(int Id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", Id);

            var result = await GetValueByStoredProcedureAsync<DispatchInstructionDetail>("WTPWeb.DispatchInstructionDetail", parameters);

            return result;
        }


        public async Task<(IList<DispatchInstruction>, int total)> GetInstructionsAsync(int page = 1, bool paged = true, int pageSize = 0)
        {
            // if paged is false we just want the first page*pageSize records - ignore paging
            pageSize = pageSize == 0 ? PageSize : pageSize;

            if (!paged)
            {
                pageSize *= page;
                page = 1;
            }

            var parameters = new DynamicParameters();
            parameters.Add("@Total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@PageNumber", page);
            parameters.Add("@PageSize", pageSize);

            var results = await GetValuesByStoredProcedureAsync<DispatchInstruction>("WTPWeb.DispatchInstructionList", parameters);
            var total = parameters.Get<int?>("Total") ?? 0;

            return (results, total);
        }
        public async Task<IList<string>> GetPrimaryDispatchTypesAsync()
        {
            try
            {
                using var connection = GetConnection();

                connection.Open();
                var result = await connection.QueryAsync<string>("SELECT [PrimaryDispatchTypeCode] FROM [WTPApp].[WTPPrimaryDispatchType]");

                return result.ToList();

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Query failure");
            }

            return new List<string>();
        }


        public async Task<IList<string>> GetBlockAndNodeTypesAsync()
        {
            try
            {
                using var connection = GetConnection();

                connection.Open();
                var result = await connection.QueryAsync<string>("SELECT DISTINCT [BlockCode] as code FROM [WTPApp].[vw_WTPDispatchInstructionWithGroup] UNION ALL SELECT DISTINCT [NodeCode] as code FROM [WTPApp].[vw_WTPDispatchInstructionWithGroup] WHERE NodeCode IS NOT NULL");

                return result.ToList();

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Query failure");
            }

            return new List<string>();
        }

        public async Task<Heartbeat> GetHeartbeatAsync(string groupCode, int warnThresholdSeconds, int errorThresholdSeconds)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@DispatchGroup", groupCode);

            var result = await GetValueByStoredProcedureAsync<Heartbeat>("WTPWeb.Heartbeat", parameters);
            var offset = Math.Abs(result.LastRequestUTC.Subtract(DateTime.UtcNow).TotalSeconds);

            result.Status = Enums.NotificationType.SUCCESS;
          
            if (offset > errorThresholdSeconds) { 
                result.Status = Enums.NotificationType.ERROR;
                return result;
            }
            if (offset > warnThresholdSeconds) { 
                result.Status = Enums.NotificationType.WARNING;
                return result;
            }

            return result;

        }

    }
}
