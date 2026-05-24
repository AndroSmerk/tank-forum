using TankiForum.DTOs.Posts;

namespace TankiForum.Services.Interfaces;

public interface IPostService
{
    Task<List<PostDto>> GetByTopicIdAsync(int topicId);
    Task<PostDto> CreateAsync(int userId, CreatePostRequest request);
    Task<PostDto?> UpdateAsync(int id, int userId, UpdatePostRequest request);
    Task<bool> DeleteAsync(int id);
    Task<bool> LikePostAsync(int postId, int userId);
}
