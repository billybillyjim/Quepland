using System.Collections.Generic;

public class House
{
    public List<Room> Rooms { get; }

    public int CurentPlanks { get; set; }
    public int CurrentBars { get; set; }

    public House()
    {
        Rooms = new List<Room>();
    }
}
