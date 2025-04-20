namespace Soundy.CatalogService.Dto.PlaylistDto
{
    public class CreatePlaylistRequestDto
    {
        public Guid AuthorId { get; set; }
        public string Name { get; set; }
    }
}
