using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WelNetworks.BidWel.Portal.Core.Models;

namespace WelNetworks.BidWel.Portal.Core.Contracts
{
    public interface IDataAccess
    {
        Task<(IList<DispatchInstruction>, int total)> GetInstructionsAsync(int page = 1, bool paged = true, int pageSize = 0);

        Task<DispatchInstructionDetail> GetInstructionAsync(int Id);

        Task<IList<DispatchInstruction>> GetSearchAsync(DateTime from, DateTime to, string node, string product);

        Task<Heartbeat> GetHeartbeatAsync(string groupCode, int warnThresholdSeconds, int errorThresholdSeconds);

        Task<IList<string>> GetPrimaryDispatchTypesAsync();

        Task<IList<string>> GetBlockAndNodeTypesAsync();
    }
}
