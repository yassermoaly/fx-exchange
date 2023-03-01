using DataAccessLayer.Interfaces;
using Models.Data;
using Models.DTOs.FxTransaction;
using Moq;
using ServiceLayer;

namespace UnitTesting
{
    [TestClass]
    public class FxTransactionDetailTypeUnitTest
    {
        Helper _helper;
        public FxTransactionDetailTypeUnitTest()
        {
            _helper=new Helper();
        }
        [TestMethod]
        public async Task Get()
        {
            IFxTransactionDetailTypeRepository MockFxTransactionDetailTypeRepository = Mock.Of<IFxTransactionDetailTypeRepository>(l => l.GetAllAsync() == Task.Run(()=>_helper.FxTransactionDetailTypeList));
            var FxTransactionDetailTypeService = new FxTransactionDetailTypeService();
            await FxTransactionDetailTypeService.Load(MockFxTransactionDetailTypeRepository);
           
            var FxTransactionDetailTypeSell = _helper.FxTransactionDetailTypeService.Get(_helper.FxTransactionDetailTypeSell.Id);
            Assert.AreEqual(FxTransactionDetailTypeSell.Id, _helper.FxTransactionDetailTypeSell.Id);
        }

        [TestMethod]
        public async Task GetNotExists()
        {
            IFxTransactionDetailTypeRepository MockFxTransactionDetailTypeRepository = Mock.Of<IFxTransactionDetailTypeRepository>(l => l.GetAllAsync() == Task.Run(() => new List<FxTransactionDetailType>()));
            var FxTransactionDetailTypeService = new FxTransactionDetailTypeService();
            await FxTransactionDetailTypeService.Load(MockFxTransactionDetailTypeRepository);

            var AppException = Assert.ThrowsException<ApplicationException>(() => FxTransactionDetailTypeService.Get(_helper.FxTransactionDetailTypeSell.Id));
            Assert.AreEqual(AppException.Message, "FxTransactionDetailType is not found");

        }
    }
}