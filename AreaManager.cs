using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Components;

public class AreaManager
{
    private List<Area> areas = new List<Area>();
	public AreaManager()
	{
	}
    public async void LoadAreas(HttpClient Http)
    {
        Area[] areasArray = await Http.GetJsonAsync<Area[]>("data/areas.json");
        areas = areasArray.ToList();
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
                GetAreaByID(areaID).IsUnlocked = unlocked;
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
        Console.WriteLine("Area " + area.Name + " has no parent.");
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
    public RenderFragment BuildDisabledButtons(bool playerHasLevels, bool actionIsEnabled, string action)
    {
        return builder =>
        {
            builder.OpenElement(0, "span");
            builder.AddAttribute(0, "class", "d-inline-block");
            builder.AddAttribute(0, "tabindex", "0");
            builder.AddAttribute(0, "data-toggle", "tooltip");

            if (playerHasLevels)
            {
                builder.AddAttribute(0, "title", "you lack the tool to do this.");
            }
            else if (actionIsEnabled)
            {
                builder.AddAttribute(0, "title", "you lack the levels to do this.");
            }
            else
            {
                builder.AddAttribute(0, "title", "you lack the tool and levels to do this.");
            }
            builder.OpenElement(1, "button");
            builder.AddAttribute(1, "style", "margin-bottom:10px");
            builder.AddAttribute(1, "class", "btn btn-primary");
            builder.AddAttribute(1, "disabled", true);
            builder.AddContent(1, action);
            builder.CloseElement();
            builder.CloseElement();
        };
    }
   
}

