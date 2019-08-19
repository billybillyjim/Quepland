using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

public class NPCManager
{
    private List<NPC> NPCs = new List<NPC>();

    public async Task LoadNPCs(HttpClient Http)
    {
        NPC[] npcArray = await Http.GetJsonAsync<NPC[]>("data/NPCs.json");
        NPCs.AddRange(npcArray);
    }

    public NPC GetNPCByID(int id)
    {
        return NPCs[id];
    }
    public string GetNPCData()
    {
        string data = "";
        foreach(NPC npc in NPCs)
        {
            data += "" + npc.IsInteractable + ",";
        }
        data = data.Substring(0, data.Length - 1);
        return data;
    }
    public void LoadNPCData(string data)
    {
        if(data == null || data.Length == 0)
        {
            return;
        }
        string[] lines = data.Split(',');
        int iterator = 0;
        foreach(string s in lines)
        {
            try
            {
                NPCs[iterator].IsInteractable = bool.Parse(s);
            }
            catch
            {
                Console.WriteLine("Failed to parse bool for NPC " + iterator + " with value " + s);
            }
            iterator++;
        }
    }
    public void TestLoadNPCData(string data)
    {
        if (data == null || data.Length == 0)
        {
            Console.WriteLine("NPC data was empty.");
            return;
        }
        string[] lines = data.Split(',');
        int iterator = 0;
        foreach (string s in lines)
        {
            try
            {
                bool.Parse(s);
            }
            catch
            {
                Console.WriteLine("Failed to parse bool for NPC " + iterator + " with value " + s);
            }
            iterator++;
        }
    }
    public List<NPC> GetNPCs()
    {
        return NPCs;
    }
}
