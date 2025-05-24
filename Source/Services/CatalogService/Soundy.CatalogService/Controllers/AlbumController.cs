using AutoMapper;
using Grpc.Core;
using Service.Album;
using Soundy.CatalogService.Dto.AlbumDtos;
using Soundy.CatalogService.Entities;
using Soundy.CatalogService.Interfaces;
using System.Collections.Generic;

namespace Soundy.CatalogService.Controllers
{
    public class AlbumGrpcController : AlbumGrpcService.AlbumGrpcServiceBase
    {
        private readonly IAlbumService _albumService;
        private readonly IMapper _mapper;

        public AlbumGrpcController(IAlbumService albumService, IMapper mapper)
        {
            _albumService = albumService;
            _mapper = mapper;
        }

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

        /// <summary>
        /// Получает список альбомов по идентификатору автора
        /// </summary>
        /// <param name="request">Запрос с идентификатором автора</param>
        /// <param name="context">Контекст gRPC запроса</param>
        /// <returns>Ответ со списком альбомов автора</returns>
        public override async Task<GetByAuthorIdResponse> GetByAuthorId(GetByAuthorIdRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<GetByAuthorIdRequestDto>(request);
            var responseDto = await _albumService.GetByAuthorIdAsync(requestDto, context.CancellationToken);
            return _mapper.Map<GetByAuthorIdResponse>(responseDto);
        }

        /// <summary>
        /// Выполняет поиск альбомов по названию
        /// </summary>
        /// <param name="request">Параметры поиска</param>
        /// <param name="context">Контекст gRPC запроса</param>
        /// <returns>Результаты поиска альбомов</returns>
        public override async Task<SearchResponse> Search(SearchRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<SearchRequestDto>(request);
            var responseDto = await _albumService.SearchAsync(requestDto, context.CancellationToken);
            return _mapper.Map<SearchResponse>(responseDto);
        }

        /// <summary>
        /// Получает список последних созданных альбомов
        /// </summary>
        /// <param name="request">Запрос с количеством записей</param>
        /// <param name="context">Контекст gRPC запроса</param>
        /// <returns>Список последних созданных альбомов</returns>
        public override async Task<GetLatestAlbumsResponse> GetLatestAlbums(GetLatestAlbumsRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<GetLatestAlbumsRequestDto>(request);
            var responseDto = await _albumService.GetLatestAlbumsAsync(requestDto, context.CancellationToken);
            return _mapper.Map<GetLatestAlbumsResponse>(responseDto);
        }
    }
}
