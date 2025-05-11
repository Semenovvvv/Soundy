namespace Soundy.CatalogService.Dto.TrackDtos;

public class SearchResponseDto
{
    public string Pattern { get; set; }
    public int PageSize { get; set; }
    public int PageNum { get; set; }
    public List<TrackDto> Tracks { get; set; }
}