using System;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Threading;

public class GameState
{
	public event EventHandler StateChanged;

    public bool isGathering;
    public bool isHunting;
    public bool canSell;
    public bool canBank;

    public bool saveDataLoaded;
    public bool gameDataLoaded;

    public string previousURL;
    
    public GameItem currentUsedItem;
    public GameItem currentGatherItem;
    private Player player = new Player();

    public Area currentArea;

    public Timer attackTimer;
    public Timer foodTimer;

    private void StateHasChanged()
    {
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
    public void UpdateState()
    {
        StateHasChanged();
    }
    public GameState()
    {

    }
    public Player GetPlayer()
    {
        return player;
    }
    public Inventory GetPlayerInventory()
    {
        return player.inventory;
    }
    public Bank GetPlayerBank()
    {
        return player.bank;
    }
    public void LoadPlayerData(HttpClient Http)
    {
        player.LoadSkills(Http);
    }
}
