using KillTheFly.Server.Controllers;
using KillTheFly.Shared;
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
    public MapService()
    {
        actors = new Dictionary<string, GameEntity>();
        kills = new List<Kill>();
        movements = new List<Movement>();
    }

    public IEnumerable<GameEntity> GetFullMap()
    {
        return actors.Select(actor => actor.Value);
    }
    public void MoveEntity(string playerMapGuid, Directions direction)
    {
        var entity = actors[playerMapGuid];
        if(entity.IsPlayer)
        {
            entity.IsOnline = true;
            entity.LastAccess = DateTime.Now;
        }
        if (Movement.LEFT.Contains(direction) && entity.X < 1 ||
            Movement.UP.Contains(direction) && entity.Y < 1 ||
            Movement.RIGHT.Contains(direction) && entity.X >= MAP_SIZE - 1 ||
            Movement.DOWN.Contains(direction) && entity.Y >= MAP_SIZE - 1)
        {
            return;
        }
        var movement = new Movement
        {
            Direction = direction,
            Entity = entity,
            MoveDate = DateTime.Now,
            X = entity.X,
            Y = entity.Y
        };
        movements.Add(movement);
        var neighbors = GetPlayerMap(playerMapGuid);
        var neighbor = neighbors.ElementAt((int)direction);
        if(neighbor.IsPlayer)
        {
            return;
        }
        if(neighbor.Guid?.Length > 0)
        {
            if(!entity.IsPlayer)
            {
                return;
            }
            kills.Add(new Kill(neighbor, movement));
            actors.Remove(neighbor.Guid);
            return;
        }
        if (Movement.LEFT.Contains(direction))
        {
            entity.X--;
        }
        if(Movement.UP.Contains(direction))
        {
            entity.Y--;
        }
        if(Movement.RIGHT.Contains(direction))
        {
            entity.X++;
        }
        if(Movement.DOWN.Contains(direction))
        {
            entity.Y++;
        }
    }
    public int GetFoesNumber()
    {
        return actors.Count(actor => !actor.Value.IsPlayer);
    }
    public int GetTotalKills() => kills.Count;
    public int GetPlayerScore(string playerMapGuid) => kills.Where(kill => kill?.KillerMovement?.Entity?.Guid == playerMapGuid).Count();
    
    public IEnumerable<GameEntity> GetPlayerMap(string playerMapGuid)
    {
        var player = actors[playerMapGuid];
        var closeSpaces = new List<GameEntity>();
        for(int  j = -1; j <= 1; j++)
        {
            var y = player.Y + j;
            for(int i = -1; i <= 1; i++)
            {
                var x = player.X + i;
                var neighbor = actors
                    .Select(keyValue => keyValue.Value)
                    .FirstOrDefault(actor => actor.X == x && actor.Y == y);
                var emptyChar = x >= 0 && x < MAP_SIZE && y >= 0 && y < MAP_SIZE ? 'F' : 'W';
                if(emptyChar == 'F' && 
                    kills.Any(kill => 
                        kill.EventDate > DateTime.Now.AddSeconds(-10) && 
                        kill.Victim.X == x && 
                        kill.Victim.Y == y))
                {
                    emptyChar = 'D';
                }
                var emptyEntity = GameEntity.EmptySpace(x, y, emptyChar);
                closeSpaces.Add(neighbor ?? emptyEntity);
            }
        }
        return closeSpaces;
    }
    private void AddActor(string guid, char avatar, bool isPlayer)
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
            Avatar = avatar,
            Guid = guid,
            IsPlayer = isPlayer
        };
        actors.Add(guid, actorRepresentation);
    }
    public void DropOffline()
    {
        foreach(var player in actors.Values.Where(actor => actor.IsPlayer && actor.LastAccess < DateTime.Now.AddMinutes(3)))
        {
            player.IsOnline = false;
        }
    }
    public void MoveFlies()
    {
        var players = actors.Values.Where(actor => actor.IsPlayer && actor.IsOnline);
        var flies = actors.Values.Where(actor => !actor.IsPlayer);
        var fliesToMove = flies.Where(fly => players.Any(player => 
            Math.Abs(player.X - fly.X) <= 2 && Math.Abs(player.Y - fly.Y) <= 2));
        foreach(var fly in fliesToMove)
        {
            MoveEntity(fly.Guid, (Directions)Random.Shared.Next(0, 8));
        }
    }
    public void AddFly()
    {
        if(GetFoesNumber() >= MAX_NUMBER_OF_FOES)
        {
            return;
        }
        AddActor(Guid.NewGuid().ToString(), 'Z', false);
    }

    public void AddPlayer(string guid)
    {
        AddActor(guid, (actors.Count(actor => actor.Value.IsPlayer) + 1).ToString().FirstOrDefault(), true);
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