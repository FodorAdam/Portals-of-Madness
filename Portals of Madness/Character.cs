using System;
using System.Collections.Generic;

namespace Portals_of_Madness
{
    //Used for damage over time and healing over time effects
    public struct DoT
    {
        public string Name { get; set; }
        public double Amount { get; set; }
        public int Duration { get; set; }

        public DoT(string n, double a, int d)
        {
            Name = n;
            Amount = a;
            Duration = d;
        }

        public void Tick()
        {
            --Duration;
        }
    }

    public struct Buff
    {
        public string Name { get; set; }
        public string Target { get; set; }
        public double Amount { get; set; }
        public int Duration { get; set; }

        public Buff(string n, string t, double a, int d)
        {
            Name = n;
            Target = t;
            Amount = a;
            Duration = d;
        }

        public void Tick()
        {
            --Duration;
        }
    }

    public class Character : Sprite
    {
        public string ID { get; }
        public string Name { get; }
        public string Story { get; }
        public int Level { get; set; }
        public readonly int MaxLevel = 25;
        public int XP { get; set; }

        public double BaseHealth { get; }
        public double HealthMult { get; }
        public double MaxHealth { get; set; }
        public double CurrentHealth { get; set; }

        public string ResourceName { get; }
        public int MaxResource { get; }
        public int CurrentResource { get; set; }

        public double BasePhysicalAttack { get; }
        public double PhysicalAttackMultiplier { get; }
        public double PhysicalAttack { get; set; }
        public double BaseMagicAttack { get; }
        public double MagicAttackMultiplier { get; }
        public double MagicAttack { get; set; }
        public double BasePhysicalArmor { get; }
        public double PhysicalArmorMultiplier { get; }
        public double PhysicalArmor { get; set; }
        public double BaseMagicArmor { get; }
        public double MagicArmorMultiplier { get; }
        public double MagicArmor { get; set; }
        public List<string> Weaknesses { get; }

        public List<Ability> Abilities { get; }

        public int BaseSpeed { get; }
        public int Speed { get; set; }

        public bool Alive { get; set; }
        public bool Stunned { get; set; }
        public bool WasStunned { get; set; }
        public int StunLength { get; set; }
        public List<DoT> DoTs { get; set; }
        public List<Buff> Buffs { get; set; }
        public List<Character> TauntTargets { get; set; }

        public string Rarity { get; }
        public bool Collectable { get; set; }
        public string CharacterClass { get; }
        public string AIType { get; }

        public Character(string im = "default",
            string id = "default", int l = 1, int x = 0, string n = "Default Unit", string s = "", string chC = "",
            double bHP = 100, double hpM = 1.1,
            string rN = "mana", int mR = 20,
            double pAt = 10, double pAtM = 1.1, double mAt = 10, double mAtM = 1.1,
            double pAr = 0, double pArM = 0, double mAr = 0, double mArM = 0, string weak = "fire",
            Ability ab1 = null, Ability ab2 = null, Ability ab3 = null,
            int ini = 5, string rar = "rare", bool coll = false, string ai = "none") : base(im)
        {
            ID = id;
            Name = n;
            Story = s;

            if(l < 1)
            {
                Level = 1;
            }
            else if(l > MaxLevel)
            {
                Level = MaxLevel;
            }
            else
            {
                Level = l;
            }
            XP = x;

            CharacterClass = chC;

            Rarity = rar;
            Collectable = coll;
            AIType = ai;

            BaseHealth = bHP;
            HealthMult = hpM;
            MaxHealth = CalculateStat(Level, BaseHealth, HealthMult);
            CurrentHealth = MaxHealth;

            ResourceName = rN;
            MaxResource = mR;

            ResetResource();

            BasePhysicalAttack = pAt;
            PhysicalAttackMultiplier = pAtM;
            PhysicalAttack = CalculateStat(Level, BasePhysicalAttack, PhysicalAttackMultiplier);
            BaseMagicAttack = mAt;
            MagicAttackMultiplier = mAtM;
            MagicAttack = CalculateStat(Level, BaseMagicAttack, MagicAttackMultiplier);
            BasePhysicalArmor = pAr;
            PhysicalArmorMultiplier = pArM;
            PhysicalArmor = CalculateStat(Level, BasePhysicalArmor, PhysicalArmorMultiplier);
            BaseMagicArmor = mAr;
            MagicArmorMultiplier = mArM;
            MagicArmor = CalculateStat(Level, BaseMagicArmor, MagicArmorMultiplier);
            var weakSplit = weak.Split(',');
            Weaknesses = new List<string>();
            Weaknesses.AddRange(weakSplit);

            Abilities = new List<Ability>
            {
                ab1 ?? new Ability(),
                ab2 ?? new Ability(),
                ab3 ?? new Ability()
            };

            BaseSpeed = ini;
            Speed = ((BaseSpeed - Level % 10) >= 0 ? (BaseSpeed - Level % 10) : 0);

            TauntTargets = new List<Character>();
            DoTs = new List<DoT>();
            Buffs = new List<Buff>();
            Alive = true;
            Stunned = false;
            WasStunned = false;
        }

