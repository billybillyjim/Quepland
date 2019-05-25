﻿using System;

public class NPC
{
	public bool IsInteractable { get; set; }
    public string Name { get; set; }
    public string SpokenText { get; set; }
    public int[] UnlockFollowerIDs { get; set; }
    public int[] UnlockAreaIDs { get; set; }
    public int[] UnlockBuildingIDs { get; set; }
    public int[] UnlockNPCIDs { get; set; }
    public int[] PurchaseItemIDs { get; set; }
}