using System;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;

public class Player
{
    public Inventory inventory;
    public Bank bank;
    public House house;
    public Follower activeFollower;
    public Pet activePet;
    public bool hasLoadedSkills;
    public int CurrentHP;
    public int MaxHP;
    private List<Skill> skills;
    private List<GameItem> equippedItems = new List<GameItem>();
    private List<string> knownAlchemicRecipes = new List<string>();
    private List<string> shorterRecipes = new List<string>();
    public List<Pet> Pets = new List<Pet>();
    //DEBUG Value!
    private readonly int maxInventorySize = 30;
    private MessageManager messageManager;
    public string LastLevelledSkill;
    public bool LastLevelledSkillLocked;
    
    
    

	public Player()
	{
        inventory = new Inventory(maxInventorySize);
        bank = new Bank();
        skills = new List<Skill>();
        MaxHP = 50;
        house = new House();
    
	}
    public async void LoadSkills(HttpClient Http)
    {
        Skill[] skillArray = await Http.GetJsonAsync<Skill[]>("data/skills.json");
        skills = skillArray.ToList();
        hasLoadedSkills = true;
    }
    private void IncreaseMaxHPBy(int amount)
    {
        MaxHP += amount;
    }
    public void SetSkills(List<Skill> skillList)
    {
        foreach(Skill s in skillList)
        {
            if(skills.Find(x => x.SkillName == s.SkillName) != null)
            {
                skills.Find(x => x.SkillName == s.SkillName).SkillExperience = s.SkillExperience;
                skills.Find(x => x.SkillName == s.SkillName).SetSkillLevel(s.GetSkillLevelUnboosted());
                if(s.SkillName == "Strength")
                {
                    int extraSlots = (s.GetSkillLevelUnboosted() / 10) * 4;
                    //Subtract 1 to make up for starting at level 1.
                    inventory.ResetMaxSize();
                    inventory.IncreaseMaxSizeBy(s.GetSkillLevelUnboosted() + extraSlots - 1);
                }
                else if(s.SkillName == "HP")
                {
                    MaxHP = 50;
                    int extraHP = (s.GetSkillLevelUnboosted() / 5) * 10;
                    IncreaseMaxHPBy((s.GetSkillLevelUnboosted() * 5) + extraHP - 5);
                }
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
        string shortRecipe = "" + metal.ItemName.Substring(0, 3) + "+" + element.ItemName.Substring(0,3) + "+" + location.Name.Substring(0,3) + "+" + result.ItemName.Substring(0,3);
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
            shorterRecipes.Add(recipe);

        }
        knownAlchemicRecipes = knownAlchemicRecipes.OrderBy(x => x).ToList();
    }
    public bool HasPet(Pet pet)
    {
        foreach(Pet p in Pets)
        {
            if(p.Name == pet.Name)
            {
                return true;
            }
        }
        return false;
    }
    public List<string> GetRecipes()
    {
        return knownAlchemicRecipes;
    }
    public List<string> GetShortRecipes()
    {
        return shorterRecipes;
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
    public string GetPetString()
    {
        string petString = "";
        foreach(Pet p in Pets)
        {
            petString += p.GetSaveString();
            petString += (char)16;
        }
        if (activePet != null)
        {
            petString += (char)17 + activePet.Name;
        }
        else
        {
            petString += (char)17 + "None";
        }
        return petString;
    }
    public void LoadPetsFromString(string data)
    {
        string[] lines = data.Split((char)17)[0].Split((char)16);
        foreach(string line in lines)
        {
            if(line.Length > 1)
            {
                Pet newPet = new Pet();
                string[] info = line.Split((char)15)[0].Split((char)14);
                newPet.Name = info[0];
                newPet.Description = info[1];
                newPet.Nickname = info[2];

                newPet.MinLevel = int.Parse(info[3]);

                newPet.Affinity = info[4];

                newPet.Identifier = info[5];
                string skillString = line.Split((char)15)[1];
                
                newPet.SetSkills(Extensions.GetSkillsFromString(skillString));
                if(Pets.Find(x => x.Name == info[0]) == null)
                {
                    Pets.Add(newPet);
                }
            }

        }
        if (data.Split((char)17).Length > 1)
        {
            if(data.Split((char)17)[1] == "None")
            {
                return;
            }
            activePet = Pets.Find(x => x.Name == data.Split((char)17)[1]);
        }
        
        
    }
    public void TestLoadPetsFromString(string data)
    {
        string[] lines = data.Split((char)17)[0].Split((char)16);
        foreach (string line in lines)
        {
            if (line.Length > 1)
            {
                string[] info = new string[10];
                try
                {
                    info = line.Split((char)15)[0].Split((char)14);
                }
                catch
                {
                    messageManager.AddMessage("Saved pet data failed:" + line, "red");
                    Console.WriteLine("Pet Data:Failed to load pet info:" + line);
                }
                try
                {
                    Pet newPet = new Pet();

                    newPet.Name = info[0];
                    newPet.Description = info[1];
                    newPet.Nickname = info[2];

                    newPet.MinLevel = int.Parse(info[3]);

                    newPet.Affinity = info[4];

                    newPet.Identifier = info[5];
                    string skillString = line.Split((char)15)[1];

                    newPet.SetSkills(Extensions.GetSkillsFromString(skillString));
                }
                catch
                {
                    messageManager.AddMessage("Pet data failed on setting:" + line, "red");
                    Console.WriteLine("Pet Data:Failed to set pet info:" + line);
                }

            }

        }
        if (data.Split((char)17).Length > 1)
        {
            try
            {
                if (data.Split((char)17)[1] == "None")
                {
                    return;
                }
            }
            catch
            {
                Console.WriteLine("Assigned Pet:Failed to load data:" + data.Split((char)17)[1]);
            }
        }
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

    public void GainExperience(string skill, long amount)
    {
        if (skills.Find(x => x.SkillName == skill) != null)
        {
            GainExperience(skills.Find(x => x.SkillName == skill), amount);
        }
    }
    public void GainExperience(string skill)
    {
        if(skill == null || skill == "")
        {
            return;
        }
        if (int.TryParse(skill.Split(':')[1], out int amount))
        {
            GainExperience(skills.Find(x => x.SkillName == skill.Split(':')[0]), amount);
        }
    }
    public void GainExperienceFromMultipleItems(string skill, int amount)
    {
        if (skill == null || skill == "")
        {
            return;
        }
        if (int.TryParse(skill.Split(':')[1], out int multi))
        {
            GainExperience(skills.Find(x => x.SkillName == skill.Split(':')[0]), multi* amount);
        }
    }
    public void GainExperience(Skill skill, long amount)
    {
        if (skill == null)
        {
            Console.WriteLine("Player gained " + amount + " experience in unfound skill.");
            return;
        }
        if(amount <= 0)
        {
            return;
        }
        if(LastLevelledSkillLocked == false)
        {
            LastLevelledSkill = skill.SkillName;
        }
        
        if(activePet != null)
        {
            if(activePet.messageManager == null)
            {
                activePet.messageManager = messageManager;
            }
            activePet.GainExperience(skill.SkillName, amount / 10);
            skill.SkillExperience += (long)(amount * GetExperienceGainBonus(skill) * activePet.GetSkillBoost(skill));
        }
        else
        {
            skill.SkillExperience += (long)(amount * GetExperienceGainBonus(skill));
        }
        
        if (skill.SkillExperience >= Extensions.GetExperienceRequired(skill.GetSkillLevelUnboosted()))
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
            GainExperience("Knifesmanship", (int)(damageDealt));
        }
        else if (weapon.ActionRequired.Contains("Sword"))
        {
            GainExperience("Deftness", (int)(damageDealt * 0.5));
            GainExperience("Strength", damageDealt);
            GainExperience("Swordsmanship", (int)(damageDealt));
        }
        else if (weapon.ActionRequired.Contains("Axe"))
        {
            GainExperience("Deftness", (int)(damageDealt * 0.5));
            GainExperience("Strength", damageDealt);
            GainExperience("Axemanship", (int)(damageDealt));
        }
        else if (weapon.ActionRequired.Contains("Hammer"))
        {
            GainExperience("Strength", (int)(damageDealt * 1.5));
            GainExperience("Hammermanship", (int)(damageDealt));
        }
        else if (weapon.ActionRequired.Contains("Archery"))
        {
            if (inventory.HasArrows())
            {
                GainExperience("Archery", (int)(damageDealt * 1.5));
            }
            else
            {
                GainExperience("Strength", (int)(damageDealt * 0.5));
            }
        }
        else if (weapon.ActionRequired.Contains("Fishing"))
        {
            GainExperience("Fishing", (int)(damageDealt * 0.1));
        }
    }
    public void LevelUp(Skill skill)
    {
        skill.SetSkillLevel(skill.GetSkillLevelUnboosted() + 1);
        messageManager.AddMessage("You leveled up! Your " + skill.SkillName + " level is now " + skill.GetSkillLevelUnboosted() + ".");
        if (skill.SkillName == "Strength")
        {
            inventory.IncreaseMaxSizeBy(1);
           
            if (skill.GetSkillLevelUnboosted() % 10 == 0)
            {
                inventory.IncreaseMaxSizeBy(4);
                messageManager.AddMessage("You feel stronger. You can now carry 5 more items in your inventory.");
            }
            else
            {
                messageManager.AddMessage("You feel stronger. You can now carry 1 more item in your inventory.");
            }
        }
        else if(skill.SkillName == "HP")
        {
            IncreaseMaxHPBy(5);
            if(skill.GetSkillLevelUnboosted() % 5 == 0)
            {
                IncreaseMaxHPBy(10);
                messageManager.AddMessage("You feel much healthier. Your maximum HP has increased by 15!");
            }
            else
            {
                messageManager.AddMessage("You feel healthier. Your maximum HP has increased by 5.");
            }
        }
       
        if (skill.SkillExperience >= Extensions.GetExperienceRequired(skill.GetSkillLevelUnboosted()))
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

    public int GetDamageDealt(Monster opponent)
    {
        int str = GetSkill("Strength").GetSkillLevel();
        int deft = GetSkill("Deftness").GetSkillLevel();
        float baseDamage = 1 + (str / 4);      
        float armorReduction = 1 - (float)Extensions.CalculateArmorDamageReduction(opponent);
        int equipmentBonus = (int)(GetEquipmentBonus() * armorReduction);

        if (GetWeapon() != null)
        {
            string action = GetWeapon().ActionRequired;

            if (action.Contains("Knife"))
            {
                baseDamage += deft * 2;
                baseDamage += GetSkill("Knifesmanship").GetSkillLevel();
            }
            else if (action.Contains("Sword"))
            {
                baseDamage += str;
                baseDamage += deft * 2;
                baseDamage += GetSkill("Swordsmanship").GetSkillLevel();
            }
            else if (action.Contains("Axe"))
            {
                baseDamage += str * 2;
                baseDamage += deft / 2f;
                baseDamage += GetSkill("Axemanship").GetSkillLevel() * 2;
            }
            else if (action.Contains("Hammer"))
            {
                baseDamage += str * 3;
                baseDamage += GetSkill("Hammermanship").GetSkillLevel() * 2;
            }
            else if (action.Contains("Archery"))
            {
                baseDamage = 1 + GetSkill("Archery").GetSkillLevel() * 3 + (deft * 2);
                if (inventory.HasArrows())
                {
                    baseDamage += inventory.GetStrongestArrows().Damage;
                }

            }

            baseDamage = CalculateEffectiveness(opponent, action, baseDamage);
                     
            return Math.Max(Extensions.GetGaussianRandomInt(baseDamage + equipmentBonus, baseDamage / 3f), 1);
        }
        baseDamage *= armorReduction;
        return Math.Max(Extensions.GetGaussianRandomInt(baseDamage + equipmentBonus, baseDamage / 3f), 1);
    }
    private float CalculateEffectiveness(Monster opponent, string action, float baseDamage)
    {
        if (opponent.ChangesStances)
        {
            string weakness = opponent.Weakness.Split(' ')[opponent.CurrentStance];
            string strength = opponent.Strength.Split(' ')[opponent.CurrentStance];
            if (weakness.Contains(action))
            {
                baseDamage *= 1.75f;
                baseDamage *= 1 - ((float)Extensions.CalculateArmorDamageReduction(opponent) / 3f);
            }
            else if (strength.Contains(action))
            {
                baseDamage /= 1.75f;
                baseDamage *= 1 - (float)Extensions.CalculateArmorDamageReduction(opponent);
            }
            else
            {
                baseDamage *= 1 - (float)Extensions.CalculateArmorDamageReduction(opponent);
            }
        }
        else
        {
            if (opponent.Weakness.Contains(action))
            {
                baseDamage *= 1.75f;
                baseDamage *= 1 - ((float)Extensions.CalculateArmorDamageReduction(opponent) / 3f);
            }
            else if (opponent.Strength.Contains(action))
            {
                baseDamage /= 1.75f;
                baseDamage *= 1 - (float)Extensions.CalculateArmorDamageReduction(opponent);
            }
            else
            {
                baseDamage *= 1 - (float)Extensions.CalculateArmorDamageReduction(opponent);
            }
        }
        baseDamage -= opponent.Armor;
        return baseDamage;
    }
    public int GetEquipmentBonus()
    {
        int total = 0;
        foreach(GameItem item in equippedItems)
        {
            if(item.ActionRequired == "Archery" && inventory.HasArrows() == false)
            {
                total += item.Damage / 4;
            }
            else
            {
                total += (item.Damage * 4);
            }          
        }
        return total;
    }
    public float GetExperienceGainBonus(string skill)
    {
        return GetExperienceGainBonus(skills.Find(x => x.SkillName == skill));
    }
    public float GetExperienceGainBonus(Skill skill)
    {
        float baseExp = 1;
        if(skill == null)
        {
            return 1;
        }
        foreach(GameItem equipped in equippedItems)
        {
            if(equipped.ActionRequired == skill.SkillName)
            {
                baseExp += equipped.ExperienceGainBonus;
            }
        }
        return baseExp;
    }
    public GameItem GetWeapon()
    {
        return equippedItems.Find(x => x.EquipSlot == "Right Hand");
    }
    public int GetWeaponAttackSpeed()
    {
        if(GetWeapon() != null)
        {
            return GetWeapon().AttackSpeed - GetLevel("Deftness");
        }
        else
        {
            return 1500 - GetLevel("Deftness");
        }
    }
    public float GetGatherSpeed(string skill)
    {
        float totalBonus = 1;
        if(skill == null || skill == "")
        {
            return 1;
        }
        foreach(GameItem item in equippedItems)
        {
            if (item.ActionsEnabled != null && item.ActionsEnabled.Contains(skill))
            {
                if(item.GatherSpeedBonus > 0)
                {
                    totalBonus *= 1 - item.GatherSpeedBonus;
                }             
            }
            else if(item.ActionRequired != null && item.ActionRequired.Contains(skill))
            {
                if (item.GatherSpeedBonus > 0)
                {
                    totalBonus *= 1 - item.GatherSpeedBonus;
                }
            }
        }
        int level = GetLevel(skill);
        if(level < 100)
        {
            totalBonus *= 1 - (level * 0.005f);
        }
        else if(level < 200)
        {
            totalBonus *= 1 - (100 * 0.005f);
            totalBonus *= 1 - (level * 0.002f);
        }
        else if(level < 300)
        {
            totalBonus *= 1 - (100 * 0.005f);
            totalBonus *= 1 - (200 * 0.002f);
            totalBonus *= 1 - (level * 0.001f);
        }
        else
        {
            totalBonus *= 1 - (100 * 0.005f);
            totalBonus *= 1 - (200 * 0.002f);
            totalBonus *= 1 - (300 * 0.001f);
            totalBonus *= 1 - (level * 0.0005f);
        }
        //totalBonus -= GetLevel(skill) * 0.005f;
        totalBonus = Math.Max(totalBonus, 0.01f);
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
            if(item.Key.ActionsEnabled != null && item.Key.ActionsEnabled.Contains(requirement))
            {
                return true;
            }
        }
        return false;
    }
    public int GetTotalLevel()
    {
        int total = 0;
        foreach(Skill s in skills)
        {
            total += s.GetSkillLevelUnboosted();
        }
        return total;
    }
    public List<Pet> GetPetsSorted(int sortStyle)
    {
        List<Pet> sortedPets = new List<Pet>();
        sortedPets.AddRange(Pets);
        if(sortStyle == 0)
        {
            sortedPets = sortedPets.OrderBy(x => x.Name).ToList();
        }
        else if(sortStyle == 1)
        {
            sortedPets = sortedPets.OrderBy(x => x.GetTotalLevels()).Reverse().ToList();          
        }
        else if(sortStyle == 2)
        {
            sortedPets = sortedPets.OrderBy(x => x.Affinity).ToList();
        }
        else if(sortStyle == 3)
        {
            sortedPets = sortedPets.OrderBy(x => x.GetHighestSkillLevel()).Reverse().ToList();
        }
        else if(sortStyle == 4)
        {
            sortedPets = sortedPets.OrderBy(x => x.MinLevel).Reverse().ToList();
        }
        return sortedPets;
    }
}
