using AutoMapper;
using Models.Data;
using Models.DTOs.FxTransaction;
using Models.DTOs.General;
using Models.DTOs.Holder;

namespace APIs.MapperProfiles
{
    public class FxTransactionProfile : Profile
    {
        public FxTransactionProfile()
        {
            CreateMap<FxTransaction, DTOFxTransaction>().ForMember(
                dest => dest.FixedSide,
                opt => opt.MapFrom(src => (FxTransactionFixedSideEnum)src.FxTransactionFixedSideId)
            );

            CreateMap<FxTransactionDetail, DTOFxTransactionDetail>().ForMember(
                dest => dest.Currency,
                opt => opt.MapFrom(src => src.Account == null?null:src.Account.Currency)               
            );
            CreateMap<FxTransactionDetailType, DTOFxTransactionDetailType>();
            CreateMap<FilterResponse<FxTransaction>, FilterResponse<DTOFxTransaction>>();
        }
    }
}
