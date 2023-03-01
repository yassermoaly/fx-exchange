using DataAccessLayer.Interfaces;
using Models.Data;
using Moq;
using ServiceLayer;
using System.Collections.Generic;

namespace UnitTesting
{
    [TestClass]
    public class AccountUnitTest
    {
        Helper _helper;
        public AccountUnitTest()
        {
            _helper = new Helper();
        }
        [TestMethod]
        public async Task GetHolderBalances()
        {            
            IAccountRepository MockAccountRepository = Mock.Of<IAccountRepository>(l =>l.GetHolderBalances(_helper.Holder.Id) == Task.Run(() => _helper.AccountList));
            var AccountService = new AccountService(MockAccountRepository, _helper.Mapper);
            var result = await AccountService.GetHolderBalances(_helper.Holder.Id);
            Assert.AreEqual(result.Count, _helper.AccountList.Count);
        }
        [TestMethod]
        public async Task GetByHolderAndCurrencyExists()
        {

            IAccountRepository MockAccountRepository = Mock.Of<IAccountRepository>(l => l.GetByHolderAndCurrency(_helper.Holder.Id,_helper.CurrencyUSD.Id) == Task.Run(() => _helper.AccountUSD));
            var AccountService = new AccountService(MockAccountRepository, _helper.Mapper);
            var result = await AccountService.GetByHolderAndCurrency(_helper.Holder.Id, _helper.CurrencyUSD);
            Assert.AreEqual(result.HolderId, _helper.Holder.Id);
            Assert.AreEqual(result.CurrencyId, _helper.CurrencyUSD.Id);
        }



        [TestMethod]
        public async Task GetByHolderAndCurrencyNotExists()
        {
            IAccountRepository MockAccountRepository = Mock.Of<IAccountRepository>(l => l.GetByHolderAndCurrency(_helper.Holder.Id, _helper.CurrencyUSD.Id) == Task.Run(() => default(Account)));
            var AccountService = new AccountService(MockAccountRepository, _helper.Mapper);
            await Assert.ThrowsExceptionAsync<ApplicationException>(async () => await AccountService.GetByHolderAndCurrency(_helper.Holder.Id, _helper.CurrencyUSD));
        }

        [TestMethod]
        public void BuyAmount()
        {
            var Account = new Account()
            {
                Balance = 100
            };
            _helper.AccountService.BuyAmount(Account, 100);
            Assert.AreEqual(Account.Balance, 200);
        }

        [TestMethod]
        public void SellAmount()
        {
            var Account = new Account()
            {
                Balance = 100
            };
            _helper.AccountService.SellAmount(Account, 50);
            Assert.AreEqual(Account.Balance, 50);
        }
        [TestMethod]
        public void SellAmountWithInsufficientBalance()
        {
            var Account = new Account()
            {
                Id = 1,
                Balance = 100
            };

            var AppException = Assert.ThrowsException<ApplicationException>(() => _helper.AccountService.SellAmount(Account, 200));
            Assert.AreEqual(AppException.Message, $"Insufficient Balance in account {Account.Id}");
        }

    }
}