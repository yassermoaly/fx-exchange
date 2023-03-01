using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Rate;
using ServiceLayer.Interfaces;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FxExchangeRateController : ControllerBase
    {
        private readonly IFxExchangeRateService _fxExchangeRateService;

        public FxExchangeRateController(IFxExchangeRateService fxExchangeRateService)
        {
            _fxExchangeRateService = fxExchangeRateService;
        }
        [HttpPost("GetOffer")]
        public async Task<DTOConversionRateResponse> GetOffer(DTOConversionRateRequest Posted)
        {
            return await _fxExchangeRateService.GetOffer(Posted);
        }
    }
}
