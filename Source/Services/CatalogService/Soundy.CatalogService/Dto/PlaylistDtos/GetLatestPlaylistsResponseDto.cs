namespace Soundy.CatalogService.Dto.PlaylistDtos;

public class GetLatestPlaylistsResponseDto
{
    /// <summary>
    /// Список последних созданных плейлистов
    /// </summary>
    public List<PlaylistDto> Playlists { get; set; } = new List<PlaylistDto>();
} 