using System;

public class Skill
{
    public string SkillName { get; set; }
    private int SkillLevel { get; set; }
    private long skillExperience;
    public bool IsBlocked { get; set; }

    public long SkillExperience {
        get { return skillExperience; }
        set {
            if (value >= 0)
            {
                skillExperience = Math.Min(value, long.MaxValue - 20000000);
            }
            else
            {
                skillExperience = long.MaxValue - 20000000;
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
