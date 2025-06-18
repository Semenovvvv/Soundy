using AutoMapper;
using Grpc.Core;
using Service.Track;
using Soundy.CatalogService.Dto.TrackDtos;
using Soundy.CatalogService.Helpers;
using Soundy.CatalogService.Interfaces;

namespace Soundy.CatalogService.Controllers
{
    public class TrackGrpcController : TrackGrpcService.TrackGrpcServiceBase
    {
        private readonly ITrackService _trackService;
        private readonly IMapper _mapper;

        public TrackGrpcController(ITrackService trackService, IMapper mapper)
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
            requestDto.UserId = UserContextHelper.GetUserId(context);
            var responseDto = await _trackService.GetByIdAsync(requestDto, context.CancellationToken);
            return _mapper.Map<GetByIdResponse>(responseDto);
        }

        /// <summary>
        /// Выполняет поиск треков по строке запроса
        /// </summary>
        /// <param name="request">Параметры поиска</param>
        /// <param name="context">Контекст gRPC запроса</param>
        /// <returns>Результаты поиска треков</returns>
        public override async Task<SearchResponse> Search(SearchRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<SearchRequestDto>(request);
            requestDto.UserId = UserContextHelper.GetUserId(context);
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
        /// Получает список треков по идентификатору пользователя
        /// </summary>
        /// <param name="dto">Идентификатор пользователя</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список треков пользователя</returns>
        public override async Task<GetListByUserIdResponse> GetListByUserId(GetListByUserIdRequest request, ServerCallContext context)
        {
            var requestDto = _mapper.Map<GetListByUserIdRequestDto>(request);
            requestDto.UserId = UserContextHelper.GetUserId(context) ?? Guid.Parse(request.UserId);
            var responseDto = await _trackService.GetListByUserId(requestDto, context.CancellationToken);
            return _mapper.Map<GetListByUserIdResponse>(responseDto);
        }

        /// <summary>
        /// Добавляет лайк треку от пользователя
        /// </summary>
        /// <param name="request">Данные трека и пользователя</param>
        /// <param name="context">Контекст gRPC запроса</param>
        /// <returns>Результат операции добавления лайка</returns>
        public override async Task<LikeTrackResponse> LikeTrack(LikeTrackRequest request, ServerCallContext context)
        {
            var userId = UserContextHelper.GetUserId(context);
            if (!userId.HasValue)
                throw new RpcException(new Status(StatusCode.Unauthenticated, "User ID is required"));

            var requestDto = _mapper.Map<LikeTrackRequestDto>(request);
            requestDto.UserId = userId.Value;
            var responseDto = await _trackService.LikeTrackAsync(requestDto, context.CancellationToken);
            return _mapper.Map<LikeTrackResponse>(responseDto);
        }

        /// <summary>
        /// Удаляет лайк трека от пользователя
        /// </summary>
        /// <param name="request">Данные трека и пользователя</param>
        /// <param name="context">Контекст gRPC запроса</param>
        /// <returns>Результат операции удаления лайка</returns>
        public override async Task<UnlikeTrackResponse> UnlikeTrack(UnlikeTrackRequest request, ServerCallContext context)
        {
            var userId = UserContextHelper.GetUserId(context);
            if (!userId.HasValue)
                throw new RpcException(new Status(StatusCode.Unauthenticated, "User ID is required"));

            var requestDto = _mapper.Map<UnlikeTrackRequestDto>(request);
            requestDto.UserId = userId.Value;
            var responseDto = await _trackService.UnlikeTrackAsync(requestDto, context.CancellationToken);
            return _mapper.Map<UnlikeTrackResponse>(responseDto);
        }

        /// <summary>
        /// Получает список лайкнутых треков пользователя
        /// </summary>
        /// <param name="request">Идентификатор пользователя</param>
        /// <param name="context">Контекст gRPC запроса</param>
        /// <returns>Список лайкнутых треков</returns>
        public override async Task<GetLikedTracksResponse> GetLikedTracks(GetLikedTracksRequest request, ServerCallContext context)
        {
            var userId = UserContextHelper.GetUserId(context);
            if (!userId.HasValue)
                throw new RpcException(new Status(StatusCode.Unauthenticated, "User ID is required"));

            var requestDto = _mapper.Map<GetLikedTracksRequestDto>(request);
            requestDto.UserId = userId.Value;
            var responseDto = await _trackService.GetLikedTracksAsync(requestDto, context.CancellationToken);
            return _mapper.Map<GetLikedTracksResponse>(responseDto);
        }
    }
}
