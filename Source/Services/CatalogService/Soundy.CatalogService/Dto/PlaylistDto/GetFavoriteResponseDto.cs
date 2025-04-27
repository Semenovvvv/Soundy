namespace Soundy.CatalogService.Dto.PlaylistDto;

public class GetFavoriteResponseDto
{
    public PlaylistDto Playlist { get; set; }
    public IEnumerable<TrackDto> Tracks { get; set; }
}