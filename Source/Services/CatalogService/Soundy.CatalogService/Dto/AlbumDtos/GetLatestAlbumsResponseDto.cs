namespace Soundy.CatalogService.Dto.AlbumDtos;

public class GetLatestAlbumsResponseDto
{
    /// <summary>
    /// Список последних созданных альбомов
    /// </summary>
    public List<AlbumDto> Albums { get; set; } = new List<AlbumDto>();
} 