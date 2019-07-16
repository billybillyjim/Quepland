using System;

public class HuntingManager
{
    private DateTime currentHuntStartTime;
    private DateTime currentHuntEndTime;
    private GameState gameState;
    private readonly double catchOdds = 1 / 3600d;

    public HuntingManager()
    {
        
    }
    public void SetGameState(GameState state)
    {
        gameState = state;
    }

    public void BeginHunt(int huntLength)
    {
        currentHuntStartTime = DateTime.UtcNow;
        currentHuntEndTime = DateTime.UtcNow + TimeSpan.FromHours(huntLength);
        gameState.huntingEndTime = currentHuntEndTime;
        gameState.huntingStartTime = currentHuntStartTime;
        gameState.UpdateState();
    }
    public void LoadHunt(DateTime start, DateTime end)
    {
        currentHuntStartTime = start;
        currentHuntEndTime = end;
    }
    public void EndHunt()
    {
        gameState.isHunting = false;
        gameState.UpdateState();

    }
    public DateTime GetStartTime()
    {
        return currentHuntStartTime;
    }
    public DateTime GetEndTime()
    {
        return currentHuntEndTime;
    }
    public double GetTimeUntilDone()
    {
        return currentHuntEndTime.Subtract(DateTime.UtcNow).TotalSeconds;
    }
    public string GetTimeUntilDoneString()
    {
        TimeSpan t = currentHuntEndTime.Subtract(DateTime.UtcNow);
        string timeLeft = "" + t.Hours.ToString("00") + ":" + t.Minutes.ToString("00") + ":" + t.Seconds.ToString("00");
        return timeLeft;
        
    }
    public int GetNumberCaught()
    {
        double totalTimeHunting = currentHuntEndTime.Subtract(currentHuntStartTime).TotalSeconds;
        double caughtBase = totalTimeHunting * catchOdds * (double)((gameState.GetPlayer().GetLevel("Hunting") + 4) / 4d);
        double caught = Extensions.GetGaussianRandom(caughtBase, caughtBase / 3);
        int catchFloor = ((gameState.GetPlayer().GetLevel("Archery") / 10) + 1) * Math.Max((int)(totalTimeHunting / 3600), 1);
        if(caught < catchFloor)
        {
            caught = catchFloor;
        }
        return (int)caught;
    }
    
}
