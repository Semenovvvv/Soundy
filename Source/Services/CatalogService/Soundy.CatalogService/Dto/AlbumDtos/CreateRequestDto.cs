namespace Soundy.CatalogService.Dto.AlbumDtos;

public class CreateRequestDto
{
    public string Title { get; set; }
    public string AvatarUrl { get; set; }
    public Guid AuthorId { get; set; }
}