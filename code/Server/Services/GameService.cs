using System.Timers;
using KillTheFly.Shared;

using Timer = System.Timers.Timer;

namespace KillTheFly.Server.Services;
public class GameService
{
    const int TIC_MILISECONDS = 200;
    public int GameTime { get; set; }
    public DateTime GameStart { get; set; }

    private Timer timer;
    private Dictionary<string, string> players;
    private MapService mapService;

    public MapSnapshot GetFullMap()
    {
        return new MapSnapshot
        {
            Foes = mapService.GetFoesNumber(),
            GameTime = GameTime,
            MilisecondsPast = (int)(DateTime.Now - GameStart).TotalMilliseconds,
            Map = mapService.GetFullMap()
        };
    }
    public MapSnapshot GetPlayerMap(string guidPlayer)
    {
        var guidPlayerMap = players[guidPlayer];
        return new MapSnapshot
        {
            Foes = mapService.GetFoesNumber(),
            GameTime = GameTime,
            MilisecondsPast = (int)(DateTime.Now - GameStart).TotalMilliseconds,
            Map = mapService.GetPlayerMap(guidPlayerMap)
        };
    }

    public GameService()
    {
        timer = new Timer(TIC_MILISECONDS);
        timer.Elapsed += Timer_Elapsed;
        players = new Dictionary<string, string>();
        mapService = new MapService();
    }


    public void PlayerAction(string playerToken, int? movement = null)
    {
        if (!players.ContainsKey(playerToken))
        {
            AddPlayer(playerToken);
        }
        if (movement.HasValue)
        {
            ExecuteMove(playerToken, movement.Value);
        }
    }

    private void ExecuteMove(string guidPlayer, int movement)
    {
        var guidPlayerMap = players[guidPlayer];
        mapService.MovePlayerMap(guidPlayerMap, movement);
    }

    private void AddPlayer(string playerGuid)
    {
        if (players.Count == 0)
        {
            Start();
        }
        var mapGuid = Guid.NewGuid().ToString();
        players.Add(playerGuid, mapGuid);
        mapService.AddPlayer(mapGuid);
    }

    private void Start()
    {
        if (GameTime > 0)
        {
            return;
        }
        GameTime = 0;
        GameStart = DateTime.Now;
        timer.Start();
    }

    private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        GameTime++;
        if (GameTime % 50 == 0)
        {
            mapService.AddFly();
        }
    }
}
