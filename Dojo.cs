using System;
using System.Collections.Generic;

public class Dojo
{
	public string Name { get; set; }
    public int ID { get; set; }
    public string Description { get; set; }
    public List<int> OpponentIDs { get; set; }
    public int ItemEarned { get; set; }
    public int AmountEarned { get; set; }
    public DateTime? LastWonTime { get; set; }
    public List<int> NPCIDs { get; set; }
    public int DojoTokens { get; set; }
    public bool BeginChallenge { get; set; }
}
