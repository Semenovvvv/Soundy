using AutoMapper;
using Grpc.Core;
using Service.Playlist;
using Soundy.CatalogService.Dto.PlaylistDtos;
using Soundy.CatalogService.Interfaces;

namespace Soundy.CatalogService.Controllers
{
    public class PlaylistGrpcController(IPlaylistService playlistService, IMapper mapper) : PlaylistGrpcService.PlaylistGrpcServiceBase
    {
        private IPlaylistService _playlistService = playlistService;
        private IMapper _mapper = mapper;

        public override async Task<CreateResponse> Create(CreateRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<CreateRequestDto>(request);
            var responseDto = await _playlistService.CreateAsync(requestDto, context.CancellationToken);
            return _mapper.Map<CreateResponse>(responseDto);
        }

        public override async Task<CreateFavoriteResponse> CreateFavorite(CreateFavoriteRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<CreateFavoriteRequestDto>(request);
            var responseDto = await _playlistService.CreateFavoriteAsync(requestDto, context.CancellationToken);
            return _mapper.Map<CreateFavoriteResponse>(responseDto);
        }

        public override async Task<GetByIdResponse> GetById(GetByIdRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<GetByIdRequestDto>(request);
            var responseDto = await _playlistService.GetByIdAsync(requestDto, context.CancellationToken);
            return _mapper.Map<GetByIdResponse>(responseDto);
        }

        public override async Task<GetListByAuthorIdResponse> GetListByAuthorId(GetListByAuthorIdRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<GetListByAuthorIdRequestDto>(request);
            var responseDto = await _playlistService.GetListByAuthorIdAsync(requestDto, context.CancellationToken);
            return _mapper.Map<GetListByAuthorIdResponse>(responseDto);
        }

        public override async Task<GetFavoriteResponse> GetFavorite(GetFavoriteRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<GetFavoriteRequestDto>(request);
            var responseDto = await _playlistService.GetFavoriteAsync(requestDto, context.CancellationToken);
            return _mapper.Map<GetFavoriteResponse>(responseDto);
        }

        public override async Task<AddTrackResponse> AddTrack(AddTrackRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<AddTrackRequestDto>(request);
            var responseDto = await _playlistService.AddTrackAsync(requestDto, context.CancellationToken);
            return _mapper.Map<AddTrackResponse>(responseDto);
        }

        public override async Task<UpdateResponse> Update(UpdateRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<UpdateRequestDto>(request);
            var responseDto = await _playlistService.UpdateAsync(requestDto, context.CancellationToken);
            return _mapper.Map<UpdateResponse>(responseDto);
        }

        public override async Task<DeleteResponse> Delete(DeleteRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<DeleteRequestDto>(request);
            var responseDto = await _playlistService.DeleteAsync(requestDto, context.CancellationToken);
            return _mapper.Map<DeleteResponse>(responseDto);
        }
    }
}
