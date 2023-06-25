using System.Timers;
using KillTheFly.Shared;

using Timer = System.Timers.Timer;

namespace KillTheFly.Server.Services;
public class GameService
{
    const int TIC_MILISECONDS = 1000;
    public int GameTime { get; set; }
    public DateTime GameStart { get; set; }

    private Timer timer;
    private Dictionary<string, string> playerGuidMap;
    private MapService mapService;

    public GameSnapshot GetPlayerMap(string playerGuid)
    {
        var playerMapGuid = playerGuidMap[playerGuid];
        var scoreBoard = new ScoreBoard
        {
            Foes = mapService.GetFoesNumber(),
            GameTime = GameTime,
            MilisecondsPast = (int)(DateTime.Now - GameStart).TotalMilliseconds,
            Score = mapService.GetPlayerScore(playerMapGuid),
            TotalKills = mapService.GetTotalKills()
        };
        return new GameSnapshot
        {
            Map = mapService.GetPlayerMap(playerMapGuid),
            Board = scoreBoard
        };
    }

    public GameService()
    {
        timer = new Timer(TIC_MILISECONDS);
        timer.Elapsed += Timer_Elapsed;
        playerGuidMap = new Dictionary<string, string>();
        mapService = new MapService();
        Enumerable.Range(0,4).ToList().ForEach(_ => mapService.AddFly());
    }


    public void MovePLayer(string playerGuid, Directions? direction = null)
    {
        if (!playerGuidMap.ContainsKey(playerGuid))
        {
            AddPlayer(playerGuid);
        }
        if (direction.HasValue)
        {
            ExecuteMove(playerGuid, direction.Value);
        }
    }

    private void ExecuteMove(string playerGuid, Directions direction)
    {
        var playerMapGuid = playerGuidMap[playerGuid];
        mapService.MoveEntity(playerMapGuid, direction);
    }

    private void AddPlayer(string playerGuid)
    {
        if (playerGuidMap.Count == 0)
        {
            Start();
        }
        var mapGuid = Guid.NewGuid().ToString();
        playerGuidMap.Add(playerGuid, mapGuid);
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
        if (GameTime % 3 == 0)
        {
            mapService.MoveFlies();
        }
        if (GameTime % 50 == 0)
        {
            mapService.AddFly();
        }
        if(GameTime % 1000 == 0)
        {
            mapService.DropOffline();
        }
    }
}
