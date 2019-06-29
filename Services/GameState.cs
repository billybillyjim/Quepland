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

    public int huntingAreaID;
    public DateTime huntingEndTime;
    public DateTime huntingStartTime;

    public bool isAutoCollecting;

    public bool isSplitView;
    public bool inventoryIsActiveView;
    public bool compactBankView;

    public bool saveDataLoaded;
    public bool gameDataLoaded;
    public bool saveGameExists;

    public string previousURL;
    public string updateVersionString = "1.017b";
    
    public GameItem currentUsedItem;
    public GameItem currentGatherItem;
    public GameItem currentBuffItem;

    public int sushiHouseRice;
    public int sushiHouseSeaweed;

    public int bankWithdrawAmount;

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

    private SimpleAES Encryptor = new SimpleAES();

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
    public string GetSaveString(AreaManager areaManager, FollowerManager followerManager, NPCManager npcManager, BuildingManager buildingManager)
    {
        string data = "";
        //Bank 0
        data += "" + GetPlayerBank().GetInventory().ToString();
        //Skills 1
        data += "#" + GetPlayer().GetSkillString();
        //Inventory 2
        data += "#" + GetPlayerInventory().ToString();
        //Areas 3
        data += "#" + areaManager.SaveAreas();
        //Followers 4
        data += "#" + followerManager.ToString();
        //HP 5
        data += "#" + GetPlayer().CurrentHP.ToString();
        //ActiveFollower 6
        if (GetPlayer().activeFollower != null)
        {
            data += "#" + GetPlayer().activeFollower.id;
        }
        else
        {
            data += "#";
        }
        //Recipes 7
        data += "#";
        foreach (string s in GetPlayer().GetRecipes())
        {
            data += s + "/";
        }
        //EquippedItems 8
        data += "#";
        foreach (KeyValuePair<GameItem, int> pair in GetPlayerInventory().GetEquippedItems())
        {
            data += pair.Key.Id + "/";
        }
        //Settings 9
        data += "#";
        data += isSplitView.ToString();
        data += ",";
        data += compactBankView.ToString();
        //NPC data 10
        data += "#";
        data += npcManager.GetNPCData();
        //Sushi House Data 11
        data += "#";
        data += sushiHouseRice + "," + sushiHouseSeaweed;
        //Tannery Data 12
        data += "#";
        foreach (Building b in buildingManager.GetBuildings())
        {
            if (b.Salt > 0)
            {
                data += "" + b.ID + "," + b.Salt + "/";
            }
        }
        //Tannery Slot Data 13
        data += "#";
        foreach (Building b in buildingManager.GetBuildings())
        {
            if (b.IsTannery)
            {
                data += b.ID + ">";
                foreach (TanningSlot slot in b.TanneryItems)
                {
                    data += slot.GetString() + "_";
                }
                data += "@";
            }
        }
        //GameState.isHunting 14
        data += "#";
        data += isHunting.ToString() + ",";
        data += huntingAreaID + ",";
        data += huntingStartTime.ToString() + ",";
        data += huntingEndTime.ToString();
        data = Encryptor.EncryptToString(data);
        return data;
    }
}
