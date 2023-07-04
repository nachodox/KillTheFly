﻿using KillTheFly.Server.Services;
using KillTheFly.Shared;
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
    public async Task<ActionResult> Register(string playerGuid)
    {
        await _gameService.MovePLayer(playerGuid);
        return Ok();
    }

    [HttpPost("Move/{playerGuid}/{direction}")]
    public async Task<ActionResult> MoveAsync(string playerGuid, Directions direction)
    {
        await _gameService.MovePLayer(playerGuid, direction);
        return Ok();
    }

    [HttpGet("Map/{playerGuid}")]
    public IActionResult Map(string playerGuid)
    {
        var map = _gameService.GetPlayerMap(playerGuid);

        return Ok(map);
    }
}
