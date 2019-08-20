using System.Collections.Generic;

public class BankTab
{
    public string Name { get; set; }
    public List<int> ItemIDs { get; }

    public BankTab(string name)
    {
	    Name = name;
        ItemIDs = new List<int>();
    }
}
