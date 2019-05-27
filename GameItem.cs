using System;

public class GameItem
{
    public string ItemName { get; set; }
    public string ItemDescription { get; set; }
    public string GatherString { get; set; }
    public string EquipSlot { get; set; }

    public int Id { get; set; }


    public int Value { get; set; }
    public int GatherSpeed { get; set; }
    public float GatherSpeedBonus { get; set; }
    public int AttackSpeed { get; set; }
    public int Damage { get; set; }
    public int QueplarValue { get; set; }
    public int RequiredLevel { get; set; }
    public int HealAmount { get; set; }
    public int BuffAmount { get; set; }
    public int HealDuration { get; set; }
    public int MadeOnCreation { get; set; }

    public bool IsUsable { get; set; }
    public bool IsEquipable { get; set; }
    public bool IsStackable { get; set; }
    public bool IsUnpackable { get; set; }
    public bool IsSmithable { get; set; }
    public bool IsEquipped { get; set; }
    public bool IsEdible { get; set; }
    public bool IsArrow { get; set; }
    public bool DestroyOnUse { get; set; }

    public int[][] UsableIDs { get; set; }
    public int[] UnpackIDs { get; set; }
    public int[] IngredientIDs { get; set; }
    public string UnpackString { get; set; }

    public string ExperienceGained { get; set; }

    public string ActionsEnabled { get; set; }
    public string ActionRequired { get; set; }
    public string BuffedSkill { get; set; }


    public override string ToString()
    {
        return ItemName;
    }
}