        public void ResetResource()
        {
            switch (ResourceName)
            {
                case "rage":
                    CurrentResource = MaxResource / 5;
                    break;
                case "focus":
                    CurrentResource = MaxResource;
                    break;
                case "mana":
                    CurrentResource = MaxResource / 2;
                    break;
                case "energy":
                    CurrentResource = 0;
                    break;
            }
        }

        public bool CanCast(Ability ab)
        {
            if (CurrentResource >= ab.Cost || ab.Cost == 0)
            {
                return true;
            }
            return false;
        }

        public void SelectAbility(Ability ab, Character target)
        {
            switch (ab.AbilityType)
            {
                case "attack":
                    target.HealthChange(CalculateDamageWithArmor(ab, target) * -1);
                    break;
                case "heal":
                    target.HealthChange(CalculateDamageWithoutArmor(ab));
                    break;
                case "DoT":
                    target.AddDoT(ab.Name, CalculateDamageWithoutArmor(ab) * -1, ab.Duration);
                    break;
                case "HoT":
                    target.AddDoT(ab.Name, CalculateDamageWithoutArmor(ab), ab.Duration);
                    break;
                case "resurrect":
                    target.Resurrect();
                    break;
                case "stun":
                    target.Stun(CalculateDamageWithoutArmor(ab) * -1, ab.Duration);
                    break;
                case "buff":
                    target.AddBuff(ab.Name, ab.Modifier, ab.ModifiedAmount, ab.Duration);
                    break;
                case "debuff":
                    target.AddBuff(ab.Name, ab.Modifier, ab.ModifiedAmount, ab.Duration);
                    break;
                case "taunt":
                    target.AddTaunt(this);
                    break;
                default:
                    break;
            }
        }

        public void AddTaunt(Character attacker)
        {
            TauntTargets.Add(attacker);
        }

        //Cast a ability at the targets
        public void CastAbility(Ability ab, List<Character> targets)
        {
            CurrentResource -= ab.Cost;
            foreach (Character target in targets)
            {
                SelectAbility(ab, target);
            }
        }

        //Cast a ability at the target
        public void CastAbility(Ability ab, Character target)
        {
            CurrentResource -= ab.Cost;
            SelectAbility(ab, target);
        }

        public Character Summon(Ability ab)
        {
            return null;
        }

        //Calculate the final stats of the character
        private double CalculateStat(int level, double baseStat, double characterMult)
        {
            double result = baseStat;

            for (int i = 1; i < level; i++)
            {
                result *= characterMult;
            }
            return result;
        }

        public void SetLevel(int l)
        {
            if (l < 1)
            {
                Level = 1;
            }
            else if (l > MaxLevel)
            {
                Level = MaxLevel;
            }
            else
            {
                Level = l;
            }
            MaxHealth = CalculateStat(Level, BaseHealth, HealthMult);
            PhysicalAttack = CalculateStat(Level, BasePhysicalAttack, PhysicalAttackMultiplier);
            MagicAttack = CalculateStat(Level, BaseMagicAttack, MagicAttackMultiplier);
            PhysicalArmor = CalculateStat(Level, BasePhysicalArmor, PhysicalArmorMultiplier);
            MagicArmor = CalculateStat(Level, BaseMagicArmor, MagicArmorMultiplier);
        }

        private double CalculateDamageWithArmor(Ability ab, Character target) 
        {
            double mult = 1;
            if(target.WeakTo(ab))
            { 
                mult = 1.3;
            }

            double pdamage = ((ab.PhysicalAttackDamage * PhysicalAttack - target.PhysicalArmor) * mult) > 0 ?
                ((ab.PhysicalAttackDamage * PhysicalAttack - target.PhysicalArmor) * mult) : 0;

            double mdamage = ((ab.MagicAttackDamage * MagicAttack - target.MagicArmor) * mult) > 0 ?
                ((ab.MagicAttackDamage * MagicAttack - target.MagicArmor) * mult) : 0;

            double damage = pdamage + mdamage;

            return damage;
        }

