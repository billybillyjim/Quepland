using System;
using System.Collections.Generic;
using System.Linq;

public class Bank
{
    private Inventory inventory;
    public List<BankTab> tabs = new List<BankTab>();
    public BankTab activeTab;
    
	public Bank()
	{
        inventory = new Inventory(int.MaxValue);
        tabs.Add(new BankTab("All"));
        SetActiveTab("All");
	}
    public Inventory GetInventory()
    {
        return inventory;
    }
    public void SetActiveTab(string tabName)
    {
        activeTab = tabs.Find(x => x.Name == tabName);
        if(activeTab == null)
        {
            activeTab = tabs[0];
        }
    }
    public void AddItemsToCurrentTab(List<int> itemIDs)
    {
        activeTab.itemIDs.AddRange(itemIDs);
    }
    public void AddTab(string tabName, List<int> ids)
    {
        BankTab newTab = new BankTab(tabName);
        newTab.itemIDs.AddRange(ids);
        tabs.Add(newTab);
    }
    public string GetTabsString()
    {
        string returnString = "";
        foreach(BankTab tab in tabs.Skip(1))
        {
            returnString += tab.Name + ",";
            foreach(int id in tab.itemIDs)
            {
                returnString += id + ",";
            }
            returnString = returnString.Substring(0, returnString.Length - 1);
            returnString += (char)14;
        }
        return returnString;
    }
    public void LoadTabsFromString(string data)
    {
        string[] lines = data.Split((char)14);
        foreach (string line in lines)
        {
            if (line.Length > 0)
            {
                string[] lineData = line.Split(',');

                BankTab newTab = new BankTab(lineData[0]);
                List<int> ids = new List<int>();
                foreach (string i in lineData.Skip(1))
                {
                    ids.Add(int.Parse(i));
                }
                newTab.itemIDs = ids;
                tabs.Add(newTab);
            }
        }

    }
    public bool NameIsValid(string name)
    {
        foreach(BankTab t in tabs)
        {
            if(t.Name == name || t.Name.Contains("#"))
            {
                return false;
            }
        }
        return true;
    }
}
