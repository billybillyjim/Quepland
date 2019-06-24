using System;
using System.Collections.Generic;

public class Building
{
    public string Name { get; set; }
    public string ButtonText { get; set; }
    public int ID { get; set; }
    public string URL { get; set; }
    public bool IsUnlocked { get; set; }
    public int[] items { get; set; }
    public string Description { get; set; }
    public int[] NPCIDs { get; set; }
    public bool IsShop { get; set; }
    public bool IsInn { get; set; }
    public bool IsSushiTrainingBuilding { get; set; }
    public bool IsTannery { get; set; }
    public int TannerySlots { get; set; }
    public int MaxTannerySlots { get; set; }
    public int Salt { get; set; }
    public List<TanningSlot> TanneryItems = new List<TanningSlot>();
    public string[] UnlockableAreas { get; set; }
    public string[] Buildings { get; set; }
    public bool HasAlchemyStation { get; set; }
    public int QueplarValue { get; set; }
    public bool IsExpensive { get; set; }

}