        private double CalculateDamageWithoutArmor(Ability ab)
        {
            double mult = 1;
            if(WeakTo(ab))
            {
                mult = 1.3;
            }

            double damage = ((ab.PhysicalAttackDamage * PhysicalAttack) +
                (ab.MagicAttackDamage * MagicAttack)) * mult;

            return damage;
        }

        private bool WeakTo(Ability ab)
        {
            foreach (string weakness in Weaknesses)
            {
                if (weakness == ab.DamageType)
                {
                    Console.WriteLine($"{Name} is weak to this ability!");
                    return true;
                }
            }
            return false;
        }

        //Adds a DoT to the target
        private void AddDoT(string name, double amount, int dur)
        {
            if (Alive)
            {
                DoTs.Add(new DoT(name, amount, dur));
            }
        }

        //Used for both taking and healing damage
        private void HealthChange(double amount)
        {
            Console.WriteLine($"{Name} took {amount}");
            if (Alive)
            {
                if(CurrentHealth + amount <= 0)
                {
                    if (CurrentHealth >= MaxHealth * 0.8 && AIType == "none")
                    {
                        CurrentHealth = 1;
                    }
                    else
                    {
                        Die();
                    }
                }
                else if(CurrentHealth + amount > MaxHealth)
                {
                    CurrentHealth = MaxHealth;
                }
                else
                {
                    CurrentHealth += amount;
                }
            }
        }

        //The character resets to base state
        public void Reset()
        {
            CurrentHealth = MaxHealth;
            Alive = true;
            Stunned = false;
            WasStunned = false;
            DoTs.Clear();
            foreach (Buff buff in Buffs)
            {
                RemoveBuffEffects(buff);
            }
            Buffs.Clear();
            ResetResource();
        }

        //The character dies
        public void Die()
        {
            CurrentHealth = 0;
            Alive = false;
            Stunned = false;
            WasStunned = false;
            DoTs.Clear();
            foreach (Buff buff in Buffs)
            {
                RemoveBuffEffects(buff);
            }
            Buffs.Clear();
            CurrentResource = 0;

            Console.WriteLine($"{Name} died");
        }

        //Resurrects characters to 20% of their maximum health
        private void Resurrect()
        {
            if(!Alive)
            {
                Alive = true;
                CurrentHealth = MaxHealth * 0.2;
            }
        }

        //Stuns the target and deals damage to them
        private void Stun(double amount, int length)
        {
            if(Alive)
            {
                if (!WasStunned && CharacterClass != "boss")
                {
                    Stunned = true;
                    StunLength = length;
                }
                HealthChange(amount);
            }
        }

        //Removes a DoT from the list
        public void RemoveDoT(DoT dot)
        {
            DoTs.Remove(dot);
        }

        public void AddBuff(string name, string target, double amount, int dur)
        {
            if (Alive)
            {
                Buff buff = new Buff(name, target, amount, dur);
                switch (buff.Target)
                {
                    case "health":
                        MaxHealth *= buff.Amount;
                        break;
                    case "pAttack":
                        PhysicalAttack *= buff.Amount;
                        break;
                    case "mAttack":
                        MagicAttack *= buff.Amount;
                        break;
                    case "pArmor":
                        PhysicalArmor *= buff.Amount;
                        break;
                    case "mArmor":
                        MagicArmor *= buff.Amount;
                        break;
                    case "all":
                        MaxHealth *= buff.Amount;
                        PhysicalAttack *= buff.Amount;
                        MagicAttack *= buff.Amount;
                        PhysicalArmor *= buff.Amount;
                        MagicArmor *= buff.Amount;
                        break;
                    case "attack":
                        PhysicalAttack *= buff.Amount;
                        MagicAttack *= buff.Amount;
                        break;
                    case "armor":
                        PhysicalArmor *= buff.Amount;
                        MagicArmor *= buff.Amount;
                        break;
                    case "survival":
                        MaxHealth *= buff.Amount;
                        PhysicalArmor *= buff.Amount;
                        MagicArmor *= buff.Amount;
                        break;
                    default:
                        break;
                }
                Buffs.Add(buff);
            }
        }

