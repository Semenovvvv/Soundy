using AutoMapper;
using Google.Protobuf;
using Grpc.Core;
using Soundy.CatalogService.Interfaces;

namespace Soundy.CatalogService.Controllers
{
    public class TrackGrpcController : TrackGrpcService.TrackGrpcServiceBase
{
    private readonly ITrackMetadataService _metadataService;
    private readonly ITrackFileService _fileService;
    private readonly IMapper _mapper;

    public TrackGrpcController(
        ITrackMetadataService metadataService,
        ITrackFileService fileService,
        IMapper mapper)
    {
        _metadataService = metadataService;
        _fileService = fileService;
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

    public override async Task<DeleteTrackResponse> DeleteTrack(DeleteTrackRequest request, ServerCallContext context)
    {
        var isDeleted = await _metadataService.DeleteTrackAsync(request.Id, context.CancellationToken);
        return _mapper.Map<DeleteTrackResponse>(isDeleted);
    }

    public override async Task<UploadTrackFileResponse> UploadTrackFile(
        IAsyncStreamReader<UploadTrackFileRequest> requestStream,
        ServerCallContext context)
    {
        var memoryStream = new MemoryStream();
        string trackId = null;

        await foreach (var request in requestStream.ReadAllAsync())
        {
            if (request.DataCase == UploadTrackFileRequest.DataOneofCase.TrackId)
            {
                trackId = request.TrackId;
            }
            else
            {
                await memoryStream.WriteAsync(request.Chunk.ToByteArray());
            }
        }

        memoryStream.Position = 0;
        await _fileService.UploadTrackAsync(trackId, memoryStream);

        return new UploadTrackFileResponse
        {
            FileUrl = $"{trackId}" // Или полный URL к MinIO
        };
    }

    public override async Task DownloadTrackFile(
        DownloadTrackFileRequest request,
        IServerStreamWriter<DownloadTrackFileResponse> responseStream,
        ServerCallContext context)
    {
        var fileStream = await _fileService.DownloadTrackAsync(request.TrackId);
        var buffer = new byte[8192];
        int bytesRead;

        while ((bytesRead = await fileStream.ReadAsync(buffer)) > 0)
        {
            await responseStream.WriteAsync(new DownloadTrackFileResponse
            {
                Chunk = ByteString.CopyFrom(buffer, 0, bytesRead)
            });
        }
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
    }

    // Аналогично реализуйте SearchTracks, GetTracksByPlaylist и т.д.
}
}
