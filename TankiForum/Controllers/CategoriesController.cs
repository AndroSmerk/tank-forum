using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankiForum.DTOs.Categories;
using TankiForum.Services.Interfaces;

namespace TankiForum.Controllers;

/// <summary>
/// Manage forum categories (top-level groupings of sections).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Get all forum categories.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Create a new forum category (requires authentication).
    /// </summary>
    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        var category = await _categoryService.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = category.Id }, category);
    }

    /// <summary>
    /// Update an existing forum category (requires authentication).
    /// </summary>
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest request)
    {
        var category = await _categoryService.UpdateAsync(id, request);
        if (category is null) return NotFound();
        return Ok(category);
    }

    /// <summary>
    /// Delete a forum category and all its sections (requires authentication).
    /// </summary>
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _categoryService.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}
