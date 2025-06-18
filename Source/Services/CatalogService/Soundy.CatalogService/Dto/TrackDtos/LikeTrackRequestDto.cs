namespace Soundy.CatalogService.Dto.TrackDtos
{
    public class LikeTrackRequestDto
    {
        public Guid TrackId { get; set; }
        public Guid UserId { get; set; }
    }
} 