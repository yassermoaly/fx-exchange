using DataAccessLayer.Interfaces;
using Models.Data;
using Models.DTOs;
using Moq;
using Serilog;
using ServiceLayer;
using ServiceLayer.Interfaces;
using System.Collections.Generic;

namespace UnitTesting
{
    [TestClass]
    public class RateLimitUnitTest
    {
        Helper _helper;
        public RateLimitUnitTest()
        {
            _helper = new Helper();
        }
        [TestMethod]
        public async Task NotExceedLimits()
        {
            //Max Rate Per Hour is configured to 10 in Helper Class 
            IPresistanceService MockPresistanceService = Mock.Of<IPresistanceService>(l => l.Increment(It.IsAny<string>(),It.IsAny<DateTimeOffset>()) == Task.Run(()=> (long)5));
            var RateService = new RateLimitService(MockPresistanceService, _helper.Configuration);
            await RateService.NotExceedLimits(_helper.Holder.Id);
        }

        [TestMethod]
        public async Task ExceedLimits()
        {
            IPresistanceService MockPresistanceService = Mock.Of<IPresistanceService>(l => l.Increment(It.IsAny<string>(), It.IsAny<DateTimeOffset>()) == Task.Run(() => (long)11));
            var RateService = new RateLimitService(MockPresistanceService, _helper.Configuration);

            await Assert.ThrowsExceptionAsync<ApplicationException>(async() => await RateService.NotExceedLimits(_helper.Holder.Id));
        }

    }
}