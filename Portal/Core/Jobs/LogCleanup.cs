using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WelNetworks.BidWel.Portal.Core.Contracts;

namespace WelNetworks.BidWel.Portal.Core.Jobs
{
    public class LogCleanUp : ICleanUpJob
    {
        public LogCleanUp(IDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task ExecuteAsync()
        {
            try
            {
                Debug.WriteLine($"TODO: Clean:{DateTime.Now.ToLongTimeString()}");
                await Task.CompletedTask;
            }
            catch (Exception)
            {

            }
        }

        private readonly IDataAccess dataAccess;
    }
}
