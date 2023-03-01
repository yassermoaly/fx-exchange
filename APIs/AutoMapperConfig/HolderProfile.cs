using AutoMapper;
using Models.Data;
using Models.DTOs.General;
using Models.DTOs.Holder;

namespace APIs.MapperProfiles
{
    public class HolderProfile : Profile
    {
        public HolderProfile()
        {
            CreateMap<Holder, DTOHolder>();            
            CreateMap<FilterResponse<Holder>, FilterResponse<DTOHolder>>();
        }

    }
}
