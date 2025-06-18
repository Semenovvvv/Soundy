namespace Soundy.CatalogService.Entities
{
    /// <summary>
    /// Связь между пользователем и лайкнутым треком
    /// </summary>
    public class LikedTrack
    {
        public Guid UserId { get; set; }
        public Guid TrackId { get; set; }
        public Track Track { get; set; }
        public DateTime LikedAt { get; set; } = DateTime.UtcNow;
    }
} 