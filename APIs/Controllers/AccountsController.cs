using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Models.DTOs.Account;
using Models.DTOs.Holder;
using ServiceLayer.Interfaces;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("GetBalances/{HolderId}")]        
        public async Task<List<DTOAccount>> GetBalances(long HolderId)
        {
            return await _accountService.GetHolderBalances(HolderId);
        }
    }
}
