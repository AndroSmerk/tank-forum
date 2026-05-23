using Microsoft.AspNetCore.Mvc;

namespace TankiForum.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StorageController : ControllerBase
{
    private readonly IWebHostEnvironment _env;

    public StorageController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HttpGet("{name}")]
    public IActionResult GetFile(string name)
    {
        if (string.IsNullOrEmpty(name) || name.Contains(".."))
            return BadRequest();

        var uploadsDir = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "uploads");
        var filePath = Path.Combine(uploadsDir, name);

        if (!System.IO.File.Exists(filePath))
            return NotFound();

        var ext = Path.GetExtension(filePath).ToLowerInvariant();
        var contentType = ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            _ => "application/octet-stream"
        };

        return PhysicalFile(filePath, contentType);
    }
}
