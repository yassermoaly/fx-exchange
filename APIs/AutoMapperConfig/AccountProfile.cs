using AutoMapper;
using Models.Data;
using Models.DTOs.Account;

namespace APIs.MapperProfiles
{   
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<Account, DTOAccount>();
        }

    }
}
