namespace Soundy.CatalogService.Dto.PlaylistDtos;

public class SearchResponseDto
{
    public string Pattern { get; set; }
    public int PageSize { get; set; }
    public int PageNum { get; set; }
    public List<PlaylistDto> Playlists { get; set; }
} 