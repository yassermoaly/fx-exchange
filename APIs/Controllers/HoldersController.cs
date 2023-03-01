using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.General;
using Models.DTOs.Holder;
using ServiceLayer.Interfaces;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoldersController : ControllerBase
    {
        private readonly IHolderService _holderService;
        public HoldersController(IHolderService holderService)
        {
            _holderService = holderService;
        }


        [HttpPost("Filter")]
        public async Task<FilterResponse<DTOHolder>> Filter(FilterRequest<string> Posted)
        {
            return await _holderService.Filter(Posted);
        }
    }
}
