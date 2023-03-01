using DataAccessLayer.Interfaces;
using Models.Data;
using Moq;
using ServiceLayer;

namespace UnitTesting
{
    [TestClass]
    public class CurrencyUnitTest
    {
        Helper _helper;
        public CurrencyUnitTest()
        {
            _helper=new Helper();
        }
        [TestMethod]
        public async Task GetByISO()
        {
            ICurrencyRepository MockCurrencyRepository = Mock.Of<ICurrencyRepository>(l => l.GetAllAsync() == Task.Run(() => _helper.CurrencyList));
            var CurrencyService = new CurrencyService();
            await CurrencyService.Load(MockCurrencyRepository);

            var Currency = CurrencyService.GetByISO(_helper.CurrencyUSD.ISOCode);
            Assert.AreEqual(_helper.CurrencyUSD.ISOCode, Currency?.ISOCode);
        }

        [TestMethod]
        public async Task GetByISONotExists()
        {
            ICurrencyRepository MockCurrencyRepository = Mock.Of<ICurrencyRepository>(l => l.GetAllAsync() == Task.Run(() => new List<Currency>()));
            var CurrencyService = new CurrencyService();
            await CurrencyService.Load(MockCurrencyRepository);

            Assert.ThrowsException<ApplicationException>(() => CurrencyService.GetByISO(_helper.CurrencyUSD.ISOCode));
        }
    }
}