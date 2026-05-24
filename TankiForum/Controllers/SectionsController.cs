using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TankiForum.DTOs.Sections;
using TankiForum.Services.Interfaces;

namespace TankiForum.Controllers;

/// <summary>
/// Manage forum sections (subdivisions within categories).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SectionsController : ControllerBase
{
    private readonly ISectionService _sectionService;

    public SectionsController(ISectionService sectionService)
    {
        _sectionService = sectionService;
    }

    /// <summary>
    /// Get all sections, optionally filtered by category.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? categoryId)
    {
        var sections = await _sectionService.GetAllAsync(categoryId);
        return Ok(sections);
    }

    /// <summary>
    /// Create a new section in a category (requires authentication).
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create([FromBody] CreateSectionRequest request)
    {
        var section = await _sectionService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = section.Id }, section);
    }

    /// <summary>
    /// Get a specific section by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var section = await _sectionService.GetByIdAsync(id);
        if (section is null) return NotFound();
        return Ok(section);
    }

    /// <summary>
    /// Update an existing section (requires authentication).
    /// </summary>
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSectionRequest request)
    {
        var section = await _sectionService.UpdateAsync(id, request);
        if (section is null) return NotFound();
        return Ok(section);
    }

    /// <summary>
    /// Delete a section and all its topics (requires authentication).
    /// </summary>
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _sectionService.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}
