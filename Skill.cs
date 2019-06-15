using System;

public class Skill
{
    public string SkillName { get; set; }
    private int SkillLevel { get; set; }
    private int skillExperience;
    public int SkillExperience {
        get { return (int)skillExperience; }
        set {
            if (value >= 0)
            {
                skillExperience = Math.Min(value, int.MaxValue - 47483647);
            }
            else
            {
                skillExperience = int.MaxValue - 47483647;
            }
        }
    }
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
