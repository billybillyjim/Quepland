using System;
using System.Collections.Generic;

public class Dojo
{
	public string Name { get; set; }
    public int ID { get; set; }
    public string Description { get; set; }
    public List<int> OpponentIDs { get; set; }
    public int itemEarned { get; set; }
    public int amountEarned { get; set; }
    public DateTime LastWonTime { get; set; }
}
