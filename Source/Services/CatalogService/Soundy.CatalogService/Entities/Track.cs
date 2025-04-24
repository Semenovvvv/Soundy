namespace Soundy.CatalogService.Entities
{
    /// <summary> Трек </summary>
    public class Track
    {
        /// <summary> Идентификатор </summary>
        public Guid Id { get; set; }

        /// <summary> Название </summary>
        public string Title { get; set; }

        /// <summary> Идентификатор пользователя, загрузившего трек </summary>
        public Guid UserId { get; set; }

        /// <summary> Идентификатор плейлиста </summary>
        public Guid PlaylistId { get; set; }

        /// <summary> Плейлист </summary>
        public Playlist Playlist { get; set; }

        /// <summary> Длительность в секундах </summary>
        public int Duration { get; set; }

        /// <summary> Дата загрузки </summary>
        public DateTime CreatedAt { get; set; }
    }
}
