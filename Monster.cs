using System;

public class Monster
{
	public string Name { get; set; }
    public int ID { get; set; }
    public int Level { get; set; }
    public int ExperienceGained { get; set; }
    public int HP { get; set; }
    public int CurrentHP { get; set; }
    public int Armor { get; set; }
    public string Weakness { get; set; }
    public string Strength { get; set; }
    public int Damage { get; set; }
    public int AttackSpeed { get; set; }
    public int[] AlwaysDrops { get; set; }
    public int[][] DropTable { get; set; }
    /// <summary>
    /// The status effect this monster can inflict. Not to be confused with CurrentStatusEffect, which is the effect the monster has been inflicted with.
    /// </summary>
    public string StatusEffect { get; set; }
    public string CurrentStatusEffect { get; set; }
    public int StatusEffectTimeLeft { get; set; }
    public string ImmuneTo { get; set; }
}
