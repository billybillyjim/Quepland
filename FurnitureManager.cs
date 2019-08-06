using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

public class FurnitureManager
{
    private List<Furniture> Furniture = new List<Furniture>();

    public async Task LoadFurniture(HttpClient Http)
    {
        Furniture[] furnitureArray = await Http.GetJsonAsync<Furniture[]>("data/furniture.json");
        Furniture.AddRange(furnitureArray);
    }
    public List<Furniture> GetFurniture()
    {
        return Furniture;
    }
    public Furniture GetFurnitureByID(int id)
    {
        return Furniture[id];
    }
    public Furniture GetFurnitureByName(string name)
    {
        return Furniture.Find(x => x.Name == name);
    }
}
