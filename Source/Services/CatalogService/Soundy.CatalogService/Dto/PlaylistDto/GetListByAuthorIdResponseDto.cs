namespace Soundy.CatalogService.Dto.PlaylistDto;

public class GetListByAuthorIdResponseDto
{
    public Guid AuthorId { get; set; }
    public List<PlaylistDto> Playlists { get; set; }
}