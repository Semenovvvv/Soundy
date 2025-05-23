using Soundy.CatalogService.Dto.AlbumDtos;

namespace Soundy.CatalogService.Dto.AlbumDtos
{
    public class GetByAuthorIdResponseDto
    {
        public IEnumerable<AlbumDto> Albums { get; set; }
    }
} 