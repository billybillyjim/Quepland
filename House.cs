using System;
using System.Collections.Generic;

public class House
{
    private List<Room> rooms = new List<Room>();
    public int curentPlanks;
    public int currentBars;

    public House()
    {

    }
    public void AddRoom(Room room)
    {
        rooms.Add(room);
    }
    public List<Room> GetRooms()
    {
        return rooms;
    }
}
