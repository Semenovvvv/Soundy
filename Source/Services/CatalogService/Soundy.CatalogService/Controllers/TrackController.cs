using AutoMapper;
using Grpc.Core;
using Service.Track;
using Soundy.CatalogService.Dto.TrackDtos;
using Soundy.CatalogService.Interfaces;

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

        /// <summary>
        /// Создает новый трек
        /// </summary>
        /// <param name="request">Запрос на создание трека</param>
        /// <param name="context">Контекст gRPC запроса</param>
        /// <returns>Ответ с информацией о созданном треке</returns>
        public override async Task<CreateResponse> Create(CreateRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<CreateRequestDto>(request);
            var responseDto = await _trackService.CreateAsync(requestDto, context.CancellationToken);
            return _mapper.Map<CreateResponse>(responseDto);
        }

        /// <summary>
        /// Получает трек по идентификатору
        /// </summary>
        /// <param name="request">Запрос с идентификатором трека</param>
        /// <param name="context">Контекст gRPC запроса</param>
        /// <returns>Ответ с детальной информацией о треке</returns>
        public override async Task<GetByIdResponse> GetById(GetByIdRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<GetByIdRequestDto>(request);
            var responseDto = await _trackService.GetByIdAsync(requestDto, context.CancellationToken);
            return _mapper.Map<GetByIdResponse>(responseDto);
        }

        //public override async Task<GetListByPlaylistResponse> GetListByPlaylist(GetListByPlaylistRequest request, ServerCallContext context)
        //{
        //    var requestDto = _mapper.Map<GetListByPlaylistRequestDto>(request);
        //    var responseDto = await _trackService.GetListByPlaylistAsync(requestDto, context.CancellationToken);
        //    return _mapper.Map<GetListByPlaylistResponse>(responseDto);
        //}

        /// <summary>
        /// Выполняет поиск треков по строке запроса
        /// </summary>
        /// <param name="request">Параметры поиска</param>
        /// <param name="context">Контекст gRPC запроса</param>
        /// <returns>Результаты поиска треков</returns>
        public override async Task<SearchResponse> Search(SearchRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<SearchRequestDto>(request);
            var responseDto = await _trackService.SearchAsync(requestDto, context.CancellationToken);
            return _mapper.Map<SearchResponse>(responseDto);
        }

        /// <summary>
        /// Обновляет информацию о треке
        /// </summary>
        /// <param name="request">Данные для обновления трека</param>
        /// <param name="context">Контекст gRPC запроса</param>
        /// <returns>Обновленная информация о треке</returns>
        public override async Task<UpdateResponse> Update(UpdateRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<UpdateRequestDto>(request);
            var responseDto = await _trackService.UpdateAsync(requestDto, context.CancellationToken);
            return _mapper.Map<UpdateResponse>(responseDto);
        }

        /// <summary>
        /// Удаляет трек
        /// </summary>
        /// <param name="request">Запрос на удаление трека</param>
        /// <param name="context">Контекст gRPC запроса</param>
        /// <returns>Результат операции удаления</returns>
        public override async Task<DeleteResponse> Delete(DeleteRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<DeleteRequestDto>(request);
            var responseDto = await _trackService.DeleteAsync(requestDto, context.CancellationToken);
            return _mapper.Map<DeleteResponse>(responseDto);
        }

        /// <summary>
        /// Получает список треков пользователя
        /// </summary>
        /// <param name="request">Запрос с идентификатором пользователя</param>
        /// <param name="context">Контекст gRPC запроса</param>
        /// <returns>Список треков пользователя</returns>
        public override async Task<GetListByUserIdResponse> GetListByUserId(GetListByUserIdRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<GetListByUserIdRequestDto>(request);
            var responseDto = await _trackService.GetListByUserIdRequest(requestDto, context.CancellationToken);
            return _mapper.Map<GetListByUserIdResponse>(responseDto);
        }
    }
}
