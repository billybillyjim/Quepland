using System;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

public class FollowerManager
{
    private List<Follower> followers = new List<Follower>();
	public FollowerManager()
	{
	}
    public async Task LoadFollowers(HttpClient Http)
    {
        Follower[] followerArray = await Http.GetJsonAsync<Follower[]>("data/followers.json");
        followers.AddRange(followerArray);
    }
    public List<Follower> GetFollowers()
    {
        return followers;
    }
    public Follower GetFollowerByID(int id)
    {
        return followers[id];
    }
    public int GetNumberOfUnlockedFollowers()
    {
        int amount = 0;
        foreach(Follower f in followers)
        {
            if (f.IsUnlocked)
            {
                amount++;
            }
        }
        return amount;
    }
    public override string ToString()
    {
        string returnString = "";
        foreach(Follower follower in followers)
        {
            returnString += follower.id + "," + follower.IsUnlocked + "/";
        }
        returnString = returnString.Remove(returnString.Length - 1);
        return returnString;
    }
    public void LoadSaveData(string data)
    {
        if(data == null || data.Length < 1)
        {
            return;
        }
        string[] dataArray = data.Split('/');
        int i = 0;
        foreach(string line in dataArray)
        {
            followers[i].IsUnlocked = bool.Parse(line.Split(',')[1]);
            i++;
        }
    }
}
