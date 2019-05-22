using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Components;

public class NPCManager
{
    private List<NPC> NPCs = new List<NPC>();

    public async void LoadNPCs(HttpClient Http)
    {
        NPC[] npcArray = await Http.GetJsonAsync<NPC[]>("data/NPCs.json");
        NPCs.AddRange(npcArray);
    }

    public NPC GetNPCByID(int id)
    {
        return NPCs[id];
    }
}
