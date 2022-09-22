using System.Threading.Tasks;

namespace WelNetworks.BidWel.Portal.Core.Contracts
{
    public interface IHeartbeatJob
    {
        Task ExecuteAsync(bool trigger);
    }
}