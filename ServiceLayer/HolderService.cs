using AutoMapper;
using DataAccessLayer.Interfaces;
using Models.Data;
using Models.DTOs.General;
using Models.DTOs.Holder;
using ServiceLayer.Interfaces;

namespace ServiceLayer
{
    public class HolderService : IHolderService
    {
        private readonly IHolderRepository _holderRepository;
        private readonly IMapper _mapper;
        public HolderService(IHolderRepository holderRepository, IMapper mapper)
        {
            _holderRepository = holderRepository;
            _mapper = mapper;
        }
        public async Task<FilterResponse<DTOHolder>> Filter(FilterRequest<string> Filter)
        {
            return _mapper.Map<FilterResponse<DTOHolder>>(await _holderRepository.Filter(Filter));
        }
        public async Task<Holder> Get(long Id)
        {
            return await _holderRepository.FindAsync(Id)??throw new ApplicationException($"Holder Not Exists, Holder Id {Id}");
        }
    }
}
