using System;
using System.ComponentModel;
public class NPC
{
	public bool IsInteractable { get; set; }
    public bool HideOnFollow { get; set; }
    public bool HideOnSpeak { get; set; }
    public string Name { get; set; }
    public string SpokenText { get; set; }
    public int[] UnlockFollowerIDs { get; set; }
    public int[] UnlockAreaIDs { get; set; }
    public int[] UnlockBuildingIDs { get; set; }
    public int[] UnlockNPCIDs { get; set; }
    public int[] PurchaseItemIDs { get; set; }
    public int[][] ItemPaymentIDs { get; set; }
    public int[] SpecialCosts { get; set; }
    public bool HasSpecialCosts { get; set; }
    [DefaultValue(1.0d)]
    public double PriceDiscount { get; set; }
    public int UnlockAreaCost { get; set; }
    public int ID { get; set; }
}
