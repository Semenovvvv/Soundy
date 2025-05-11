namespace Soundy.CatalogService.Dto.TrackDtos;

public class GetListByUserIdResponseDto
{
    public IEnumerable<TrackDto> Tracks { get; set; }
}

