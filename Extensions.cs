using System;
using System.Linq;
using System.Collections.Generic;

public static class Extensions
{
    private static readonly Random rand = new Random();
    /*public static int GetExperienceRequired(int level)
    {
        int exp = 0;
        for(int i = 0; i < level; i++)
        {
            exp += (int)(100.0d * Math.Pow(1.1, i));
        }
        return exp;
    }*/
    public static long GetExperienceRequired(int level)
    {
        long exp = 0;
        for (int i = 0; i < level; i++)
        {
            exp += (long)(100.0d * Math.Pow(1.1, i));
        }
        return exp;
    }
    public static int Clamp(this int value, int min, int max)
    {
        return (value < min) ? min : (value > max) ? max : value;
    }
    /*public static int GetProgressForLevel(int exp, int level)
    {
        int expToLevel = GetExperienceRequired(level) - GetExperienceRequired(level - 1);
        int expProgress = exp - GetExperienceRequired(level - 1);

        return (int)(((double)expProgress / (double)expToLevel) * 100);
    }*/
    public static long GetProgressForLevel(long exp, int level)
    {
        long expToLevel = GetExperienceRequired(level) - GetExperienceRequired(level - 1);
        long expProgress = exp - GetExperienceRequired(level - 1);

        return (long)(((double)expProgress / (double)expToLevel) * 100);
    }
    public static Dictionary<GameItem, int> GetItemDicFromString(string data, ItemDatabase itemDB)
    {
        Dictionary<GameItem, int> items = new Dictionary<GameItem, int>();
        string[] dataLines = data.Split('/');
        int pos = 0;
        foreach (string line in dataLines)
        {
            if (line.Length > 0)
            {
                string[] i = line.Split('-');
                int id = int.Parse(i[0]);
                int amount = int.Parse(i[1]);
                int locked = 0;
                if(i.Length > 2)
                {
                    locked = int.Parse(i[2]);
                }
                GameItem item = itemDB.GetItemByID(id);
                if(item == null)
                {
                    Console.WriteLine("Failed to find item." + line);
                }
                else
                {
                    if (locked == 1)
                    {
                        item.IsLocked = true;
                    }
                    item.itemPos = pos;
                    items[item] = amount;
                    pos++;
                }

            }
        }
        return items;
    }
    public static string ReplaceLastOccurrence(string Source, string Find, string Replace)
    {
        int place = Source.LastIndexOf(Find);

        if (place == -1)
            return Source;

        string result = Source.Remove(place, Find.Length).Insert(place, Replace);
        return result;
    }
    public static List<Skill> GetSkillsFromString(string data)
    {
        List<Skill> skills = new List<Skill>();
        string[] dataLines = data.Split('/');
        foreach(string line in dataLines)
        {
            if (line.Length > 0)
            {
                string[] s = line.Split(',');
                Skill skill = new Skill();
                skill.SkillName = s[0];
                skill.SkillExperience = long.Parse(s[1]);
                skill.SetSkillLevel(int.Parse(s[2]));
                if(s.Length > 3 && s[3] != null)
                {
                    skill.IsBlocked = bool.Parse(s[3]);
                }
                skills.Add(skill);
            }
        }
        return skills;
    }

