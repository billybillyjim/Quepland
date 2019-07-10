using System;
using System.ComponentModel;

public class Area
{
    [DefaultValue("No Name")]
	public string Name { get; set; }
    public string AreaURL { get; set; }
    public int ID { get; set; }
    [DefaultValue("oi oi-question-mark")]
    public string Icon { get; set; }
    [DefaultValue("Whoops. The dev forgot to add a description here.")]
    public string Description { get; set; }
    public string[] Actions { get; set; }
    public bool IsUnlocked { get; set; }
    public string[] UnlockableAreas { get; set; }
    public string AreaToUnlockOnArrival { get; set; }
    public string[] Buildings { get; set; }
    public int[] NPCIDs { get; set; }
    public int[] MonsterIDs { get; set; }
    public int[] HuntingIDs { get; set; }
    public string[] Children { get; set; }
    public string ActionRequiredForAccess { get; set; }
    public string RedirectAreaURL { get; set; }

    [DefaultValue(true)]
    public bool Hidden { get; set; }
    public bool IsMarked { get; set; }
    public bool IsShowingChildren { get; set; }
    public bool IsRegion { get; set; }
    public string Region { get; set; }
    public string AccessibleRegions { get; set; }
}
