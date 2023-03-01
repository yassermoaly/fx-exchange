using Microsoft.Extensions.Configuration;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public class RateLimitService : IRateLimitService
    {
        private readonly IPresistanceService _presistanceService;
        private readonly IConfiguration _configurationService;
        public RateLimitService(IPresistanceService presistanceService, IConfiguration configurationService)
        {
            _presistanceService = presistanceService;
            _configurationService = configurationService;
        }
        public async Task NotExceedLimits(long HolderId)
        {
            DateTime SysDate = DateTime.Now;
            string Key = $"holder-{HolderId}-hour-limit-{DateTime.Now.ToString("yyyyMMddHH")}";

            long MaxRateLimitPerHour = long.Parse(_configurationService["FxTransactionRateLimitPerHour"] ?? "10");
            var Counter = await _presistanceService.Increment(Key, new DateTime(SysDate.Year, SysDate.Month, SysDate.Day, SysDate.Hour, 0, 0).AddHours(1));
            if (Counter > MaxRateLimitPerHour)
                throw new ApplicationException($"Exceeds limits per hour ({MaxRateLimitPerHour}), HolderId=>{HolderId}");
        }
    }
}
