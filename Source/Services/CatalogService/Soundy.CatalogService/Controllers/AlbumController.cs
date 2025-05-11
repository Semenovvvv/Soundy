using AutoMapper;
using Grpc.Core;
using Service.Album;
using Soundy.CatalogService.Dto.AlbumDtos;
using Soundy.CatalogService.Interfaces;

namespace Soundy.CatalogService.Controllers
{
    public class AlbumGrpcController(IAlbumService albumService, IMapper mapper) : AlbumGrpcService.AlbumGrpcServiceBase
    {
        private IAlbumService _albumService = albumService;
        private IMapper _mapper = mapper;

        public override async Task<CreateResponse> Create(CreateRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<CreateRequestDto>(request);
            var responseDto = await _albumService.CreateAsync(requestDto, context.CancellationToken);
            return _mapper.Map<CreateResponse>(responseDto);
        }

        public override async Task<AddTrackResponse> AddTrack(AddTrackRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<AddTrackRequestDto>(request);
            var responseDto = await _albumService.AddTrackAsync(requestDto, context.CancellationToken);
            return _mapper.Map<AddTrackResponse>(responseDto);
        }
    }
}
