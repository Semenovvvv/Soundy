namespace Soundy.CatalogService.Dto.AlbumDtos;

public class GetLatestAlbumsRequestDto
{
    /// <summary>
    /// Количество записей для получения (по умолчанию 10)
    /// </summary>
    public int Count { get; set; } = 10;
} 