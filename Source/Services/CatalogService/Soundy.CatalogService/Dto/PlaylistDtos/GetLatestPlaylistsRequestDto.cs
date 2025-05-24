namespace Soundy.CatalogService.Dto.PlaylistDtos;

public class GetLatestPlaylistsRequestDto
{
    /// <summary>
    /// Количество записей для получения (по умолчанию 10)
    /// </summary>
    public int Count { get; set; } = 10;
} 