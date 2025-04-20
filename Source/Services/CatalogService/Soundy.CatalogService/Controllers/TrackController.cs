using AutoMapper;
using Grpc.Core;
using Soundy.CatalogService.Interfaces;

namespace Soundy.CatalogService.Controllers
{
    public class TrackGrpcController : TrackGrpcService.TrackGrpcServiceBase
    {
        private readonly ITrackMetadataService _metadataService;
        private readonly IMapper _mapper;

        public TrackGrpcController(
            ITrackMetadataService metadataService,
            IMapper mapper)
        {
            _metadataService = metadataService;
            _mapper = mapper;
        }

        public override async Task<TrackResponse> CreateTrack(CreateTrackRequest request, ServerCallContext context)
        {
            var track = await _metadataService.CreateTrackAsync(request);
            return _mapper.Map<TrackResponse>(track);
        }

        public override async Task<TrackResponse> GetTrack(GetTrackRequest request, ServerCallContext context)
        {
            var track = await _metadataService.GetTrackAsync(request.Id);
            return _mapper.Map<TrackResponse>(track);
        }

        public override async Task<DeleteTrackResponse> DeleteTrack(DeleteTrackRequest request,
            ServerCallContext context)
        {
            var isDeleted = await _metadataService.DeleteTrackAsync(request.Id, context.CancellationToken);
            return _mapper.Map<DeleteTrackResponse>(isDeleted);
        }

        // Потоковые методы (пример для ListTracks)
        public override async Task ListTracks(
            ListTracksRequest request,
            IServerStreamWriter<TrackResponse> responseStream,
            ServerCallContext context)
        {
            await foreach (var track in _metadataService.ListTracksAsync(request.Page, request.PageSize))
            {
                await responseStream.WriteAsync(_mapper.Map<TrackResponse>(track));
            }

            // Аналогично реализуйте SearchTracks, GetTracksByPlaylist и т.д.
        }
    }
}
