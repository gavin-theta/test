using System;

namespace WelNetworks.BidWel.Portal.Core.Models
{
    public class Heartbeat
    {
        public DateTime LastRequestUTC { get; set; }
        public string DispatchGroup { get; set; }
        public Enums.NotificationType Status { get; set; }

    }

}
