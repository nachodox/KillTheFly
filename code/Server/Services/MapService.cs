using KillTheFly.Server.Controllers;
using KillTheFly.Shared;
using System.ComponentModel.DataAnnotations;

namespace KillTheFly.Server.Services;

public class MapService
{
    const int MAX_NUMBER_OF_FOES = 20;
    const int MAP_SIZE = 20;
    readonly List<int> LEFT = new List<int> { 0, 3, 6 };
    readonly List<int> TOP = new List<int> { 0, 1, 2 };
    readonly List<int> RIGHT = new List<int> { 2, 5, 8 };
    readonly List<int> BOTTOM = new List<int> { 6, 7, 8 };

    private Dictionary<string, MapRepresentation> actors;
    public MapService()
    {
        actors = new Dictionary<string, MapRepresentation>();
    }

    public IEnumerable<MapRepresentation> GetFullMap()
    {
        return actors.Select(actor => actor.Value);
    }
    public void MovePlayerMap(string guidPlayer, int movement)
    {
        var player = actors[guidPlayer];
        if(LEFT.Contains(movement) && player.X < 1 ||
            TOP.Contains(movement) && player.Y < 1 ||
            RIGHT.Contains(movement) && player.X >= MAP_SIZE - 1 ||
            BOTTOM.Contains(movement) && player.Y >= MAP_SIZE - 1)
        {
            return;
        }
        var neighbors = GetPlayerMap(guidPlayer);
        var neighbor = neighbors.ElementAt(movement);
        if(neighbor.IsPlayer)
        {
            return;
        }
        if(neighbor.Guid?.Length > 0)
        {
            actors.Remove(neighbor.Guid);
        }
        if (LEFT.Contains(movement))
        {
            player.X--;
        }
        if(TOP.Contains(movement))
        {
            player.Y--;
        }
        if(RIGHT.Contains(movement))
        {
            player.X++;
        }
        if(BOTTOM.Contains(movement))
        {
            player.Y++;
        }
    }
    public int GetFoesNumber()
    {
        return actors.Count(actor => !actor.Value.IsPlayer);
    }
    public IEnumerable<MapRepresentation> GetPlayerMap(string guidPlayer)
    {
        var player = actors[guidPlayer];
        var closeSpaces = new List<MapRepresentation>();
        for(int  j = -1; j <= 1; j++)
        {
            var y = player.Y + j;
            for(int i = -1; i <= 1; i++)
            {
                var x = player.X + i;
                var neighbor = actors
                    .Select(keyValue => keyValue.Value)
                    .FirstOrDefault(actor => actor.X == x && actor.Y == y);
                closeSpaces.Add(neighbor ?? MapRepresentation.EmptySpace(x, y));
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
        var actorRepresentation = new MapRepresentation
        {
            X = x,
            Y = y,
            Avatar = avatar,
            Guid = guid,
            IsPlayer = isPlayer
        };
        actors.Add(guid, actorRepresentation);
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