namespace Soundy.CatalogService.Dto.AlbumDtos;

public class SearchResponseDto
{
    public string Pattern { get; set; }
    public int PageSize { get; set; }
    public int PageNum { get; set; }
    public List<AlbumDto> Albums { get; set; }
} 