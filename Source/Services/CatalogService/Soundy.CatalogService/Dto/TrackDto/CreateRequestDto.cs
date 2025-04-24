namespace Soundy.CatalogService.Dto.TrackDto
{
    public class CreateRequestDto
    {
        public string Title { get; set; }
        public Guid UserId { get; set; }
        public Guid PlaylistId { get; set; }
        public int Duration { get; set; }
    }
}
