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

        /// <summary>
        /// Создает новый альбом
        /// </summary>
        /// <param name="request">Запрос на создание альбома</param>
        /// <param name="context">Контекст gRPC запроса</param>
        /// <returns>Ответ с информацией о созданном альбоме</returns>
        public override async Task<CreateResponse> Create(CreateRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<CreateRequestDto>(request);
            var responseDto = await _albumService.CreateAsync(requestDto, context.CancellationToken);
            return _mapper.Map<CreateResponse>(responseDto);
        }

        /// <summary>
        /// Добавляет трек в альбом
        /// </summary>
        /// <param name="request">Запрос с идентификатором альбома и трека</param>
        /// <param name="context">Контекст gRPC запроса</param>
        /// <returns>Ответ с обновленной информацией об альбоме</returns>
        public override async Task<AddTrackResponse> AddTrack(AddTrackRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<AddTrackRequestDto>(request);
            var responseDto = await _albumService.AddTrackAsync(requestDto, context.CancellationToken);
            return _mapper.Map<AddTrackResponse>(responseDto);
        }
        
        /// <summary>
        /// Получает информацию об альбоме по идентификатору
        /// </summary>
        /// <param name="request">Запрос с идентификатором альбома</param>
        /// <param name="context">Контекст gRPC запроса</param>
        /// <returns>Ответ с детальной информацией об альбоме</returns>
        public override async Task<GetByIdResponse> GetById(GetByIdRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<GetByIdRequestDto>(request);
            var responseDto = await _albumService.GetByIdAsync(requestDto, context.CancellationToken);
            return _mapper.Map<GetByIdResponse>(responseDto);
        }
    }
}
