using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace testttt.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class FileUploadController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<FileUploadController> _logger;

    public FileUploadController(
        IWebHostEnvironment environment,
        ILogger<FileUploadController> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    [HttpPost("product-image")]
    public async Task<IActionResult> UploadProductImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        // Validate file type
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        if (!allowedExtensions.Contains(fileExtension))
        {
            return BadRequest("Invalid file type. Allowed types: jpg, jpeg, png, gif, webp");
        }

        // Validate file size (max 5MB)
        const long maxFileSize = 5 * 1024 * 1024; // 5MB
        if (file.Length > maxFileSize)
        {
            return BadRequest("File size exceeds 5MB limit");
        }

        try
        {
            // Determine the web root path - use WebRootPath if available, otherwise use ContentRootPath/wwwroot
            var webRootPath = _environment.WebRootPath;
            if (string.IsNullOrEmpty(webRootPath))
            {
                webRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
                _logger.LogInformation("WebRootPath was null, using ContentRootPath/wwwroot: {WebRootPath}", webRootPath);
            }

            // Create wwwroot directory if it doesn't exist
            if (!Directory.Exists(webRootPath))
            {
                Directory.CreateDirectory(webRootPath);
                _logger.LogInformation("Created wwwroot directory: {WebRootPath}", webRootPath);
            }

            // Create uploads directory if it doesn't exist
            var uploadsFolder = Path.Combine(webRootPath, "uploads", "products");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
                _logger.LogInformation("Created uploads/products directory: {UploadsFolder}", uploadsFolder);
            }

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the URL path (relative to wwwroot)
            var imageUrl = $"/uploads/products/{fileName}";
            
            _logger.LogInformation("Product image uploaded successfully: {ImageUrl}", imageUrl);
            
            return Ok(new { imageUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading product image");
            return StatusCode(500, "Error uploading file");
        }
    }

    [HttpDelete("product-image")]
    public IActionResult DeleteProductImage([FromQuery] string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
        {
            return BadRequest("Image URL is required");
        }

        try
        {
            // Determine the web root path - use WebRootPath if available, otherwise use ContentRootPath/wwwroot
            var webRootPath = _environment.WebRootPath;
            if (string.IsNullOrEmpty(webRootPath))
            {
                webRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
            }

            // Remove leading slash if present
            var relativePath = imageUrl.TrimStart('/');
            var filePath = Path.Combine(webRootPath, relativePath);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                _logger.LogInformation("Product image deleted: {ImageUrl}", imageUrl);
                return Ok(new { message = "Image deleted successfully" });
            }

            return NotFound("Image not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product image");
            return StatusCode(500, "Error deleting file");
        }
    }
}

