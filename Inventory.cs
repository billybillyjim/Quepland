using System;
using System.Collections.Generic;
using System.Linq;

public class Inventory
{
    private Dictionary<GameItem, int> items;
    private List<KeyValuePair<GameItem, int>> sortedItems = new List<KeyValuePair<GameItem, int>>();
    private int currentSortStyle = 0;
    private readonly int maxSize;
    private readonly int maxValue = int.MaxValue - 1000000;
    private int totalItems;
    public Inventory(int max)
    {
        items = new Dictionary<GameItem, int>();
        maxSize = max;
    }
    /// <summary>
    /// Returns the amount of that specific item in the inventory.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int GetNumberOfItem(GameItem item)
    {
        return items[item];
    }
    /// <summary>
    /// Returns the total amount of items in the inventory.
    /// </summary>
    /// <returns></returns>
    public int GetTotalNumberOfItems()
    {
        return totalItems;
    }
    public int GetAvailableSpaces()
    {
        return maxSize - totalItems;
    }
    public Dictionary<GameItem, int> GetItems()
    {
        return items;
    }
    /// <summary>
    /// Returns a sorted list of key value pairs of the items in an inventory. 0 sorts by name, 1 by amount, 2 by action group
    /// </summary>
    /// <param name="sortedBy"></param>
    /// <returns></returns>
    public List<KeyValuePair<GameItem, int>> GetItemsSorted(int sortedBy)
    {
        sortedItems = new List<KeyValuePair<GameItem, int>>();
        foreach (KeyValuePair<GameItem, int> i in items)
        {
            sortedItems.Add(i);
        }
        if (sortedBy == 0)
        {
            sortedItems = sortedItems.OrderBy(x => x.Key.ItemName).ToList();
        }
        else if (sortedBy == 1)
        {
            sortedItems = sortedItems.OrderBy(x => x.Value).Reverse().ToList();
        }
        else if (sortedBy == 2)
        {
            sortedItems = sortedItems.OrderBy(x => x.Key.ActionRequired).ToList();
        }
        else if(sortedBy == 3)
        {
            sortedItems = sortedItems.OrderBy(x => x.Key.Damage).Reverse().ToList();
        }
        return sortedItems;
    }
    public Dictionary<GameItem, int> GetUnequippedItems()
    {
        Dictionary<GameItem, int> unequippedItems = new Dictionary<GameItem, int>();
        foreach (KeyValuePair<GameItem, int> item in items)
        {
            if (item.Key.IsEquipped == false)
            {
                unequippedItems[item.Key] = item.Value;
            }
        }
        return unequippedItems;
    }
    public Dictionary<GameItem, int> GetEquippedItems()
    {
        Dictionary<GameItem, int> equippedItems = new Dictionary<GameItem, int>();
        foreach (KeyValuePair<GameItem, int> item in items)
        {
            if (item.Key.IsEquipped)
            {
                equippedItems[item.Key] = item.Value;
            }
        }
        return equippedItems;
    }
    public bool HasItem(GameItem item)
    {
        if (items.TryGetValue(item, out int temp))
        {
            return true;
        }
        return false;
    }
    public bool HasItem(int itemID)
    {
        foreach (KeyValuePair<GameItem, int> pair in items)
        {
            if (pair.Key.Id == itemID)
            {
                return true;
            }
        }
        return false;
    }
    public int GetCoins()
    {
        foreach (KeyValuePair<GameItem, int> pair in items)
        {
            if (pair.Key.Id == 0)
            {
                return pair.Value;
            }
        }
        return 0;
    }
    public GameItem GetAnyBar()
    {
        foreach (KeyValuePair<GameItem, int> pair in items)
        {
            if (pair.Key.ActionRequired == "Smithing")
            {
                return pair.Key;
            }
        }
        return null;
    }
    public bool HasArrows()
    {
        foreach (KeyValuePair<GameItem, int> pair in items)
        {
            if (pair.Key.IsArrow)
            {
                return true;
            }
        }
        return false;
    }
    public GameItem GetStrongestArrows()
    {
        GameItem strongestArrow = null;
        foreach (KeyValuePair<GameItem, int> pair in items)
        {
            if (pair.Key.IsArrow && strongestArrow == null)
            {
                strongestArrow = pair.Key;
            }
            else if(pair.Key.IsArrow && strongestArrow.Damage < pair.Key.Damage)
            {
                strongestArrow = pair.Key;
            }
        }
        return strongestArrow;
    }
    public bool AddItem(GameItem item)
    {
        //If the added item is null, the inventory is full and the item is not stackable, 
        //or the inventory is full and the item is stackable, but not already in the inventory
        if (item == null ||
          (totalItems == maxSize && item.IsStackable == false) ||
          (totalItems == maxSize && item.IsStackable == true && HasItem(item) == false))
        {
            return false;
        }
        if (items.TryGetValue(item, out int amount))
        {
            items[item] = Math.Min(amount + 1, maxValue);
            if (item.IsStackable == false)
            {
                totalItems++;
                totalItems.Clamp(0, maxSize);
            }
        }
        else
        {
            items[item] = 1;
            totalItems++;
            totalItems.Clamp(0, maxSize);
        }
        return true;
    }
    public int GetAmountOfItem(GameItem item)
    {
        if (items.TryGetValue(item, out int amount))
        {
            return amount;
        }
        return 0;
    }

