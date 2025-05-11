namespace Soundy.CatalogService.Dto.PlaylistDtos
{
    public class AddTrackRequestDto
    {
        public Guid PlaylistId { get; set; }
        public Guid TrackId { get; set; }
    }
}
