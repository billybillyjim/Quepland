using System;

public class Follower
{
	public string Name { get; set; }
    public string Description { get; set; }
    public string AutoCollectMessage { get; set; }
    public int Cost { get; set; }
    public int CostItemID { get; set; }
    public int id { get; set; }
    public string[] RequiredLevels { get; set; }
    public string AutoCollectSkill { get; set; }
    public int AutoCollectLevel { get; set; }
    public int AutoCollectSpeed { get; set; }
    public bool IsUnlocked { get; set; }
    public int MaxCarry { get; set; }

    public override string ToString()
    {
        return "Current Follower:" + Name;
    }
}
