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
    public bool isWorkingOut;
    public bool isRunning;
    public bool isSmithing;
    public bool isFighting;
    public bool canSell;
    public bool canBank;
    public bool isUsing;

    public bool isAutoCollecting;

    public bool isSplitView;
    public bool inventoryIsActiveView;
    public bool compactBankView;

    public bool saveDataLoaded;
    public bool gameDataLoaded;
    public bool saveGameExists;

    public string previousURL;
    public string updateVersionString = "1.015b";
    
    public GameItem currentUsedItem;
    public GameItem currentGatherItem;
    public GameItem currentBuffItem;

    public int sushiHouseRice;
    public int sushiHouseSeaweed;

    public int buffSecondsLeft;

    public int bankSortStyle;

    private Player player = new Player();

    public Area currentArea;

    public Timer attackTimer;
    public Timer foodTimer;
    public Timer UIRefreshTimer;

    //Area Menu Timers
    public Timer gatherTimer;
    public Timer huntCountdownTimer;
    public Timer autoCollectTimer;
    public Timer miningVeinSearchTimer;
    public Timer workoutTimer;

    //Fighting Menu Timers
    public Timer monsterAttackTimer;
    public Timer autoFightTimer;

    //Inventory Timer
    public Timer createRepeatTimer;

    //Smithing Timer
    public Timer smithingTimer;
    public Timer autoSmithingTimer;

    //Navbar Timer
    public Timer autoSaveTimer;

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
    public void ToggleBankStyle()
    {
        compactBankView = !compactBankView;
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
    public void SetBuffItem(GameItem item)
    {
        currentBuffItem = item;
        buffSecondsLeft = item.HealDuration;
    }
    public bool CanLeave()
    {
        if(!isGathering && !isHunting && !isWorkingOut && !isSmithing && !isRunning && !isFighting)
        {
            return true;
        }
        return false;
    }
    public void StopActions()
    {
        isWorkingOut = false;
        isRunning = false;
        isGathering = false;
        isHunting = false;
        isSmithing = false;
        isFighting = false;

        if(attackTimer != null)
        {
            attackTimer.Dispose();
            attackTimer = null;
        }
        if (gatherTimer != null)
        {
            gatherTimer.Dispose();
            gatherTimer = null;
        }
        if (huntCountdownTimer != null)
        {
            huntCountdownTimer.Dispose();
            huntCountdownTimer = null;
        }
        if (autoCollectTimer != null)
        {
            autoCollectTimer.Dispose();
            autoCollectTimer = null;
        }
        if (miningVeinSearchTimer != null)
        {
            miningVeinSearchTimer.Dispose();
            miningVeinSearchTimer = null;
        }
        if (workoutTimer != null)
        {
            workoutTimer.Dispose();
            workoutTimer = null;
        }
        if (monsterAttackTimer != null)
        {
            monsterAttackTimer.Dispose();
            monsterAttackTimer = null;
        }
        if (autoFightTimer != null)
        {
            autoFightTimer.Dispose();
            autoFightTimer = null;
        }
        if (createRepeatTimer != null)
        {
            createRepeatTimer.Dispose();
            createRepeatTimer = null;
        }
        if (smithingTimer != null)
        {
            smithingTimer.Dispose();
            smithingTimer = null;
        }
        if (autoSmithingTimer != null)
        {
            autoSmithingTimer.Dispose();
            autoSmithingTimer = null;
        }
        if(UIRefreshTimer != null)
        {
            UIRefreshTimer.Dispose();
            UIRefreshTimer = null;
        }
        UpdateState();
    }
    public void ToggleSplitView()
    {
        isSplitView = !isSplitView;
        UpdateState();
    }
    public void ToggleActiveView()
    {
        inventoryIsActiveView = !inventoryIsActiveView;
        UpdateState();
    }
}
