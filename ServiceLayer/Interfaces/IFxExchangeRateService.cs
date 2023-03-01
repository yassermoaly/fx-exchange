using Models.DTOs.Rate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface IFxExchangeRateService
    {
        Task<DTOConversionRateResponse> GetOffer(DTOConversionRateRequest Posted);
    }
}
