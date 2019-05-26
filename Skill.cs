using System;

public class Skill
{
    public string SkillName { get; set; }
    private int SkillLevel { get; set; }
    public int SkillExperience { get; set; }
    public string SkillDescription { get; set; }
    public int Boost { get; set; }

    public int GetSkillLevel()
    {
        return SkillLevel + Boost;
    }
    public int GetSkillLevelUnboosted()
    {
        return SkillLevel;
    }
    public void SetSkillLevel(int level)
    {
        SkillLevel = level;
    }
}
