using System;

public class Bank
{
    private Inventory inventory;
	public Bank()
	{
        inventory = new Inventory(int.MaxValue);
	}
    public Inventory GetInventory()
    {
        return inventory;
    }
}
