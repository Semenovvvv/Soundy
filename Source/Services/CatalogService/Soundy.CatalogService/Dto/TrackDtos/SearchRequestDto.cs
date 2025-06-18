namespace Soundy.CatalogService.Dto.TrackDtos;

public class SearchRequestDto
{
    public string Pattern { get; set; }
    public int PageSize { get; set; }
    public int PageNum { get; set; }
    public Guid? UserId { get; set; }
}