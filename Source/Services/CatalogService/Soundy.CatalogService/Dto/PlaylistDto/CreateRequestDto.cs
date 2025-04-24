namespace Soundy.CatalogService.Dto.PlaylistDto
{
    public class CreateRequestDto
    {
        public string Name { get; set; }
        public Guid AuthorId { get; set; }
    }
}
