using Microsoft.EntityFrameworkCore;
using TankiForum.Data;
using TankiForum.DTOs.Sections;
using TankiForum.Models;
using TankiForum.Services.Interfaces;

namespace TankiForum.Services;

public class SectionService : ISectionService
{
    private readonly AppDbContext _context;

    public SectionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SectionDto?> GetByIdAsync(int id)
    {
        var section = await _context.ForumSections
            .Include(s => s.Category)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (section is null) return null;

        return new SectionDto
        {
            Id = section.Id,
            CategoryId = section.CategoryId,
            Name = section.Name,
            Description = section.Description,
            CategoryName = section.Category.Name
        };
    }

    public async Task<List<SectionDto>> GetAllAsync(int? categoryId = null)
    {
        var query = _context.ForumSections
            .Include(s => s.Category)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(s => s.CategoryId == categoryId.Value);

        return await query
            .OrderBy(s => s.CategoryId)
            .ThenBy(s => s.Id)
            .Select(s => new SectionDto
            {
                Id = s.Id,
                CategoryId = s.CategoryId,
                Name = s.Name,
                Description = s.Description,
                CategoryName = s.Category.Name
            })
            .ToListAsync();
    }

    public async Task<SectionDto> CreateAsync(CreateSectionRequest request)
    {
        var categoryExists = await _context.ForumCategories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
            throw new InvalidOperationException("Category not found.");

        var section = new ForumSection
        {
            CategoryId = request.CategoryId,
            Name = request.Name,
            Description = request.Description
        };

        _context.ForumSections.Add(section);
        await _context.SaveChangesAsync();

        return new SectionDto
        {
            Id = section.Id,
            CategoryId = section.CategoryId,
            Name = section.Name,
            Description = section.Description
        };
    }

    public async Task<SectionDto?> UpdateAsync(int id, UpdateSectionRequest request)
    {
        var section = await _context.ForumSections.FindAsync(id);
        if (section is null) return null;

        section.Name = request.Name;
        section.Description = request.Description;

        await _context.SaveChangesAsync();

        return new SectionDto
        {
            Id = section.Id,
            CategoryId = section.CategoryId,
            Name = section.Name,
            Description = section.Description
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var section = await _context.ForumSections.FindAsync(id);
        if (section is null) return false;

        _context.ForumSections.Remove(section);
        await _context.SaveChangesAsync();
        return true;
    }
}