    public static bool FollowerCanAutoCollect(Follower follower, GameItem item)
    {
        if(follower.AutoCollectSkill != item.ActionRequired)
        {
            return false;
        }

        if(follower.AutoCollectLevel >= item.RequiredLevel)
        {
            return true;
        }
        return false;
    }
    public static bool FollowerCanAutoCollect(Follower follower, string skill, int level)
    {
        if (follower.AutoCollectSkill != skill)
        {
            return false;
        }
        if (follower.AutoCollectLevel >= level)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// Conerts source to 2D array.
    /// </summary>
    /// <typeparam name="T">
    /// The type of item that must exist in the source.
    /// </typeparam>
    /// <param name="source">
    /// The source to convert.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if source is null.
    /// </exception>
    /// <returns>
    /// The 2D array of source items.
    /// </returns>
    public static T[,] To2DArray<T>(this IList<IList<T>> source)
    {
        if (source == null)
        {
            throw new ArgumentNullException("source");
        }

        int max = source.Select(l => l).Max(l => l.Count());

        var result = new T[source.Count, max];

        for (int i = 0; i < source.Count; i++)
        {
            for (int j = 0; j < source[i].Count(); j++)
            {
                result[i, j] = source[i][j];
            }
        }

        return result;
    }
    public static string GetIngredientsAsString(GameItem item, ItemDatabase itemDatabase)
    {
        string returnString = "";
        if (item.IngredientIDs.Length == 1)
        {
            return itemDatabase.GetItemByID(item.IngredientIDs[0]).ItemName;
        }
        else
        {
            foreach (int i in item.IngredientIDs)
            {
                GameItem ingredient = itemDatabase.GetItemByID(i);
                returnString += ingredient.ItemName + ", ";
            }
            returnString = returnString.Remove(returnString.Length - 2);
            int lastAndPos = returnString.LastIndexOf(',');
            returnString = returnString.Remove(lastAndPos, 1).Insert(lastAndPos, " and");
        }
        return returnString;
    }
    public static string GetItemsAsString(List<GameItem> items)
    {
        string returnString = "";
        if(items.Count == 0)
        {
            returnString = "Nothing";
            return returnString;
        }
        if(items.Count == 1)
        {
            return items[0].ItemName;
        }
        foreach(GameItem item in items)
        {
            returnString += item.ItemName + ", ";
        }
        returnString = returnString.Remove(returnString.Length - 2);
        int lastAndPos = returnString.LastIndexOf(',');
        returnString = returnString.Remove(lastAndPos, 1).Insert(lastAndPos, " and");
        return returnString;
    }
    public static int GetDrop(Monster monster)
    {
        if(monster.DropTable == null || monster.DropTable.Length == 0)
        {
            return -1;
        }
        int odds = 1000;
        int dropRoll = rand.Next(0, odds);
        int count = 0;
        int[] drops = monster.DropTable.SelectMany(x => x).ToArray();
        for(int i = 0; i < drops.Length - 1; i += 2)
        {
            count += drops[i + 1];
            if(count >= dropRoll)
            {
                return drops[i];
            }
        }
        return -1;
    }
    public static int GetRandomReward(Reward reward)
    {
        if (reward.Rewards == null || reward.Rewards.Length == 0)
        {
            return -1;
        }
        int odds = 1000;
        int dropRoll = rand.Next(0, odds);
        int count = 0;
        int[] drops = reward.Rewards.SelectMany(x => x).ToArray();
        for (int i = 0; i < drops.Length - 1; i += 2)
        {
            count += drops[i + 1];
            if (count >= dropRoll)
            {
                return drops[i];
            }
        }
        return -1;
    }
 
    public static double GetGaussianRandom(double mean, double stdDev)
    {
        double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0 - rand.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                     Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        double randNormal =
                     mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
        return randNormal;
    }
    public static int GetGaussianRandomInt(double mean, double stdDev)
    {
        double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0 - rand.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                     Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        double randNormal =
                     mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
        return (int)randNormal;
    }
    public static double CalculateArmorDamageReduction(Player player)
    {      
        double armorTotal = 0;
        foreach (KeyValuePair<GameItem, int> item in player.inventory.GetItems())
        {
            if (item.Key.IsEquipped)
            {
                armorTotal += item.Key.Armor;
            }
            
        }
        double reduction = ((armorTotal * 0.04d) / (1 + (armorTotal * 0.04d)));
        return reduction;
    }
    public static double CalculateArmorDamageReduction(Monster monster)
    {
        return ((monster.Armor * 0.07d) / (1 + (monster.Armor * 0.07d))) / 2;

    }
}
