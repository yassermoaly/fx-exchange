using DataAccessLayer.Interfaces;
using Models.Data;
using Models.DTOs.General;
using Moq;
using ServiceLayer;
using System.Collections.Generic;

namespace UnitTesting
{
    [TestClass]
    public class HolderUnitTest
    {
        Helper _helper;
        public HolderUnitTest()
        {
            _helper = new Helper();
        }
        [TestMethod]
        public async Task Filter()
        {
            var FilterRequest = new FilterRequest<string>()
            {
                PageLength = 10,
                Start = 1,
                SearchData = string.Empty
            };       

            IHolderRepository MockHolderRepository = Mock.Of<IHolderRepository>(l => l.Filter(FilterRequest) == Task.Run(()=> new FilterResponse<Holder>()
            {
                TotalCount = 1,
                Data = new List<Holder>(new Holder[] { _helper.Holder })
            }));

            var HolderService = new HolderService(MockHolderRepository, _helper.Mapper);
            var result = await HolderService.Filter(FilterRequest);
            Assert.AreEqual(result.Data.Count, 1);
        }
        
    }
}