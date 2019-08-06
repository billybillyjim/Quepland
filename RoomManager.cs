using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

public class RoomManager
{
    private List<Room> Rooms = new List<Room>();

    public async Task LoadRooms(HttpClient Http)
    {
        Room[] roomArray = await Http.GetJsonAsync<Room[]>("data/rooms.json");
        Rooms.AddRange(roomArray);
    }
    public List<Room> GetRooms()
    {
        return Rooms;
    }
    public Room GetRoomByID(int id)
    {
        return Rooms[id];
    }
    public Room GetRoomByName(string name)
    {
        return Rooms.Find(x => x.Name == name);
    }
    public Room GetRoomByURL(string url)
    {
        return Rooms.Find(x => x.URL == url);
    }
}
