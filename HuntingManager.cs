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
        gameState.UpdateState();
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
        string hours = "";
        string minutes = "";
        string seconds = "";
        if(t.Hours < 10)
        {
            hours = "0" + t.Hours;
        }
        else
        {
            hours = "" + t.Hours;
        }
            
        if(t.Minutes < 10)
        {
            minutes = "0" + t.Minutes;
        }
        else
        {
            minutes = "" + t.Minutes;
        }

        if(t.Seconds < 10)
        {
           seconds =  "0" + t.Seconds;
        }
        else
        {
            seconds = "" + t.Seconds;
        }
        return hours + ":" + minutes + ":" + seconds;
        
    }
    public int GetNumberCaught()
    {
        double totalTimeHunting = currentHuntEndTime.Subtract(currentHuntStartTime).TotalSeconds;
        double caughtBase = totalTimeHunting * catchOdds * (double)(gameState.GetPlayer().GetLevel("Hunting") + 1);
        double caught = Extensions.GetGaussianRandom(caughtBase, caughtBase / 2);
        if(caught < 1)
        {
            caught = 1;
        }
        return (int)caught;
    }
    
}