        public void RemoveBuffEffects(Buff buff)
        {
            switch (buff.Target)
            {
                case "health":
                    MaxHealth /= buff.Amount;
                    break;
                case "pAttack":
                    PhysicalAttack /= buff.Amount;
                    break;
                case "mAttack":
                    MagicAttack /= buff.Amount;
                    break;
                case "pArmor":
                    PhysicalArmor /= buff.Amount;
                    break;
                case "mArmor":
                    MagicArmor /= buff.Amount;
                    break;
                case "all":
                    MaxHealth /= buff.Amount;
                    PhysicalAttack /= buff.Amount;
                    MagicAttack /= buff.Amount;
                    PhysicalArmor /= buff.Amount;
                    MagicArmor /= buff.Amount;
                    break;
                case "attack":
                    PhysicalAttack /= buff.Amount;
                    MagicAttack /= buff.Amount;
                    break;
                case "armor":
                    PhysicalArmor /= buff.Amount;
                    MagicArmor /= buff.Amount;
                    break;
                case "survival":
                    MaxHealth /= buff.Amount;
                    PhysicalArmor /= buff.Amount;
                    MagicArmor /= buff.Amount;
                    break;
                default:
                    break;
            }
        }

        public void RemoveBuff(Buff buff)
        {
            Buffs.Remove(buff);
        }

        public Character SelectRandomTarget(List<Character> team)
        {
            Random rand = new Random();
            int target = rand.Next(0, team.Count);
            if (!team[target].Alive)
            {
                return SelectNewTarget(team, target, 0);
            }
            return team[target];
        }

        public Character SelectNewTarget(List<Character> team, int num, int counter)
        {
            counter++;
            if(counter < team.Count)
            {
                num++;
                if (num >= team.Count)
                {
                    num = 0;
                }
                if (!team[num].Alive)
                {
                    return SelectNewTarget(team, num, counter);
                }
                return team[num];
            }
            else
            {
                return team[num];
            }
        }

        public string Act(List<Character> playerTeam, List<Character> AITeam)
        {
            if (Alive)
            {
                if (TauntTargets.Count > 0)
                {
                    return RandomAttackAct(0, TauntTargets);
                }
                Random rand = new Random();
                int r;
                switch (AIType)
                {
                    case "basic":
                        if (CanCast(Abilities[0]))
                        {
                            return RandomAttackAct(0, playerTeam);
                        }
                        break;
                    case "advanced":
                        r = rand.Next(0, 100);
                        if (r > 40 && CanCast(Abilities[1]) && Abilities[1].AbilityType != "heal"
                            && Abilities[1].AbilityType != "HoT" && Abilities[1].AbilityType != "resurrect"
                            && Abilities[1].AbilityType != "taunt")
                        {
                            return RandomAttackAct(1, playerTeam);
                        }
                        else if (CanCast(Abilities[0]))
                        {
                            return RandomAttackAct(0, playerTeam);
                        }
                        break;
                    case "expert":
                        r = rand.Next(0, 100);
                        if (r > 40 && CanCast(Abilities[1]) && Abilities[1].AbilityType != "heal"
                            && Abilities[1].AbilityType != "HoT" && Abilities[1].AbilityType != "resurrect"
                            && Abilities[1].AbilityType != "taunt")
                        {
                            r = rand.Next(0, 100);
                            if (r > 70 && Abilities[1].TargetCount == 1)
                            {
                                return SnipeAct(1, playerTeam);
                            }
                            else
                            {
                                return RandomAttackAct(1, playerTeam);
                            }
                        }
                        else if (CanCast(Abilities[0]))
                        {
                            r = rand.Next(0, 100);
                            if (r > 30 && Abilities[0].TargetCount == 1)
                            {
                                return SnipeAct(0, playerTeam);
                            }
                            else
                            {
                                return RandomAttackAct(0, playerTeam);
                            }
                        }
                        break;
                    case "healer":
                        bool hasHealTarget = false;
                        foreach(Character c in AITeam)
                        {
                            if(c.Alive && c.CurrentHealth < c.MaxHealth)
                            {
                                hasHealTarget = true;
                                break;
                            }
                        }
                        if (CanCast(Abilities[2]) && hasHealTarget && Abilities[2].AbilityType == "heal")
                        {
                            int healTargets = 0;
                            foreach(Character c in AITeam)
                            {
                                if(c.Alive && c.CurrentHealth < c.MaxHealth)
                                {
                                    healTargets++;
                                }
                            }
                            if( healTargets > 1 )
                            {
                                return HealAct(2, AITeam);
                            }
                        }
                        else if(CanCast(Abilities[1]) && hasHealTarget && Abilities[1].AbilityType == "heal")
                        {
                            return HealAct(1, AITeam);
                        }
                        return RandomAttackAct(0, playerTeam);
                    case "sniper":
                        if (CanCast(Abilities[0]) && Abilities[0].TargetCount == 1)
                        {
                            return SnipeAct(0, playerTeam);
                        }
                        break;
                    case "snakeBoss":
                        if (CanCast(Abilities[2]))
                        {
                            return RandomAttackAct(2, playerTeam);
                        }
                        if (CurrentResource % 15 == 0)
                        {
                            return RandomAttackAct(1, playerTeam);
                        }
                        return RandomAttackAct(0, playerTeam);
                    default:
                        break;
                }
                return $"{Name} did nothing";
            }
            return $"{Name} is dead";
        }

