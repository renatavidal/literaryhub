// BE/BEActiveSubscription.cs
using System;

namespace BE
{
    public class BEActiveSubscription
    {
        public int OrderId { get; set; }
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public DateTime? PaidUtc { get; set; }
        public DateTime? ExpiresUtc { get; set; }
        public bool IsActive { get; set; }
    }
}
