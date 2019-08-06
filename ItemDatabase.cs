using System;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;

public class ItemDatabase
{
    private List<GameItem> items;
    private List<GameItem> smithingBarTable = new List<GameItem>();
    private List<GameItem> smithingItemTable = new List<GameItem>();
    private List<GameItem> huntedAnimals = new List<GameItem>();
    private List<GameItem> woodcuttingItems = new List<GameItem>();
    private List<GameItem> nails = new List<GameItem>();
    private List<Reward> rewards = new List<Reward>();
    private static Random rand = new Random();

    public ItemDatabase()
    {
        items = new List<GameItem>();
    }

    public async System.Threading.Tasks.Task LoadItems(HttpClient Http)
    {
        GameItem[] newItems = await Http.GetJsonAsync<GameItem[]>("data/items.json");
        items.AddRange(newItems);
        foreach(GameItem i in items)
        {
            if(i.ActionRequired == "Smithing" && i.ItemName != "Smithing Necklace")
            {
                smithingBarTable.Add(i);
            }
            else if (i.IsSmithable)
            {
                smithingItemTable.Add(i);
                if (i.ItemName.Contains("Nails"))
                {
                    nails.Add(i);
                }
            }
            else if(i.ActionRequired == "Hunting")
            {
                huntedAnimals.Add(i);
            }
            else if(i.ActionRequired == "Woodcutting")
            {
                woodcuttingItems.Add(i);
            }

        }
        LoadRewards(Http);
    }
    public async void LoadRewards(HttpClient Http)
    {
        Reward[] rewardsArray = await Http.GetJsonAsync<Reward[]>("data/rewards.json");
        rewards.AddRange(rewardsArray);
    }
    public Reward GetRewardByName(string name)
    {
        return rewards.Find(x => x.Name == name);
    }
    /// <summary>
    /// Gets the item by id. The fastest way to get an item from the database.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public GameItem GetItemByID(int id)
    {
        if(id == -1)
        {
            return null;
        }
        if(items.Count > id)
        {
            return items[id];
        }
        return null;
    }
    /// <summary>
    /// Gets the item by name. Use GetItemByID when possible as it is faster.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameItem GetItemByName(string name)
    {
        return items.Find(x => x.ItemName == name);
    }
    public List<GameItem> GetItems(int[] ids)
    {
        List<GameItem> returnList = new List<GameItem>();
        foreach(int id in ids)
        {
            returnList.Add(GetItemByID(id));
        }
        return returnList;
    }
    /// <summary>
    /// Gets all the items in the database.
    /// </summary>
    /// <returns></returns>
    public List<GameItem> GetAllItems()
    {
        return items;
    }
    /// <summary>
    /// Returns all usable items for a given item.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>

    public GameItem GetGold()
    {
        return items[0];
    }
    public GameItem GetItemByQueplarValue(int value)
    {
        foreach(GameItem item in smithingBarTable)
        {
            if(item.QueplarValue == value)
            {
                return item;
            }
        }
        //Returns alchemic dust if recipe not found.
        return GetItemByID(61);
    }
    public List<GameItem> GetItemsWithSkillRequirement(Skill skill)
    {
        List<GameItem> itemList = new List<GameItem>();

        foreach(GameItem item in items)
        {
            if (item.ActionRequired != null && item.ActionRequired == skill.SkillName)
            {
                itemList.Add(item);
            }
        }
        itemList = itemList.OrderBy(x => x.RequiredLevel).ToList();
        return itemList;
    }
    public List<GameItem> GetSmithingBars()
    {
        return smithingBarTable;
    }
    public List<GameItem> GetSmithingItems()
    {
        return smithingItemTable;
    }
    public List<GameItem> GetHuntedAnimals()
    {
        return huntedAnimals;
    }
    public GameItem GetItemWithIngredientID(int id)
    {
        foreach(GameItem item in items)
        {
            if(item.IngredientIDs != null && item.IngredientIDs.Contains(id))
            {
                return item;
            }
        }
        return null;
    }
    public List<int> GetAllItemIDs()
    {
        List<int> ids = new List<int>();
        for (int i = 0; i < items.Count; i++)
        {
            ids.Add(i);
        }
        return ids;
    }
    public List<GameItem> GetLogs()
    {
        return woodcuttingItems;
    }
    public List<GameItem> GetNails()
    {
        return nails;
    }
    public List<Reward> GetRewards()
    {
        return rewards;
    }
}
