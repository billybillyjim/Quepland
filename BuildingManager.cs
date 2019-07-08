using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

public class BuildingManager
{
    private List<Building> buildings = new List<Building>();
	public BuildingManager()
	{
	}
    public async Task LoadBuildings(HttpClient Http)
    {
        Building[] buildingArray = await Http.GetJsonAsync<Building[]>("data/buildings.json");
        buildings = buildingArray.ToList();
    }
    public Building GetBuildingByURL(string url)
    {
        return buildings.Find(x => x.URL == url);
    }
    public Building GetBuildingByID(int id)
    {
        return buildings[id];
    }
    public List<Building> GetBuildings()
    {
        return buildings;
    }
}
