using System;
using System.Collections.Generic;
using System.Linq;

public class Inventory
{
    private Dictionary<GameItem, int> items;
    private List<KeyValuePair<GameItem, int>> sortedItems = new List<KeyValuePair<GameItem, int>>();
    private int currentSortStyle = 0;
    private int maxSize;
    private readonly int maxValue = int.MaxValue - 1000000;
    private int totalItems;
    public Inventory(int max)
    {
        items = new Dictionary<GameItem, int>();
        maxSize = max;
    }
    public void IncreaseMaxSizeBy(int increase)
    {
        maxSize += increase;
    }
    public void ResetMaxSize()
    {
        maxSize = 30;
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
        else if (sortedBy == 4)
        {
            sortedItems = sortedItems.OrderBy(x => x.Key.Value).Reverse().ToList();
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
    public GameItem GetAnyOre()
    {
        foreach (KeyValuePair<GameItem, int> pair in items)
        {
            if (pair.Key.ActionRequired == "Mining")
            {
                return pair.Key;
            }
        }
        return null;
    }
    public bool HasBarIngredients(GameItem bar)
    {
            foreach (int i in bar.IngredientIDs)
            {
                if (items.Keys.FirstOrDefault(x => x.Id == i) == null)
                {
                    return false;
                }
            }
        

        return true;
    }
    public int GetMaxBarIngredients(GameItem bar)
    {
        int max = 0;
        foreach (KeyValuePair<GameItem, int> pair in items)
        {
            foreach (int i in bar.IngredientIDs)
            {
                if (pair.Key.Id == i)
                {
                    max = Math.Max(max, pair.Value);
                }
            }
        }
        return max;
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
            UpdateItemCount();
            return false;
        }
        if (items.TryGetValue(item, out int amount))
        {
            items[item] = Math.Min(amount + 1, maxValue);
        }
        else
        {
            items[item] = 1;
        }
        UpdateItemCount();
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
            AddMultipleOfItem(itemToAdd.Key, itemToAdd.Value);
        }
        UpdateItemCount();
        return true;
    }
    public void LoadItems(Dictionary<GameItem, int> itemsToAdd)
    {
        if (itemsToAdd == null || itemsToAdd.Count == 0)
        {
            return;
        }
        foreach (KeyValuePair<GameItem, int> itemToAdd in itemsToAdd)
        {
            if(items.TryGetValue(itemToAdd.Key, out _))
            {
                if(itemToAdd.Value > 0)
                {
                    items[itemToAdd.Key] = itemToAdd.Value;
                }            
            }
            else
            {
                items.Add(itemToAdd.Key, itemToAdd.Value);
            }

        }
        UpdateItemCount();
    }
    public bool AddMultipleOfItem(GameItem item, int amount)
    {
        if(totalItems == maxSize)
        {
            return false;
        }
        if(amount < 0)
        {
            amount = 0;
        }
        if (item.IsStackable)
        {
            return AddItemStackable(item, amount);
        }
        else
        {
            for (int i = 0; i < amount; i++)
            {
                if (AddItem(item) == false)
                {
                    return false;
                }
            }
        }
        UpdateItemCount();
        return true;
    }
    public bool AddItemStackable(GameItem item, int amount)
    {
        if (totalItems == maxSize)
        {
            return false;
        }
        if (amount < 0)
        {
            amount = 0;
        }
        if(items.TryGetValue(item, out int current))
        {
            if (item.IsStackable)
            {
                items[item] = current + amount;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (item.IsStackable)
            {
                items[item] = amount;
            }
            else
            {
                return false;
            }
        }
        UpdateItemCount();
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
                /*if (item.IsStackable == false)
                {
                    totalItems--;
                    totalItems.Clamp(0, maxSize);
                }*/
                amount -= 1;
            }
            if (amount == 0)
            {
                //totalItems--;
                items.Remove(item);             
            }
            UpdateItemCount();
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
                /*if (item.IsStackable == false)
                {
                    totalItems -= amount;
                    totalItems.Clamp(0, maxSize);
                }*/
                if (items[item] == 0)
                {
                    items.Remove(item);
                    /*if (item.IsStackable)
                    {
                        totalItems -= 1;
                        totalItems.Clamp(0, maxSize);
                    }*/
                }
                UpdateItemCount();
                return amount;
            }

        }
        UpdateItemCount();
        return 0;
    }
    /// <summary>
    /// Slower than RemoveOneOfItem, iterates through all keyvalue pairs in items.
    /// </summary>
    /// <param name="id"></param>
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
        Dictionary<GameItem, int> lockedItems = new Dictionary<GameItem, int>();
        foreach (KeyValuePair<GameItem, int> item in items)
        {
            if (item.Key.IsEquipped)
            {
                equippedItems[item.Key] = item.Value;
            }
            else if (item.Key.IsLocked)
            {
                lockedItems[item.Key] = item.Value;
            }
        }
        EmptyInventory();
        AddItems(equippedItems);
        AddItems(lockedItems);
    }
    private void UpdateItemCount()
    {
        totalItems = 0;
        foreach(KeyValuePair<GameItem, int> item in items)
        {
            if (item.Key.IsStackable)
            {
                totalItems += 1;
            }
            else
            {
                totalItems += item.Value;
            }
        }
    }
    public override string ToString()
    {
        string returnString = "";
        foreach (KeyValuePair<GameItem, int> pair in items)
        {
            int isLocked = 0;
            if (pair.Key.IsLocked)
            {
                isLocked = 1;
            }
            if(pair.Value < 0)
            {
                returnString += pair.Key.Id + "-" + 1 + "-" + isLocked + "/";
            }
            else
            {
                returnString += pair.Key.Id + "-" + pair.Value + "-" + isLocked + "/";
            }
        }
        return returnString;
    }
    public string GetFreeSpacesString()
    {
        return "Free Spaces: " + GetAvailableSpaces() + "/" + maxSize;
    }

}
