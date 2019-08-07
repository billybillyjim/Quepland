using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

public class BattleManager
{
    private List<Monster> monsters = new List<Monster>();
    public GameState gameState;
    IJSRuntime JSRuntime;
    public MessageManager messageManager;
    public ItemDatabase itemDatabase;
    Random rand = new Random();
    public bool battleFound;
    public bool battleStarted;
    public Monster opponent;
    public bool autoFight;
    public List<Monster> possibleMonsters = new List<Monster>();

    public BattleManager()
	{
	}

    public async Task LoadMonsters(HttpClient Http, GameState state, MessageManager mm, ItemDatabase database, IJSRuntime js)
    {
        Monster[] monsterArray = await Http.GetJsonAsync<Monster[]>("data/Monsters.json");
        monsters = monsterArray.ToList();
        gameState = state;
        messageManager = mm;
        itemDatabase = database;
        JSRuntime = js;
    }
    public List<Monster> GetMonstersForArea(Area area)
    {
        List<Monster> monsterList = new List<Monster>();
        foreach(int i in area.MonsterIDs)
        {
            monsterList.Add(monsters[i]);
        }
        possibleMonsters = monsterList;
        return monsterList;
    }
    public List<Monster> GetAllMonsters()
    {
        return monsters;
    }
    public Monster GetMonsterByID(int id)
    {
        return monsters[id];
    }
    public void StartAutoFighting()
    {
        autoFight = true;
        gameState.isFighting = true;
        if (gameState.autoFightTimer != null)
        {
            gameState.autoFightTimer.Dispose();
            gameState.autoFightTimer = null;
        }
        messageManager.AddMessage(gameState.GetPlayer().activeFollower.Name + " looks for an enemy for you to fight.");
        gameState.autoFightTimer = new Timer(new TimerCallback(_ =>
        {
            AutoBattle();
        }), null, gameState.GetPlayer().activeFollower.AutoCollectSpeed, gameState.GetPlayer().activeFollower.AutoCollectSpeed);
        gameState.UpdateState();
    }
    public void AutoBattle()
    {
        if (gameState.GetPlayer().activeFollower != null & gameState.GetPlayer().activeFollower.AutoCollectSkill == "Fighting")
        {
            if (battleFound == false)
            {
                gameState.isFighting = true;
                messageManager.AddMessage(gameState.GetPlayer().activeFollower.Name + " found a battle!");
                FindBattle();
                StartBattle();
            }
        }

    }
    public void FindBattle()
    {
        opponent = possibleMonsters[rand.Next(0, possibleMonsters.Count)];
        messageManager.AddMessage("You encounter a " + opponent.Name);
        opponent.CurrentHP = opponent.HP;
        battleFound = true;
    }
    public void StartBattle()
    {
        battleStarted = true;
        gameState.isFighting = true;
        if (gameState.attackTimer != null)
        {
            gameState.attackTimer.Dispose();
        }
        if (gameState.monsterAttackTimer != null)
        {
            gameState.monsterAttackTimer.Dispose();
        }
        if (gameState.GetPlayer().GetWeapon() != null && gameState.GetPlayer().GetWeapon().ActionRequired == "Archery" && gameState.GetPlayerInventory().HasArrows())
        {
            gameState.attackTimer = new Timer(new TimerCallback(_ =>
            {
                Attack();
                gameState.UpdateState();
               
            }), null, 150, gameState.GetPlayer().GetWeaponAttackSpeed());
        }
        else
        {
            gameState.attackTimer = new Timer(new TimerCallback(_ =>
            {
                Attack();
                gameState.UpdateState();

            }), null, gameState.GetPlayer().GetWeaponAttackSpeed(), gameState.GetPlayer().GetWeaponAttackSpeed());
        }


        gameState.monsterAttackTimer = new Timer(new TimerCallback(_ =>
        {
            BeAttacked();
            gameState.UpdateState();

        }), null, 1000 * opponent.AttackSpeed, 1000 * opponent.AttackSpeed);

        gameState.UpdateState();

    }
    public void Attack()
    {
        if (battleFound)
        {
            string battleString = "";
            string color = "black";
            int dmgDealt = Math.Min(gameState.GetPlayer().GetDamageDealt(opponent), opponent.CurrentHP);
            if (gameState.GetPlayer().GetWeapon() != null)
            {
                GameItem weapon = gameState.GetPlayer().GetWeapon();
                if (string.IsNullOrEmpty(weapon.StatusEffect) == false)
                {
                    if (rand.NextDouble() < weapon.EffectOdds)
                    {
                        color = "blue";
                        if (weapon.StatusEffect == "Drain")
                        {

                            color = "red";

                        }

                        ApplyStatusEffect(weapon.StatusEffect, weapon.EffectDuration, dmgDealt);

                    }
                }
                if (weapon.ActionRequired == "Archery")
                {
                    if (gameState.GetPlayerInventory().HasArrows())
                    {
                        if (string.IsNullOrEmpty(gameState.GetPlayerInventory().GetStrongestArrows().StatusEffect) == false)
                        {
                            if (rand.NextDouble() < gameState.GetPlayerInventory().GetStrongestArrows().EffectOdds)
                            {
                                ApplyStatusEffect(gameState.GetPlayerInventory().GetStrongestArrows().StatusEffect, gameState.GetPlayerInventory().GetStrongestArrows().EffectDuration, dmgDealt);
                                color = "blue";
                            }
                        }
                        if (rand.Next(2) == 0)
                        {
                            gameState.GetPlayerInventory().RemoveOneOfItem(gameState.GetPlayerInventory().GetStrongestArrows());
                        }
                        battleString = "You shoot the " + opponent.Name + " for " + dmgDealt + " damage.";
                    }
                    else
                    {
                        battleString = "You whack the " + opponent.Name + " with your bow for " + dmgDealt + " damage.";
                    }
                }
                else
                {
                    battleString = "You dealt " + dmgDealt + " damage to the " + opponent.Name + ".";
                }

                if (opponent.Weakness.Contains(weapon.ActionRequired))
                {
                    gameState.GetPlayer().GainExperienceFromWeapon(weapon, dmgDealt * 2);
                    battleString += " It seemed to be very effective!";
                }
                else if (opponent.Strength.Contains(weapon.ActionRequired))
                {
                    gameState.GetPlayer().GainExperienceFromWeapon(weapon, dmgDealt / 2);
                    battleString += " It didn't seem to be very effective...";
                }
                else
                {
                    gameState.GetPlayer().GainExperienceFromWeapon(weapon, dmgDealt);
                }
            }
            else
            {
                gameState.GetPlayer().GainExperience("Strength", dmgDealt * 3);
                battleString = "You punch the " + opponent.Name + " for " + dmgDealt + " damage.";
            }
            messageManager.AddMessage(battleString, color);
            opponent.CurrentHP -= dmgDealt;

            if (opponent.CurrentHP <= 0)
            {
                WinBattle();
            }
        }

    }
    public void BeAttacked()
    {
        if (battleFound)
        {
            opponent.StatusEffectTimeLeft--;
            if (opponent.CurrentStatusEffect == "Freeze" && opponent.StatusEffectTimeLeft > 0)
            {
                messageManager.AddMessage(opponent.Name + " is frozen and could not attack.");
                return;
            }
            int dmg = Math.Max(1, Extensions.GetGaussianRandomInt(opponent.Damage, opponent.Damage / 2d));
            dmg = (int)(dmg * Math.Max(1 - Extensions.CalculateArmorDamageReduction(gameState.GetPlayer()), 0.05d));
            gameState.GetPlayer().GainExperience("HP", Math.Min(gameState.GetPlayer().CurrentHP, dmg) * 8);
            gameState.GetPlayer().CurrentHP -= dmg;
            messageManager.AddMessage("You took " + dmg + " damage from the " + opponent.Name + "'s attack.");

            if (gameState.GetPlayer().CurrentHP <= 0)
            {
                LoseBattle();
            }
            if (opponent.StatusEffect != null && battleFound)
            {
                if (opponent.StatusEffect == "Drain")
                {

                    int maxHP = opponent.HP - opponent.CurrentHP;
                    opponent.CurrentHP += Math.Min(maxHP, dmg);
                    if (Math.Min(maxHP, dmg) > 0)
                    {
                        messageManager.AddMessage(opponent.Name + " absorbed " + Math.Min(maxHP, dmg) + " HP!");
                    }

                }
            }
        }

    }
    public void WinBattle()
    {
        gameState.totalKills++;
        gameState.IncrementKillCount(opponent.ID);
        messageManager.AddMessage("You defeated the " + opponent.Name);
        if (opponent.AlwaysDrops != null)
        {
            if (gameState.GetPlayerInventory().AddOneOfMultipleItems(itemDatabase.GetItems(opponent.AlwaysDrops)))
            {
                messageManager.AddMessage("You got " + Extensions.GetItemsAsString(itemDatabase.GetItems(opponent.AlwaysDrops)));
            }
            else
            {
                messageManager.AddMessage("Your inventory is full, so you didn't pick up any drops.");
            }
        }
        GameItem drop = itemDatabase.GetItemByID(Extensions.GetDrop(opponent));
        if (drop != null)
        {
            if (gameState.GetPlayerInventory().AddMultipleOfItem(drop, 1))
            {
                messageManager.AddMessage("You got a " + drop.ItemName);
            }
            else
            {
                messageManager.AddMessage("Your inventory is full, so you didn't pick up any drops.");
            }
        }

        battleFound = false;
        EndTimers();
        battleStarted = false;

        
        if (autoFight)
        {
            StartAutoFighting();
        }
        else
        {
            gameState.isFighting = false;
        }
        gameState.UpdateState();
        JSRuntime.InvokeAsync<object>("kongregateFunctions.updateTotalKills", gameState.totalKills);
    }
    public void LoseBattle()
    {
        messageManager.AddMessage("You died to " + opponent.Name);
        gameState.GetPlayer().CurrentHP = gameState.GetPlayer().MaxHP;
        gameState.totalDeaths++;
        battleFound = false;
        battleStarted = false;
        autoFight = false;
        EndTimers();
        gameState.isFighting = false;
    }
    public void StopAutoBattling()
    {
        battleFound = false;
        battleStarted = false;
        autoFight = false;
        EndTimers();
        messageManager.AddMessage("You stop looking for fights.");
        gameState.isFighting = false;
    }
    public void EndTimers()
    {
        if (gameState.attackTimer != null)
        {
            gameState.attackTimer.Dispose();
            gameState.attackTimer = null;
        }
        if (gameState.monsterAttackTimer != null)
        {
            gameState.monsterAttackTimer.Dispose();
            gameState.monsterAttackTimer = null;
        }
        if (gameState.autoFightTimer != null)
        {
            gameState.autoFightTimer.Dispose();
            gameState.autoFightTimer = null;
        }
        if (autoFight == false)
        {
            gameState.isFighting = false;
        }

        gameState.UpdateState();

    }
    public void ApplyStatusEffect(string effect, int duration, int dmgDealt)
    {
        if (opponent.ImmuneTo == null || opponent.ImmuneTo.Contains(effect) == false)
        {

            if (effect == "Drain")
            {
                int maxHPGain = gameState.GetPlayer().MaxHP - gameState.GetPlayer().CurrentHP;
                gameState.GetPlayer().CurrentHP += Math.Min(dmgDealt / duration, maxHPGain);
                messageManager.AddMessage("You absorbed " + Math.Min(dmgDealt / duration, maxHPGain) + " HP from the " + opponent.Name, "red");
            }
            else
            {
                opponent.CurrentStatusEffect = effect;
                opponent.StatusEffectTimeLeft = duration;
                if (gameState.GetPlayer().GetWeapon().ActionRequired == "Archery")
                {
                    messageManager.AddMessage(opponent.Name + " was affected by the " + gameState.GetPlayerInventory().GetStrongestArrows().ItemName + "' effect!", "blue");
                }
                else
                {
                    messageManager.AddMessage(opponent.Name + " was affected by the " + gameState.GetPlayer().GetWeapon().ItemName + "'s effect!", "blue");
                }
            }




        }

    }
}
