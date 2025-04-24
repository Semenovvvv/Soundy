using AutoMapper;
using Grpc.Core;
using Soundy.CatalogService.Dto.TrackDto;
using Soundy.CatalogService.Interfaces;
using Soundy.SharedLibrary.Contracts.Track;

namespace Soundy.CatalogService.Controllers
{
    public class TrackGrpcController : TrackGrpcService.TrackGrpcServiceBase
    {
        private readonly ITrackService _trackService;
        private readonly IMapper _mapper;

        public TrackGrpcController(
            ITrackService trackService,
            IMapper mapper)
        {
            _trackService = trackService;
            _mapper = mapper;
        }

        public override async Task<CreateResponse> Create(CreateRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<CreateRequestDto>(request);
            var responseDto = await _trackService.CreateAsync(requestDto, context.CancellationToken);
            return _mapper.Map<CreateResponse>(responseDto);
        }

        public override async Task<GetByIdResponse> GetById(GetByIdRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<GetByIdRequestDto>(request);
            var responseDto = await _trackService.GetByIdAsync(requestDto, context.CancellationToken);
            return _mapper.Map<GetByIdResponse>(responseDto);
        }

        public override async Task<GetListByPlaylistResponse> GetListByPlaylist(GetListByPlaylistRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<GetListByPlaylistRequestDto>(request);
            var responseDto = await _trackService.GetListByPlaylistAsync(requestDto, context.CancellationToken);
            return _mapper.Map<GetListByPlaylistResponse>(responseDto);
        }

        public override async Task<SearchResponse> Search(SearchRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<SearchRequestDto>(request);
            var responseDto = await _trackService.SearchAsync(requestDto, context.CancellationToken);
            return _mapper.Map<SearchResponse>(responseDto);
        }

        public override async Task<UpdateResponse> Update(UpdateRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<UpdateRequestDto>(request);
            var responseDto = await _trackService.UpdateAsync(requestDto, context.CancellationToken);
            return _mapper.Map<UpdateResponse>(responseDto);
        }

        public override async Task<DeleteResponse> Delete(DeleteRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<DeleteRequestDto>(request);
            var responseDto = await _trackService.DeleteAsync(requestDto, context.CancellationToken);
            return _mapper.Map<DeleteResponse>(responseDto);
        }
    }
}
