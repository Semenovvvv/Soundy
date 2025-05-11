namespace Soundy.CatalogService.Dto
{
    public class PlaylistTrackDto
    {
        public Guid PlaylistId { get; set; }
        public PlaylistDto Playlist { get; set; }
        public Guid TrackId { get; set; }
        public TrackDto Track { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
