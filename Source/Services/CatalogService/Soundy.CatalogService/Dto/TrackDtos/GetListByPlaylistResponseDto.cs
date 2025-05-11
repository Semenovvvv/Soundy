namespace Soundy.CatalogService.Dto.TrackDtos;

public class GetListByPlaylistResponseDto
{
    public Guid PlaylistId { get; set; }
    public PlaylistDto Playlist { get; set; }
    public List<TrackDto> Tracks { get; set; }
}