using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
public class BattleManager
{
    private List<Monster> monsters = new List<Monster>();
    public BattleManager()
	{
	}

    public async void LoadMonsters(HttpClient Http)
    {
        Monster[] monsterArray = await Http.GetJsonAsync<Monster[]>("data/Monsters.json");
        monsters = monsterArray.ToList();
    }
    public List<Monster> GetMonstersForArea(Area area)
    {
        List<Monster> monsterList = new List<Monster>();
        foreach(int i in area.MonsterIDs)
        {
            monsterList.Add(monsters[i]);
        }
        return monsterList;
    }
}
