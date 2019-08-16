using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

public class AreaManager
{
    private List<Area> areas = new List<Area>();
	public AreaManager()
	{
	}
    public async Task LoadAreas(HttpClient Http)
    {
        Area[] areasArray = await Http.GetJsonAsync<Area[]>("data/areas.json");
        areas = areasArray.ToList();
        areas[0].IsUnlocked = true;
    }
    public string SaveAreas()
    {
        string saveData = "";
        foreach(Area area in areas)
        {
            saveData += area.ID + "," + area.IsUnlocked + "/";
        }
        return saveData;
    }
    public void LoadSaveData(string data)
    {
        string[] dataLines = data.Split('/');
        foreach(string line in dataLines)
        {
            if(line.Length > 1)
            {
                int areaID = int.Parse(line.Split(',')[0]);
                bool unlocked = bool.Parse(line.Split(',')[1]);
                try
                {
                    GetAreaByID(areaID).IsUnlocked = unlocked;
                }
                catch
                {
                    Console.WriteLine("Failed to load area ID:" + areaID);
                }
            }
        }
        areas[0].IsUnlocked = true;
    }
    public void TestLoadSaveData(string data)
    {
        string[] dataLines = data.Split('/');
        foreach (string line in dataLines)
        {
            if (line.Length > 1)
            {
                int areaID = int.Parse(line.Split(',')[0]);
                bool unlocked = bool.Parse(line.Split(',')[1]);
                try
                {
                    bool test = GetAreaByID(areaID).IsUnlocked;
                }
                catch
                {                   
                    Console.WriteLine("Failed to load area ID:" + areaID);
                }
            }
        }
        
    }
    public Area GetAreaByID(int id)
    {
        return areas[id];
    }
    public Area GetAreaByName(string name)
    {
        if (areas.Count > 0)
        {
            return areas.Find(x => x.Name == name);
        }
        else
        {
            return new Area();
        }
    }
    public Area GetAreaByURL(string url)
    {
        if (areas.Count > 0)
        {
            return areas.Find(x => x.AreaURL == url);
        }
        else
        {
            return new Area();
        }
    }
    public List<Area> GetAreas()
    {
        return areas;
    }
    public Area GetAreaParent(Area area)
    {
        foreach(Area a in areas)
        {         
            foreach(string s in a.Children ?? Enumerable.Empty<string>())
            {
                if (s.Contains(area.AreaURL))
                {
                    return a;
                }
            }
        }
        return null;
    }
    public List<Area> GetAreasWithChildren()
    {
        return areas.Where(x => x.Children != null).ToList();
    }
    public List<Area> GetAreasWithoutChildren()
    {
        List<Area> areaList = new List<Area>();
        foreach(Area area in areas)
        {
            if(area.Children == null || area.Children[0] == null || area.Children[0] == "")
            {
                areaList.Add(area);
            }
        }
        return areaList;
    }

   public void UnlockAllAreas()
    {
        foreach(Area area in areas)
        {
            area.IsUnlocked = true;
        }
    }
}