        private string RandomAttackAct(int abilityNum, List<Character> playerTeam)
        {
            List<Character> targets = new List<Character>();
            if (TauntTargets.Count > 0)
            {
                targets.Add(TauntTargets[0]);
                TauntTargets.Clear();
                CastAbility(Abilities[abilityNum], targets);
                return $"{Name} used {Abilities[abilityNum].Name} on {targets[0].Name}";
            }

            if (Abilities[abilityNum].TargetCount >= playerTeam.Count)
            {
                targets.AddRange(playerTeam);
            }
            else if(Abilities[abilityNum].TargetCount > 1)
            {
                Random rand = new Random();
                int counter = rand.Next(0, playerTeam.Count);
                for (int i = 0; i < Abilities[abilityNum].TargetCount; i++)
                {
                    if (playerTeam[counter].Alive)
                    {
                        targets.Add(playerTeam[counter]);
                    }

                    if (counter + 1 >= playerTeam.Count)
                    {
                        counter = 0;
                    }
                    else
                    {
                        counter++;
                    }
                }
            }
            else
            {
                targets.Add(SelectRandomTarget(playerTeam));
            }
            CastAbility(Abilities[abilityNum], targets);

            if (targets.Count > 1)
            {
                return $"{Name} used {Abilities[abilityNum].Name} on multiple targets";
            }
            else if(targets.Count == 1)
            {
                return $"{Name} used {Abilities[abilityNum].Name} on {targets[0].Name}";
            }
            else
            {
                return $"{Name} tried to use {Abilities[abilityNum].Name} on noone";
            }
        }

        private string SnipeAct(int abilityNum, List<Character> playerTeam)
        {
            Character target;
            target = playerTeam[0];
            for (int i = 1; i < playerTeam.Count; i++)
            {
                if (playerTeam[i].Alive && target.CurrentHealth > playerTeam[i].CurrentHealth)
                {
                    target = playerTeam[i];
                }
                else if (playerTeam[i].Alive && !target.Alive)
                {
                    target = playerTeam[i];
                }
            }
            CastAbility(Abilities[abilityNum], target);
            return $"{Name} used {Abilities[abilityNum].Name} on {target.Name}";
        }

        private string HealAct(int abilityNum, List<Character> AITeam)
        {
            List<Character> targets = new List<Character>();
            Character target;
            target = AITeam[0];
            if(Abilities[abilityNum].TargetCount == 1)
            {
                for (int i = 1; i < AITeam.Count; i++)
                {
                    if (AITeam[i].Alive &&
                        target.MaxHealth - target.CurrentHealth < AITeam[i].MaxHealth - AITeam[i].CurrentHealth)
                    {
                        target = AITeam[i];
                    }
                    else if (AITeam[i].Alive && !target.Alive)
                    {
                        target = AITeam[i];
                    }
                }
                targets.Add(target);
            }
            else
            {
                for (int j = 0; j < Abilities[abilityNum].TargetCount; j++)
                {
                    if(j >= AITeam.Count)
                    {
                        break;
                    }
                    if(AITeam[j].Alive && AITeam[j].CurrentHealth < AITeam[j].MaxHealth)
                    {
                        targets.Add(AITeam[j]);
                    }
                }
            }
            CastAbility(Abilities[abilityNum], targets);
            if (targets.Count > 1)
            {
                return $"{Name} used {Abilities[abilityNum].Name} on multiple targets";
            }
            else if(targets.Count == 1)
            {
                return $"{Name} used {Abilities[abilityNum].Name} on {targets[0].Name}";
            }
            return "";
        }

        public override string ToString()
        {
            string result = $"{Name}\nLevel: {Level}\nClass: {CharacterClass}\nRarity: {Rarity}\n" +
                $"Speed: {Speed}\nMax Health: {MaxHealth}\nResource: {MaxResource} {ResourceName}\n" +
                $"Attack Power: {PhysicalAttack} physical, {MagicAttack} magical\n" +
                $"Armor: {PhysicalArmor} physical, {MagicArmor} magical\nWeakness(es): {Weaknesses[0]}";
            for(int i=1; i < Weaknesses.Count; i++)
            {
                result += $", {Weaknesses[i]}";
            }
            return result;
        }
    }
}
