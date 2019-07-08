using System;
using System.Collections.Generic;

public class BankTab
{
	public string Name { get; set; }
    public List<int> itemIDs = new List<int>();

    public BankTab(string name)
    {
        Name = name;
    }
}
