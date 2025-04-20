using AutoMapper;
using Grpc.Core;
using Soundy.CatalogService.Dto.PlaylistDto;
using Soundy.CatalogService.Interfaces;

namespace Soundy.CatalogService.Controllers
{
    public class PlaylistGrpcController(IPlaylistService playlistService, IMapper mapper) : PlaylistGrpcService.PlaylistGrpcServiceBase
    {
        private IPlaylistService _playlistService = playlistService;
        private IMapper _mapper = mapper;

        public override async Task<CreatePlaylistResponse> CreatePlaylist(CreatePlaylistRequest request, ServerCallContext context)
        {
            try
            {
                var requestDto = _mapper.Map<CreatePlaylistRequestDto>(request);
                var playlist = await _playlistService.CreatePlaylistAsync(requestDto);
                return _mapper.Map<CreatePlaylistResponse>(playlist);
            }
            catch (AutoMapperMappingException ex)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Types.ToString() ?? string.Empty));
            }
            catch (Exception e)
            {
                throw new RpcException(Status.DefaultCancelled);
            }
        }
    }
}
