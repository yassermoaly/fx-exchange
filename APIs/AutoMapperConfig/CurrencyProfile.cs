using AutoMapper;
using Models.Data;
using Models.DTOs.Currency;

namespace APIs.MapperProfiles
{   
    public class CurrencyProfile : Profile
    {
        public CurrencyProfile()
        {
            CreateMap<Currency, DTOCurrency>();
        }

    }
}
