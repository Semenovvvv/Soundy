namespace Soundy.CatalogService.Dto.PlaylistDtos;

public class SearchRequestDto
{
    public string Pattern { get; set; }
    public int PageSize { get; set; }
    public int PageNum { get; set; }
} 