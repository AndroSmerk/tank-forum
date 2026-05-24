using TankiForum.DTOs.Common;
using TankiForum.DTOs.Topics;

namespace TankiForum.Services.Interfaces;

public interface ITopicService
{
    Task<TopicDto?> GetByIdAsync(int id);
    Task<PaginatedResponse<TopicListDto>> GetAllAsync(int? categoryId, int? sectionId, string? search, int page, int pageSize);
    Task<TopicDto> CreateAsync(int userId, CreateTopicRequest request);
    Task<TopicDto?> UpdateAsync(int id, int userId, UpdateTopicRequest request);
    Task<bool> DeleteAsync(int id);
}
