using System;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;

public class Player
{
    public Inventory inventory;
    public Bank bank;
    public Follower activeFollower;
    public bool hasLoadedSkills;
    public int CurrentHP;
    private List<Skill> skills;
    private List<GameItem> equippedItems = new List<GameItem>();
    private List<string> knownAlchemicRecipes = new List<string>();
    private readonly int maxInventorySize = 30;
    private MessageManager messageManager;

	public Player()
	{
        inventory = new Inventory(maxInventorySize);
        bank = new Bank();
        skills = new List<Skill>();
	}
    public async void LoadSkills(HttpClient Http)
    {
        Console.WriteLine("Change");
        Skill[] skillArray = await Http.GetJsonAsync<Skill[]>("data/skills.json");
        skills = skillArray.ToList();
        hasLoadedSkills = true;
    }
    public void SetSkills(List<Skill> skillList)
    {
        foreach(Skill s in skillList)
        {
            if(skills.Find(x => x.SkillName == s.SkillName) != null)
            {
                skills.Find(x => x.SkillName == s.SkillName).SkillExperience = s.SkillExperience;
                skills.Find(x => x.SkillName == s.SkillName).SetSkillLevel(s.GetSkillLevel());
            }
        }

    }
    public void SetMessageManager(MessageManager m)
    {
        messageManager = m;
    }
    public void LearnNewAlchemyRecipe(GameItem metal, GameItem element, Building location, GameItem result)
    {
        string recipe = "" + metal.ItemName + " + " + element.ItemName + " in " + location.Name + " = " + result.ItemName;
        bool alreadyKnown = false;
        foreach(string r in knownAlchemicRecipes)
        {
            if(r == recipe)
            {
                alreadyKnown = true;
            }
        }
        if(alreadyKnown == false)
        {
            knownAlchemicRecipes.Add(recipe);
        }
        knownAlchemicRecipes = knownAlchemicRecipes.OrderBy(x => x).ToList();
    }
    public List<string> GetRecipes()
    {
        return knownAlchemicRecipes;
    }
    public void LoadRecipes(List<string> recipes)
    {
        if(recipes != null)
        {
            knownAlchemicRecipes = recipes;
        }       
    }
    public void EquipItem(GameItem item)
    {
        UnequipItem(equippedItems.Find(x => x.EquipSlot == item.EquipSlot));
        equippedItems.Add(item);
        item.IsEquipped = true;
    }
    public void EquipItems(List<int> ids)
    {
        foreach (KeyValuePair<GameItem, int> pair in inventory.GetItems())
        {
            foreach (int i in ids)
            {
                if (pair.Key.Id == i)
                {
                    EquipItem(pair.Key);
                }
            }
        }
    }
    public void UnequipItem(GameItem item)
    {
        if (item != null)
        {
            item.IsEquipped = false;
            equippedItems.Remove(item);
        }
    }
    public List<Skill> GetSkills()
    {
        return skills;
    }
    public Skill GetSkill(string skillName)
    {
        foreach(Skill skill in skills)
        {
            if(skill.SkillName == skillName)
            {
                return skill;
            }
        }
        return null;
    }
    public string GetSkillString()
    {
        string skillString = "";
        foreach(Skill skill in skills)
        {
            skillString += skill.SkillName + "," + skill.SkillExperience + "," + skill.GetSkillLevelUnboosted() + "/";
        }
        skillString = skillString.Remove(skillString.Length - 1);
        return skillString;
    }
    public int GetLevel(string skillName)
    {
        foreach(Skill skill in skills)
        {
            if(skill.SkillName == skillName)
            {
                return skill.GetSkillLevel();
            }
        }
        return 0;
    }
    public void GainExperience(string skill, int amount)
    {
        if (skills.Find(x => x.SkillName == skill) != null)
        {
            GainExperience(skills.Find(x => x.SkillName == skill), amount);
        }
    }
    public void GainExperience(string skill)
    {
        if (int.TryParse(skill.Split(':')[1], out int amount))
        {
            GainExperience(skills.Find(x => x.SkillName == skill.Split(':')[0]), amount);
        }
    }
    public void GainExperience(Skill skill, int amount)
    {
        skill.SkillExperience += amount;
        if (skill.SkillExperience >= Extensions.GetExperienceRequired(skill.GetSkillLevel()))
        {
            LevelUp(skill);
        }
    }
    public void GainExperienceFromWeapon(GameItem weapon, int damageDealt)
    {
        if (weapon.ActionRequired == null)
        {
            return;
        }
        if (weapon.ActionRequired.Contains("Knife"))
        {
            GainExperience("Deftness", (int)(damageDealt * 1.5));
        }
        else if (weapon.ActionRequired.Contains("Sword"))
        {
            GainExperience("Deftness", (int)(damageDealt * 0.5));
            GainExperience("Strength", damageDealt);
        }
        else if (weapon.ActionRequired.Contains("Axe"))
        {
            GainExperience("Deftness", (int)(damageDealt * 0.5));
            GainExperience("Strength", damageDealt);
        }
        else if (weapon.ActionRequired.Contains("Hammer"))
        {
            GainExperience("Strength", (int)(damageDealt * 1.5));
        }
    }
    public void LevelUp(Skill skill)
    {
        skill.SetSkillLevel(skill.GetSkillLevel() + 1);
        messageManager.AddMessage("You leveled up! Your " + skill.SkillName + " level is now " + skill.GetSkillLevel() + ".");
        if (skill.SkillExperience >= Extensions.GetExperienceRequired(skill.GetSkillLevel()))
        {         
            LevelUp(skill);
            
        }
    }
    /// <summary>
    /// Returns true if the player has the required skill level. Use HasRequiredLevels for multiple skills.
    /// </summary>
    /// <param name="skill"></param>
    /// <returns></returns>
    public bool HasRequiredLevel(GameItem item)
    {
        if(item.RequiredLevel == 0)
        {
            return true;
        }
        Skill skillToCheck = skills.Find(x => x.SkillName == item.ActionRequired);
        if(skillToCheck != null)
        {
            return skillToCheck.GetSkillLevel() >= item.RequiredLevel;
        }
        Console.WriteLine("Skill " + item.ActionRequired + " was not found in player's list of skills.");
        return false;
    }
    public bool HasLevelForRoadblock(string skill)
    {
        if (skill == null || skill.Length < 1)
        {
            return true;
        }
        string skillType = skill.Split(':')[0];
        int skillLevel = int.Parse(skill.Split(':')[1]);
        Skill skillToCheck = skills.Find(x => x.SkillName == skillType);
        if (skillToCheck != null)
        {
            return skillToCheck.GetSkillLevel() >= skillLevel;
        }
        Console.WriteLine("Skill " + skillType + " was not found in player's list of skills.");
        return false;
    }
    public bool HasIngredients(int[] ingredientIDs)
    {
        foreach(int ingredient in ingredientIDs)
        {
            if(inventory.HasItem(ingredient) == false)
            {
                return false;
            }
        }
        return true;
    }
    public int GetDamageDealt()
    {
        int str = GetSkill("Strength").GetSkillLevel();
        int deft = GetSkill("Deftness").GetSkillLevel();
        int baseDamage = 1 + str / 2;

        if (GetWeapon() != null)
        {
            string action = GetWeapon().ActionRequired;

            if (action.Contains("Knife"))
            {
                baseDamage += deft * 4;
            }
            else if (action.Contains("Sword"))
            {
                baseDamage += str * 2;
                baseDamage += deft * 2;
            }
            else if (action.Contains("Axe"))
            {
                baseDamage += str * 3;
                baseDamage += deft;
            }
            else if (action.Contains("Hammer"))
            {
                baseDamage += str * 4;
            }
        }
        int equipmentBonus = GetEquipmentBonus();
        return Math.Max(Extensions.GetGaussianRandomInt(baseDamage + equipmentBonus, equipmentBonus), 1);
    }
    private int GetEquipmentBonus()
    {
        int total = 0;
        foreach(GameItem item in equippedItems)
        {
            total += item.Damage;
        }
        return total;
    }
    public GameItem GetWeapon()
    {
        return equippedItems.Find(x => x.EquipSlot == "Right Hand");
    }
    public int GetWeaponAttackSpeed()
    {
        if(GetWeapon() != null)
        {
            return GetWeapon().AttackSpeed;
        }
        else
        {
            return 1500;
        }
    }
    public float GetGatherSpeed(string skill)
    {
        float totalBonus = 1;
        foreach(GameItem item in equippedItems)
        {
            if (item.ActionsEnabled.Contains(skill))
            {
                totalBonus -= item.GatherSpeedBonus;
            }
        }
        totalBonus = Math.Max(totalBonus, 0.1f);
        return totalBonus;
    }
    public bool HasItemToAccessArea(string requirement)
    {
        if(requirement == null)
        {
            return true;
        }
        foreach(KeyValuePair<GameItem, int> item in inventory.GetItems())
        {
            if(item.Key.ActionsEnabled == requirement)
            {
                return true;
            }
        }
        return false;
    }
}
