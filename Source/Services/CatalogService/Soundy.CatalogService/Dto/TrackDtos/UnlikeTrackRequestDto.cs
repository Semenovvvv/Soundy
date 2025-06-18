namespace Soundy.CatalogService.Dto.TrackDtos
{
    public class UnlikeTrackRequestDto
    {
        public Guid TrackId { get; set; }
        public Guid UserId { get; set; }
    }
} 