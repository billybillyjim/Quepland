using System;
using System.Collections.Generic;

public class TanningSlot
{
    public KeyValuePair<int, int> Item = new KeyValuePair<int, int>();
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TanningSlot()
    {

    }
    public TanningSlot(int itemID, int itemAmount, DateTime startTime, DateTime endTime)
    {
        Item = new KeyValuePair<int, int>(itemID, itemAmount);
        StartTime = startTime;
        EndTime = endTime;
    }

    public string GetString()
    {
        return Item.Key.ToString() + "," + Item.Value.ToString() + "," + EndTime.ToString();
    }
    public void SetDataFromString(string data)
    {
        string[] lines = data.Split(',');
        int itemID = int.Parse(lines[0]);
        int itemAmount = int.Parse(lines[1]);
        DateTime endTime = DateTime.Parse(lines[2]);
        Item = new KeyValuePair<int, int>(itemID, itemAmount);
        EndTime = endTime;
    }
}
