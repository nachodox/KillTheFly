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

    private MapService mapService;

    public GameSnapshot GetPlayerMap(string playerGuid)
    {
        var scoreBoard = new ScoreBoard
        {
            Foes = mapService.GetFoesNumber(),
            GameTime = GameTime,
            MilisecondsPast = (int)(DateTime.Now - GameStart).TotalMilliseconds,
            Score = mapService.GetPlayerScore(playerGuid),
            TotalKills = mapService.GetTotalKills()
        };
        return new GameSnapshot
        {
            Map = mapService.GetPlayerMap(playerGuid).Select(entity => entity.ToShared()),
            Board = scoreBoard
        };
    }
    public GameService(KTFDatabaseContext context)
    {
        timer = new Timer(TIC_MILISECONDS);
        timer.Elapsed += Timer_Elapsed;
        mapService = new MapService(context);
    }


    public async Task MovePLayer(string playerGuid, Directions? direction = null)
    {

        await AddPlayer(playerGuid);
        if (direction.HasValue)
        {
            await ExecuteMove(playerGuid, direction.Value);
        }
    }

    private async Task ExecuteMove(string playerGuid, Directions direction)
    {
        try
        {
            await mapService.MoveEntity(playerGuid, direction);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task AddPlayer(string playerGuid)
    {
        if (!timer.Enabled)
        {
            Start();
        }
        await mapService.AddPlayer(playerGuid);
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

    private async void Timer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        GameTime++;
        if (GameTime % 3 == 0)
        {
            try
            {
                await mapService.MoveFlies();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        if (GameTime % 50 == 0)
        {
            await mapService.AddFly();
        }
        if(GameTime % 1000 == 0)
        {
            //mapService.DropOffline();
        }
    }
}
