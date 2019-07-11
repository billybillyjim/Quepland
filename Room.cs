using System;

public class Room
{
    public string Name { get; set; }
    public string URL { get; set; }
    public int ID { get; set; }
    public int ConstructionLevelRequired { get; set; }
    public string Description { get; set; }
    public int MinimumPlankLevel { get; set; }
    public int PlanksRequired { get; set; }
    public int BarsRequired { get; set; }
    public int MinimumBarLevel { get; set; }
    public int[][] OtherItemCosts { get; set; }
    public int ExperienceGained { get; set; }
}
