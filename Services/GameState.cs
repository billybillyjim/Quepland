using System;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Threading;
using Microsoft.JSInterop;

public class GameState
{
	public event EventHandler StateChanged;
    public IJSRuntime JSRuntime;

    public bool isGathering;
    public bool isHunting;
    public bool isWorkingOut;
    public bool isRunning;
    public bool isSmithing;
    public bool isFighting;
    public bool canSell;
    public bool canBank;
    public bool isUsing { get; set; }
    public bool PetShopUnlocked;

    public string userID;
    public string token;

    public int huntingAreaID;
    public DateTime huntingEndTime;
    public DateTime huntingStartTime;

    public bool isAutoCollecting;

    public bool isSplitView;
    public bool inventoryIsActiveView;
    public string activeView = "Skills";
    public List<string> activeButtons = new List<string>() { "Skills", "Inventory" };
    public bool compactBankView;
    public bool autoBuySushiSupplies;
    public bool hideLockedItems;

    public bool saveDataLoaded;
    public bool gameDataLoaded;
    public bool saveGameExists;
    public bool safeToSave = true;
    public bool safeToLoad = false;

    public string previousURL;
    public string updateVersionString = "1.1e";

    public string gatherItem;


    public GameItem currentUsedItem;
    public GameItem currentGatherItem;
    public GameItem currentBuffItem;

    public int sushiHouseRice;
    public int sushiHouseSeaweed;

    public int bankWithdrawAmount;

    public int buffSecondsLeft;

    public int bankSortStyle;

    public int expensiveItemThreshold = 10000;

    public int totalKills;
    public List<int> killCount = new List<int>();
    public int totalDeaths;
    public long totalCoinsEarned;

    private Player player = new Player();

    public Area currentArea;
    public string currentRegion = "Quepland";
    public string previousArea;

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
    public int saveToPlayFab = 0;
    public int saveToPlayFabEvery = 5;

    public Pet petToBuy;
    public List<Pet> buyablePets = new List<Pet>();

    private static SimpleAES Encryptor = new SimpleAES();

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

    public void RestorePet(string petID)
    {
        foreach(Pet pet in buyablePets)
        {
            if(pet.Identifier == petID && GetPlayer().HasPet(pet) == false)
            {
                GetPlayer().Pets.Add(pet);
            }
        }
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
    public void LoadMonsters(List<Monster> monsters)
    {
        foreach(Monster m in monsters)
        {
            killCount.Add(0);
        }
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
        gatherItem = "";
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
        activeButtons.Clear();
        if (isSplitView)
        {
            
            activeButtons.Add("Skills");
            activeButtons.Add("Inventory");
        }
        else
        {

            activeView = "Inventory";
            
        }
        UpdateState();
    }
    public void ToggleActiveView()
    {
        inventoryIsActiveView = !inventoryIsActiveView;
        UpdateState();
    }
    public void IncrementKillCount(int monsterID)
    {
        killCount[monsterID] += 1;
    }
    public async void GetKongregateLogin()
    {
        userID = await JSRuntime.InvokeAsync<string>("kongregateFunctions.getUserID");
        token = await JSRuntime.InvokeAsync<string>("kongregateFunctions.getToken");
    }
    public string GetSaveStringEncrypted(AreaManager areaManager, FollowerManager followerManager, NPCManager npcManager, BuildingManager buildingManager, bool encrypt)
    {

        int pos = 0;
        string data = "";
        try
        {
            //Bank 0
            data += "" + GetPlayerBank().GetInventory().ToString();
            pos++;
            //Skills 1
            data += "#" + GetPlayer().GetSkillString();
            pos++;
            //Inventory 2
            data += "#" + GetPlayerInventory().ToStringSorted();
            pos++;
            //Areas 3
            data += "#" + areaManager.SaveAreas();
            pos++;
            //Followers 4
            data += "#" + followerManager.ToString();
            pos++;
            //HP 5
            data += "#" + GetPlayer().CurrentHP.ToString();
            pos++;
            //ActiveFollower 6
            if (GetPlayer().activeFollower != null)
            {
                data += "#" + GetPlayer().activeFollower.id;
            }
            else
            {
                data += "#";
            }
            pos++;
            //Recipes 7
            data += "#";
            /*foreach (string s in GetPlayer().GetRecipes())
            {
                data += s + "/";
            }*/
            pos++;
            //EquippedItems 8
            data += "#";
            foreach (KeyValuePair<GameItem, int> pair in GetPlayerInventory().GetEquippedItems())
            {
                data += pair.Key.Id + "/";
            }
            pos++;
            //Settings 9
            data += "#";
            data += isSplitView.ToString();
            data += ",";
            data += compactBankView.ToString();
            data += ",";
            data += expensiveItemThreshold;
            data += ",";
            data += totalKills;
            data += ",";
            data += PetShopUnlocked.ToString();
            data += ",";
            data += autoBuySushiSupplies.ToString();
            data += ",";
            data += totalCoinsEarned;
            data += ",";
            data += totalDeaths;
            pos++;
            //NPC data 10
            data += "#";
            data += npcManager.GetNPCData();
            pos++;
            //Sushi House Data 11
            data += "#";
            data += sushiHouseRice + "," + sushiHouseSeaweed;
            pos++;
            //Tannery Data 12
            data += "#";
            foreach (Building b in buildingManager.GetBuildings())
            {
                if (b.Salt > 0)
                {
                    data += "" + b.ID + "," + b.Salt + "/";
                }
            }
            pos++;
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
            pos++;
            //GameState.isHunting 14
            data += "#";
            data += isHunting.ToString() + ",";
            data += huntingAreaID + ",";
            data += huntingStartTime.ToString() + ",";
            data += huntingEndTime.ToString();
            pos++;
            //Bank Tabs 15
            data += "#";
            data += GetPlayerBank().GetTabsString();
            pos++;
            //Pets 16
            data += "#";
            data += GetPlayer().GetPetString();
            pos++;
            //KC 17
            data += "#";
            data += GetKCString();
            if (encrypt)
            {
                data = Encryptor.EncryptToString(data);
            }
            pos++;
        }
        catch
        {
            data = "Failed to generate save file. Please contact the developer to let him know he messed up. (Error line:" + pos + ")";
        }
        return data;
    }
    public string GetSaveString(AreaManager areaManager, FollowerManager followerManager, NPCManager npcManager, BuildingManager buildingManager)
    {
        return GetSaveStringEncrypted(areaManager, followerManager, npcManager, buildingManager, true);
    }
    private string GetKCString()
    {
        string data = "";
        foreach(int i in killCount)
        {
            data += i + ",";
        }
        return data;
    }
    public void LoadKC(string kcString)
    {
        string[] data = kcString.Split(',');
        int i = 0;
        foreach(string line in data)
        {
            if(line.Length > 0)
            {
                killCount[i] = int.Parse(line);
                i++;
            }
        }
    }
}
