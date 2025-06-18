using Soundy.CatalogService.Dto;

namespace Soundy.CatalogService.Dto.TrackDtos;

public class SearchResponseDto
{
    public string Pattern { get; set; }
    public int PageSize { get; set; }
    public int PageNum { get; set; }
    public int PageCount { get; set; }
    public int TotalCount { get; set; }
    public IList<TrackDto> Tracks { get; set; }
}