namespace Soundy.CatalogService.Dto.TrackDtos;

public class GetByIdRequestDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
}