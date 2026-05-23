using TankiForum.DTOs.Sections;

namespace TankiForum.Services.Interfaces;

public interface ISectionService
{
    Task<SectionDto?> GetByIdAsync(int id);
    Task<List<SectionDto>> GetAllAsync(int? categoryId = null);
    Task<SectionDto> CreateAsync(CreateSectionRequest request);
    Task<SectionDto?> UpdateAsync(int id, UpdateSectionRequest request);
    Task<bool> DeleteAsync(int id);
}
