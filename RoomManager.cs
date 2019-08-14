using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

public class RoomManager
{
    public List<Room> Rooms { get; }

    public RoomManager()
    {
        Rooms = new List<Room>();
    }

    public async Task LoadRooms(HttpClient Http)
    {
        Room[] roomArray = await Http.GetJsonAsync<Room[]>("data/rooms.json");
        Rooms.AddRange(roomArray);
    }

    public Room GetByID(int id) => Rooms[id];
    public Room GetByName(string name) => Rooms.Find(x => x.Name == name);
    public Room GetByURL(string url) => Rooms.Find(x => x.URL == url);
}
