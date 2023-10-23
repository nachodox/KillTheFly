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

    [HttpGet("Map/{mapId}/{hash}")]
    public async Task<ActionResult> GetImage(string mapId, string hash)
    {
        var x = int.Parse(hash.Split('-')[0]);
        var y = int.Parse(hash.Split('-')[1]);
        var img = await _context.ImageTiles
            .Where(image => image.Map == mapId && image.LocationX == x && image.LocationY == y)
            .Select(image => image.ToShared()).FirstOrDefaultAsync();
        return Ok(img);
    }

    [HttpGet("Map/{mapId}")]
    public async Task<ActionResult> GetImages(string mapId)
    {
        var images = await _context.ImageTiles.Where(image => image.Map == mapId).Select(image => image.ToShared()).ToListAsync();
        return Ok(images);
    }


    [HttpPost("Map")]
    public async Task<ActionResult> CreateTile([FromBody]CreateTileDto body)
    {
        await _context.ImageTiles.AddAsync(new Models.ImageTile
        {
            ImageBase64 = body.ImageBase64,
            LocationX = body.LocationX,
            LocationY = body.LocationY,
            Map = body.Map,
            Timestamp = DateTime.Now
        });
        await _context.SaveChangesAsync();
        return Ok();
    }

}
