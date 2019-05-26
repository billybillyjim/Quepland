using System;
using System.Linq;
using System.Collections.Generic;

public static class Extensions
{
    private static readonly Random rand = new Random();
    public static int GetExperienceRequired(int level)
    {
        int exp = 0;
        for(int i = 0; i < level; i++)
        {
            exp += (int)(100.0d * Math.Pow(1.1, i));
        }
        return exp;
    }
    public static int GetProgressForLevel(int exp, int level)
    {
        int expToLevel = GetExperienceRequired(level) - GetExperienceRequired(level - 1);
        int expProgress = exp - GetExperienceRequired(level - 1);

        return (int)(((double)expProgress / (double)expToLevel) * 100);
    }
    public static Dictionary<GameItem, int> GetItemDicFromString(string data, ItemDatabase itemDB)
    {
        Dictionary<GameItem, int> items = new Dictionary<GameItem, int>();
        string[] dataLines = data.Split('/');
        foreach (string line in dataLines)
        {
            if (line.Length > 0)
            { 
                string[] i = line.Split('-');
                int id = int.Parse(i[0]);
                int amount = int.Parse(i[1]);
                items.Add(itemDB.GetItemByID(id), amount);
            }
        }
        return items;
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
                skill.SkillExperience = int.Parse(s[1]);
                skill.SetSkillLevel(int.Parse(s[2]));
                Console.WriteLine(s[0] + ", " + s[1] + ", " + s[2]);
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
}
