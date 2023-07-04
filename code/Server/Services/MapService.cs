using KillTheFly.Server.Controllers;
using KillTheFly.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace KillTheFly.Server.Services;

public class MapService
{
    const int MAX_NUMBER_OF_FOES = 20;
    const int MAP_SIZE = 20;

    private Dictionary<string, GameEntity> actors;
    private List<Kill> kills;
    private List<Movement> movements;
    private readonly KTFDatabaseContext _context;
    public MapService(KTFDatabaseContext context)
    {
        _context = context;
        actors = new Dictionary<string, GameEntity>();
        foreach(var entity in _context.Entities.ToArray())
        {
            if(entity.GuidClient is not null)
            {
                actors.Add(entity.GuidClient, entity);
            }
        }
        kills = new List<Kill>();
        foreach(var kill in _context.Kills.ToArray())
        {
            kills.Add(kill);
        }
        movements = new List<Movement>();
        foreach(var movement in _context.Movements.ToArray())
        {
            movements.Add(movement);
        }
    }

    public IEnumerable<GameEntity> GetFullMap()
    {
        return actors.Select(actor => actor.Value);
    }
    public async Task MoveEntity(string playerMapGuid, Shared.Directions direction)
    {
        var entity = actors[playerMapGuid];
        if(entity.IsPlayer)
        {
            //entity.IsOnline = true;
            //entity.LastAccess = DateTime.Now;
        }
        if (Shared.Movement.LEFT.Contains(direction) && entity.X < 1 ||
            Shared.Movement.UP.Contains(direction) && entity.Y < 1 ||
            Shared.Movement.RIGHT.Contains(direction) && entity.X >= MAP_SIZE - 1 ||
            Shared.Movement.DOWN.Contains(direction) && entity.Y >= MAP_SIZE - 1)
        {
            return;
        }
        var movement = new Movement
        {
            Direction = direction,
            Entity = entity,
            StartX = entity.X,
            StartY = entity.Y,
            EndX = entity.X,
            EndY = entity.Y
        };
        var neighbors = GetPlayerMap(playerMapGuid);
        var neighbor = neighbors.ElementAt((int)direction);
        if(neighbor.IsPlayer)
        {
            try
            {
                await _context.Movements.AddAsync(movement);
                await _context.SaveChangesAsync();
                movements.Add(movement);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex);
                ex.Entries.Single().Reload();
                //await _context.SaveChangesAsync();
            }
            return;
        }
        if(neighbor.GuidClient?.Length > 0)
        {
            if(!entity.IsPlayer)
            {
                return;
            }
            movements.Add(movement);
            var kill = new Kill()
            {
                KillerMovement = movement,
                Victim = neighbor
            };
            try
            {
                await _context.Movements.AddAsync(movement);
                await _context.Kills.AddAsync(kill);
                await _context.SaveChangesAsync();
                kills.Add(kill);
                actors.Remove(neighbor.GuidClient);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex);
                ex.Entries.Single().Reload();
                //await _context.SaveChangesAsync();
            }
            return;
        }
        if (Shared.Movement.LEFT.Contains(direction))
        {
            entity.X--;
        }
        if(Shared.Movement.UP.Contains(direction))
        {
            entity.Y--;
        }
        if(Shared.Movement.RIGHT.Contains(direction))
        {
            entity.X++;
        }
        if(Shared.Movement.DOWN.Contains(direction))
        {
            entity.Y++;
        }
        movement.EndX = entity.X;
        movement.EndY = entity.Y;
        try
        {
            await _context.Movements.AddAsync(movement);
            await _context.SaveChangesAsync();
            movements.Add(movement);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            Console.WriteLine(ex);
            ex.Entries.Single().Reload();
            //await _context.SaveChangesAsync();
        }
    }
    public int GetFoesNumber()
    {
        return actors.Count(actor => !actor.Value.IsPlayer);
    }
    public int GetTotalKills() => kills.Count;
    public int GetPlayerScore(string guidClient) 
    {
        if(!actors.ContainsKey(guidClient))
        {
            return 0;
        }
        return actors[guidClient].Movements.Where(movement => movement.Kill != null).Count();
    }
    
    public IEnumerable<GameEntity> GetPlayerMap(string guidClient)
    {
        if(!actors.ContainsKey(guidClient))
        {
            return new List<GameEntity>();
        }
        var player = actors[guidClient];
        var closeSpaces = new List<GameEntity>();
        for(int  j = -1; j <= 1; j++)
        {
            var y = player.Y + j;
            for(int i = -1; i <= 1; i++)
            {
                var x = player.X + i;
                var neighbor = actors
                    .Select(keyValue => keyValue.Value)
                    .FirstOrDefault(actor => actor.X == x && actor.Y == y && actor.GuidClient != guidClient);
                var emptyChar = x >= 0 && x < MAP_SIZE && y >= 0 && y < MAP_SIZE ? 'F' : 'W';
                if(emptyChar == 'F' && 
                    kills.Any(kill => 
                        kill?.Timestamp > DateTime.Now.AddSeconds(-10) && 
                        kill?.Victim?.X == x && 
                        kill?.Victim?.Y == y))
                {
                    emptyChar = 'D';
                }
                var emptyEntity = GameEntity.EmptySpace(x, y, emptyChar);
                closeSpaces.Add(neighbor ?? emptyEntity);
            }
        }
        return closeSpaces;
    }
    private async Task AddActor(string guid, char avatar, bool isPlayer)
    {
        if (actors.ContainsKey(guid))
        {
            return;
        }
        var (x, y) = GetRandomEmptyPoint();
        var actorRepresentation = new GameEntity
        {
            X = x,
            Y = y,
            StartY = y,
            StartX = x,
            Avatar = avatar,
            GuidClient = guid,
            IsPlayer = isPlayer
        };
        try
        {
            await _context.Entities.AddAsync(actorRepresentation);
            await _context.SaveChangesAsync();
            actors.Add(guid, actorRepresentation);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            Console.WriteLine(ex);
            ex.Entries.Single().Reload();
            //await _context.SaveChangesAsync();
        }
        catch(InvalidOperationException ex)
        {
            Console.WriteLine(ex);
        }
    }
    public async Task MoveFlies()
    {
        var players = actors.Values.Where(actor => actor.IsPlayer);
        var flies = actors.Values.Where(actor => !actor.IsPlayer);
        var fliesToMove = flies.Where(fly => players.Any(player => 
            Math.Abs(player.X - fly.X) <= 2 && Math.Abs(player.Y - fly.Y) <= 2));
        foreach(var fly in fliesToMove)
        {
            await MoveEntity(fly.GuidClient ?? "", (Shared.Directions)Random.Shared.Next(0, 8));
        }
    }
    public async Task AddFly()
    {
        if (GetFoesNumber() >= MAX_NUMBER_OF_FOES)
        {
            return;
        }
        await AddActor(Guid.NewGuid().ToString(), 'Z', false);
    }

    public async Task AddPlayer(string guid)
    {
        await AddActor(guid, (actors.Count(actor => actor.Value.IsPlayer) + 1).ToString().FirstOrDefault(), true);
    }
    private (int X, int Y) GetRandomEmptyPoint()
    {
        int x, y;
        do
        {
            x = Random.Shared.Next(0, MAP_SIZE);
            y = Random.Shared.Next(0, MAP_SIZE);
        } while (actors.Any(actor => actor.Value.X == x && actor.Value.Y == y));
        return (x, y);
    }
}