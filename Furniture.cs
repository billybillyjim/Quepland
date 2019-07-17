using System;
using System.Collections.Generic;

public class Furniture
{
	public string Name { get; set; }
    public int ID { get; set; }
    public string Description { get; set; }
    public int ConstructionLevelRequired { get; set; }
    public int MinimumPlankLevel { get; set; }
    public int PlanksRequired { get; set; }
    public int BarsRequired { get; set; }
    public int MinimumBarLevel { get; set; }
    public int[][] OtherItemCosts { get; set; }
    public int ExperienceGained { get; set; }
    public int[] UpgradeIDs { get; set; }
    public List<GameItem> LoadedItems = new List<GameItem>();
    public GameItem WithdrawItem { get; set; }
    public TimeSpan WithdrawEvery { get; set; }
    public int WithdrawAmount { get; set; }

}
