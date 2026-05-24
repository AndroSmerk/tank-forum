using Microsoft.EntityFrameworkCore;
using TankiForum.Data;
using TankiForum.DTOs.Categories;
using TankiForum.Models;
using TankiForum.Services.Interfaces;

namespace TankiForum.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        return await _context.ForumCategories
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            })
            .ToListAsync();
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _context.ForumCategories.FindAsync(id);
        if (category is null) return null;

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request)
    {
        var category = new ForumCategory
        {
            Name = request.Name,
            Description = request.Description
        };

        _context.ForumCategories.Add(category);
        await _context.SaveChangesAsync();

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }

    public async Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        var category = await _context.ForumCategories.FindAsync(id);
        if (category is null) return null;

        category.Name = request.Name;
        category.Description = request.Description;

        await _context.SaveChangesAsync();

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _context.ForumCategories.FindAsync(id);
        if (category is null) return false;

        _context.ForumCategories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }
}
