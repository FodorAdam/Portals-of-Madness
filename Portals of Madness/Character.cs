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
        public int Level { get; set; }

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
        public int StunLength { get; set; }
        public List<DoT> DoTs { get; set; }
        public List<Buff> Buffs { get; set; }

        public string Rarity { get; }
        public bool Selected { get; set; }
        public bool Collectable { get; set; }
        public string CharacterClass { get; }
        public string AIType { get; }

        public Character(string im,
            string id, int l, string n, string chC,
            double bHP, double hpM,
            string rN, int mR,
            double pAt, double pAtM, double mAt, double mAtM,
            double pAr, double pArM, double mAr, double mArM, string weak,
            Ability ab1, Ability ab2, Ability ab3, 
            int ini, string rar, bool coll, string ai) : base(im)
        {
            this.ID = id;
            Name = n;

            if(l < 1)
            {
                Level = 1;
            }
            else if(l > 30)
            {
                Level = 30;
            }
            else
            {
                Level = l;
            }

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

            if(ResourceName == "rage")
            {
                CurrentResource = 0;
            }
            else if(ResourceName == "focus")
            {
                CurrentResource = MaxResource;
            }
            else
            {
                CurrentResource = MaxResource / 2;
            }

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
                ab1,
                ab2,
                ab3
            };

            BaseSpeed = ini;
            Speed = ((BaseSpeed - Level % 10) >= 0 ? (BaseSpeed - Level % 10) : 0);

            DoTs = new List<DoT>();
            Buffs = new List<Buff>();
            Alive = true;
            Stunned = false;
        }

        public bool CanCast(Ability ab)
        {
            if (CurrentResource >= ab.Cost)
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
                    target.HealthChange(CalculateDamageWithoutArmor(ab, target));
                    break;
                case "DoT":
                    target.AddDoT(ab.Name, CalculateDamageWithoutArmor(ab, target) * -1, ab.Duration);
                    break;
                case "HoT":
                    target.AddDoT(ab.Name, CalculateDamageWithoutArmor(ab, target), ab.Duration);
                    break;
                case "resurrect":
                    target.Resurrect();
                    break;
                case "stun":
                    target.Stun(CalculateDamageWithoutArmor(ab, target), ab.Duration);
                    break;
                case "buff":
                    target.AddBuff(ab.Name, ab.Modifier, ab.ModifiedAmount, ab.Duration);
                    break;
                case "debuff":
                    target.AddBuff(ab.Name, ab.Modifier, ab.ModifiedAmount, ab.Duration);
                    break;
                default:
                    break;
            }
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
            else if (l > 30)
            {
                Level = 30;
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
            int mult = 1;
            if(target.WeakTo(ab))
            { 
                mult = 2;
            }

            double pdamage = ((ab.PhysicalAttackDamage * PhysicalAttack - target.PhysicalArmor) * mult) > 0 ?
                ((ab.PhysicalAttackDamage * PhysicalAttack - target.PhysicalArmor) * mult) : 0;

            double mdamage = ((ab.MagicAttackDamage * MagicAttack - target.MagicArmor) * mult) > 0 ?
                ((ab.MagicAttackDamage * MagicAttack - target.MagicArmor) * mult) : 0;

            double damage = pdamage + mdamage;

            return damage;
        }

        private double CalculateDamageWithoutArmor(Ability ab, Character attacker)
        {
            int mult = 1;
            if(WeakTo(ab))
            {
                mult = 2;
            }

            double damage = ((ab.PhysicalAttackDamage * attacker.PhysicalAttack) +
                (ab.MagicAttackDamage * attacker.MagicAttack)) * mult;

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
                    if (CurrentHealth >= MaxHealth * 0.8)
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

        //The character dies
        public void Die()
        {
            CurrentHealth = 0;
            Alive = false;
            Stunned = false;
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
                Stunned = true;
                HealthChange(amount);
                StunLength = length;
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
                return SelectRandomTarget(team);
            }
            return team[target];
        }

        public void Act(List<Character> playerTeam, List<Character> AITeam)
        {
            if (Alive)
            {
                List<Character> targets = new List<Character>();
                switch (AIType)
                {
                    case "basic":
                        if (CanCast(Abilities[0]))
                        {
                            for (int i = 0; i < Abilities[0].TargetCount; i++)
                            {
                                targets.Add(SelectRandomTarget(playerTeam));
                            }
                            CastAbility(Abilities[0], targets);
                        }
                        break;
                    case "advanced":
                        break;
                    default:
                        break;
                }
            }
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
