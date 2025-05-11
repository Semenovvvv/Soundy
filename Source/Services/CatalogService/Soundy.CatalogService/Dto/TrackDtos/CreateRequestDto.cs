namespace Soundy.CatalogService.Dto.TrackDtos
{
    public class CreateRequestDto
    {
        public string Title { get; set; }
        public Guid AuthorId { get; set; }
        public Guid AlbumId { get; set; }
        public int Duration { get; set; }
        public string AvatarUrl { get; set; }
    }
}