    public bool AddItems(Dictionary<GameItem, int> itemsToAdd)
    {
        if (itemsToAdd == null)
        {
            return false;
        }
        foreach (KeyValuePair<GameItem, int> itemToAdd in itemsToAdd)
        {
            for (int i = 0; i < itemToAdd.Value; i++)
            {
                if (AddItem(itemToAdd.Key) == false)
                {
                    return false;
                }
            }
        }
        return true;
    }
    public void LoadItems(Dictionary<GameItem, int> itemsToAdd)
    {

        if (itemsToAdd == null)
        {
            return;
        }
        foreach (KeyValuePair<GameItem, int> itemToAdd in itemsToAdd)
        {
            items.Add(itemToAdd.Key, itemToAdd.Value);
            if (itemToAdd.Key.IsStackable)
            {
                totalItems++;
            }
            else
            {
                totalItems += itemToAdd.Value;
            }
        }
    }
    public bool AddMultipleOfItem(GameItem item, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (AddItem(item) == false)
            {
                return false;
            }
        }

        return true;
    }
    public void AddMultipleOfItemUnlimited(GameItem item, int amount)
    {
        if (items.ContainsKey(item))
        {
            items[item] = amount + items[item];
        }
        else
        {
            items[item] = amount;
        }
    }
    public void AddOneOfMultipleItemsUnlimited(List<GameItem> itemList)
    {
        foreach(GameItem item in itemList)
        {
            if (items.ContainsKey(item))
            {
                items[item] = 1 + items[item];
            }
            else
            {
                items[item] = 1;
            }
        }
    }
    public bool AddMultiplesOfItems(List<GameItem> itemList)
    {
        foreach(GameItem item in itemList)
        {
            AddMultipleOfItem(item, Math.Max(1, item.MadeOnCreation));

        }
        return true;
    }
    public bool AddOneOfMultipleItems(List<GameItem> itemList)
    {
        foreach (GameItem item in itemList)
        {
            if (AddItem(item) == false)
            {
                return false;
            }
        }
        return true;
    }
    public bool RemoveOneOfItem(GameItem item)
    {
        if (items.TryGetValue(item, out int amount))
        {
            if (amount >= 1)
            {
                items[item] = amount - 1;
                if (item.IsStackable == false)
                {
                    totalItems--;
                    totalItems.Clamp(0, maxSize);
                }
                amount -= 1;
            }
            if (amount == 0)
            {
                items.Remove(item);             
            }
            return true;
        }
        return false;
    }
    public int RemoveItems(GameItem item, int amount)
    {
        if (items.TryGetValue(item, out int currentAmount))
        {
            if (currentAmount >= amount)
            {
                items[item] = currentAmount - amount;
                if (item.IsStackable == false)
                {
                    totalItems -= amount;
                    totalItems.Clamp(0, maxSize);
                }
                if (items[item] == 0)
                {
                    items.Remove(item);
                    if (item.IsStackable)
                    {
                        totalItems -= 1;
                        totalItems.Clamp(0, maxSize);
                    }
                }
                return amount;
            }

        }
        return 0;
    }
    public void RemoveItemByID(int id)
    {
        KeyValuePair<GameItem, int> pairToRemove;
        foreach (KeyValuePair<GameItem, int> pair in items)
        {
            if (pair.Key.Id == id)
            {
                pairToRemove = pair;
            }
        }
        RemoveOneOfItem(pairToRemove.Key);
    }
    public void RemoveItemsByID(int[] ids)
    {
        foreach (int i in ids)
        {
            RemoveItemByID(i);
        }
    }
    public bool ActionIsEnabled(string skillName)
    {
        Console.WriteLine("Skill: " + skillName);
        if (skillName.Contains("Gather") || skillName.Length < 1)
        {
            return true;
        }
        foreach (GameItem i in items.Keys)
        {
            if (i.ActionsEnabled != null && i.ActionsEnabled.Contains(skillName))
            {
                return true;
            }
        }
        return false;
    }
    public void EmptyInventory()
    {
        totalItems = 0;
        items.Clear();
    }
    public void EmptyInventoryOfUnequippedItems()
    {
        Dictionary<GameItem, int> equippedItems = new Dictionary<GameItem, int>();
        foreach (KeyValuePair<GameItem, int> item in items)
        {
            if (item.Key.IsEquipped)
            {
                equippedItems[item.Key] = item.Value;
            }
        }
        EmptyInventory();
        AddItems(equippedItems);
    }
    public override string ToString()
    {
        string returnString = "";
        foreach (KeyValuePair<GameItem, int> pair in items)
        {
            returnString += pair.Key.Id + "-" + pair.Value + "/";
        }
        return returnString;
    }
    public string GetFreeSpacesString()
    {
        return "Free Spaces: " + GetAvailableSpaces() + "/" + maxSize;
    }

}
