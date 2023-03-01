using Microsoft.AspNetCore.Mvc;
using Models.DTOs.FxTransaction;
using Models.DTOs.General;
using Models.DTOs.Holder;
using ServiceLayer;
using ServiceLayer.Interfaces;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FxTransactionsController : ControllerBase
    {
        private readonly IFxTransactionService _fxTransactionService;
        public FxTransactionsController(IFxTransactionService fxTransactionService)
        {
            _fxTransactionService = fxTransactionService;
        }

        [HttpPost("CreateTrade")]
        public async Task<GResponse<DTOFxTransaction>> CreateTrade(DTOCreateFxTransactionRequest Posted)
        {
            return await _fxTransactionService.Create(Posted);
        }

        [HttpPost("Filter")]
        public async Task<FilterResponse<DTOFxTransaction>> Filter(FilterRequest<DTOFxTransactionFilter> Posted)
        {
            return await _fxTransactionService.Filter(Posted);
        }
    }
}
