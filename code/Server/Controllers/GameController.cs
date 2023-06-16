using KillTheFly.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace KillTheFly.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase
{
    private readonly ILogger<GameController> _logger;
    private readonly GameService _gameService;

    public GameController(ILogger<GameController> logger, GameService gameService)
    {
        _logger = logger;
        _gameService = gameService;
    }

    [HttpPost("Register/{playerGuid}")]
    public ActionResult Register(string playerGuid)
    {
        _gameService.PlayerAction(playerGuid);
        return Ok();
    }

    [HttpPost("Move/{playerGuid}/{movement}")]
    public ActionResult Move(string playerGuid, int movement)
    {
        _gameService.PlayerAction(playerGuid, movement);
        return Ok();
    }
    [HttpGet("FullMap")]
    public IActionResult FullMap()
    {
        var fullMap = _gameService.GetFullMap();
        var timer = fullMap.GameTime;
        var miliseconds = fullMap.MilisecondsPast;
        var map = fullMap.Map.ToArray();

        return Ok(new { action = "action", timer, miliseconds, map });
    }

    [HttpGet("Map/{playerGuid}")]
    public IActionResult Map(string playerGuid)
    {
        var map = _gameService.GetPlayerMap(playerGuid);

        return Ok(map);
    }
}
