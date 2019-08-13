using System;
using System.Collections.Generic;
using System.Linq;

public class Pet
{
	public string Name { get; set; }
    public string Description { get; set; }
    public string Nickname { get; set; }
    public int Cost { get; set; }
    public int MinLevel { get; set; }
    public string Affinity { get; set; }
    public string Identifier { get; set; }
    public MessageManager messageManager;
    public List<Skill> skills = new List<Skill>();

    public string GetSaveString()
    {
        string data = "";
        data += Name + (char)14;
        data += Description + (char)14;
        data += Nickname + (char)14;
        data += MinLevel.ToString() + (char)14;
        data += Affinity + (char)14;
        data += Identifier + (char)14;
        string skillString = "";
        foreach (Skill skill in skills)
        {
            skillString += skill.SkillName + "," + skill.SkillExperience + "," + skill.GetSkillLevelUnboosted() + "," + skill.IsBlocked.ToString() +  "/";
        }
        skillString = skillString.Remove(skillString.Length - 1);
        return data + (char)15 + skillString;
    }
    public void SetSkills(List<Skill> skillList)
    {
        skills = new List<Skill>();
        skills.AddRange(skillList);

    }
    public void SetMessageManager(MessageManager m)
    {
        messageManager = m;
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
        if (skill == null || skill == "")
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
            GainExperience(skills.Find(x => x.SkillName == skill.Split(':')[0]), multi * amount);
        }
    }
    private void GainExperience(Skill skill, long amount)
    {
        if (skill == null || skill.IsBlocked)
        {
            if(skill == null)
            {
                Console.WriteLine("Pet gained " + amount + " experience in unfound skill.");
            }
            
            return;
        }
        if (IsInSpecialty(skill))
        {
            amount = (long)(amount * 1.25d);
        }
        skill.SkillExperience += amount;
        if (skill.SkillExperience >= Extensions.GetExperienceRequired(skill.GetSkillLevelUnboosted()))
        {
            LevelUp(skill);
        }
    }
    public void LevelUp(Skill skill)
    {
        skill.SetSkillLevel(skill.GetSkillLevelUnboosted() + 1);
        messageManager.AddMessage(Name + " has levelled up! (" + skill.SkillName + " " + skill.GetSkillLevelUnboosted() + ")");
        if (skill.SkillExperience >= Extensions.GetExperienceRequired(skill.GetSkillLevelUnboosted()))
        {
            LevelUp(skill);

        }
    }
    public string GetSpecialty()
    {
        float combat = GetCombatLevels() / 8f;
        float gather = GetGatherLevels() / 4f;
        float craft = GetCraftingLevels() / 5f;
        if(combat >= gather && combat >= craft)
        {
            return "Combat";
        }
        else if(gather >= combat && gather >= craft)
        {
            return "Gathering";
        }
        else
        {
            return "Crafting";
        }
    }
    
    public Skill GetHighestSkill()
    {
        if(GetTotalLevels() == skills.Count)
        {
            return null;
        }
        return skills.Aggregate((i, j) => i.GetSkillLevelUnboosted() > j.GetSkillLevelUnboosted() ? i : j);    
    }
    public int GetHighestSkillLevel()
    {
        if (GetTotalLevels() == skills.Count)
        {
            return 1;
        }
        return skills.Aggregate((i, j) => i.GetSkillLevelUnboosted() > j.GetSkillLevelUnboosted() ? i : j).GetSkillLevelUnboosted();
    }
    public float GetSkillBoost(Skill skill)
    {
        float extraBoost = 1;
        if(GetHighestSkill() != null && GetHighestSkill().SkillName == skill.SkillName)
        {
            extraBoost += 0.5f;
        }
        if(Affinity == skill.SkillName)
        {
            extraBoost += 0.8f;
        }
        return Math.Max((skills.Find(x => x.SkillName == skill.SkillName).GetSkillLevel() / 10f), 1) * Math.Max(1, (MinLevel / 15f)) * extraBoost;
    }
    private bool IsInSpecialty(Skill skill)
    {
        string specialty = GetSpecialty();
        if(specialty == "Combat")
        {
            if(skill.SkillName == "HP" ||
                skill.SkillName == "Knifesmanship" ||
                skill.SkillName == "Swordsmanship" ||
                skill.SkillName == "Axemanship" ||
                skill.SkillName == "Hammermanship" ||
                skill.SkillName == "Deftness" ||
                skill.SkillName == "Strength" ||
                skill.SkillName == "Archery")
            {
                return true;
            }
        }
        else if(specialty == "Gathering")
        {
            if (skill.SkillName == "Mining" ||
               skill.SkillName == "Fishing" ||
               skill.SkillName == "Woodcutting" ||
               skill.SkillName == "Hunting")
            {
                return true;
            }
        }
        else if(specialty == "Crafting")
        {
            if (skill.SkillName == "Smithing" ||
               skill.SkillName == "Alchemy" ||
               skill.SkillName == "Woodworking" ||
               skill.SkillName == "Culinary Arts" ||
               skill.SkillName == "Leatherworking")
            {
                return true;
            }
        }
        return false;
    }
    private int GetCombatLevels()
    {
        int total = 0;
        total += skills.Find(x => x.SkillName == "HP").GetSkillLevelUnboosted();
        total += skills.Find(x => x.SkillName == "Knifesmanship").GetSkillLevelUnboosted();
        total += skills.Find(x => x.SkillName == "Swordsmanship").GetSkillLevelUnboosted();
        total += skills.Find(x => x.SkillName == "Axemanship").GetSkillLevelUnboosted();
        total += skills.Find(x => x.SkillName == "Hammermanship").GetSkillLevelUnboosted();
        total += skills.Find(x => x.SkillName == "Deftness").GetSkillLevelUnboosted();
        total += skills.Find(x => x.SkillName == "Strength").GetSkillLevelUnboosted();
        total += skills.Find(x => x.SkillName == "Archery").GetSkillLevelUnboosted();
        return total;
    }
    private int GetGatherLevels()
    {
        int total = 0;
        total += skills.Find(x => x.SkillName == "Mining").GetSkillLevelUnboosted();
        total += skills.Find(x => x.SkillName == "Fishing").GetSkillLevelUnboosted();
        total += skills.Find(x => x.SkillName == "Woodcutting").GetSkillLevelUnboosted();
        total += skills.Find(x => x.SkillName == "Hunting").GetSkillLevelUnboosted();
        return total;
    }
    private int GetCraftingLevels()
    {
        int total = 0;
        total += skills.Find(x => x.SkillName == "Smithing").GetSkillLevelUnboosted();
        total += skills.Find(x => x.SkillName == "Alchemy").GetSkillLevelUnboosted();
        total += skills.Find(x => x.SkillName == "Woodworking").GetSkillLevelUnboosted();
        total += skills.Find(x => x.SkillName == "Culinary Arts").GetSkillLevelUnboosted();
        total += skills.Find(x => x.SkillName == "Leatherworking").GetSkillLevelUnboosted();
        return total;
    }
    public int GetTotalLevels()
    {
        return skills.Sum(x => x.GetSkillLevelUnboosted());
    }
    public long GetTotalExp()
    {
        return skills.Sum(x => x.SkillExperience);
    }

}
