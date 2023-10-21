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

    [HttpGet("Static/{x}/{y}")]
    public ActionResult KillTheFlyImage(string x, string y)
    {
        var image = ImageHelper.GetImage($"{x}-{y}");
        return Ok(image);
    }


    [HttpGet("Map/{mapId}")]
    public async Task<ActionResult> GetImages(string map)
    {
        var images = await _context.ImageTiles.Where(image => image.Map == map).Select(image => image.ToShared()).ToListAsync();
        return Ok(images);
    }

    [HttpPost("Map")]
    public async Task<ActionResult> MoveAsync([FromBody] dynamic body)
    {
        await _context.ImageTiles.AddAsync(new Models.ImageTile
        {
            ImageBase64 = body.imageBase64,
            LocationX = body.locationX,
            LocationY = body.locationY,
            Map = body.map,
        });
        await _context.SaveChangesAsync();
        return Ok();
    }

}
