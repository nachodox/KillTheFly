using KillTheFly.Server.Services;
using KillTheFly.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KillTheFly.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{
    private readonly ILogger<ImageController> _logger;
    private readonly KTFDatabaseContext _context;

    public ImageController(ILogger<ImageController> logger, KTFDatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("Map/{mapId}")]
    public async Task<ActionResult> Register(string map)
    {
        var images = await _context.ImageTiles.Where(image => image.Map == map).Select(image => image.ToShared()).ToListAsync();
        return Ok(images);
    }

    [HttpPost("Map")]
    public async Task<ActionResult> MoveAsync([FromBody] string map, [FromBody] string imageBase64, [FromBody] int locationX, [FromBody] int locationY)
    {
        await _context.ImageTiles.AddAsync(new Models.ImageTile
        {
            ImageBase64 = imageBase64,
            LocationX = locationX,
            LocationY = locationY,
            Map = map,
        });
        return Ok();
    }

}
