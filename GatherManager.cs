using System;
using System.Threading;

public class GatherManager
{
    private GameState gameState;
    public Timer gatherTimer;

    public string gatherItem;
    
	public GatherManager()
	{
       
	}
    public void SetGameState(GameState state)
    {
        gameState = state;
    }

    public bool Gather(GameItem itemToGather)
    {
        gameState.currentGatherItem = itemToGather;
        bool gatherSuccess = gameState.GetPlayerInventory().AddItem(itemToGather);
        gameState.UpdateState();
        return gatherSuccess;
    }

}
