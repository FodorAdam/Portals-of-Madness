using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portals_of_Madness
{
    //Used for damage over time and healing over time effects
    public struct DoT
    {
        public string name { get; set; }
        public double amount { get; set; }
        public int duration { get; set; }

        public DoT(string n, double a, int d)
        {
            name = n;
            amount = a;
            duration = d;
        }

        public void Tick()
        {
            --duration;
        }
    }

    public struct Buff
    {
        public string name { get; set; }
        public string target { get; set; }
        public double amount { get; set; }
        public int duration { get; set; }

        public Buff(string n, string t, double a, int d)
        {
            name = n;
            target = t;
            amount = a;
            duration = d;
        }

        public void Tick()
        {
            --duration;
        }
    }

    public class Character : Sprite
    {
        public string name { get; }
        public int level { get; set; }

        public double baseHealth { get; }
        public double healthMult { get; }
        public double maxHealth { get; set; }
        public double currHealth { get; set; }

        public string resourceName { get; }
        public int maxResource { get; }
        public int currResource { get; set; }

        public double basePhysAttack { get; }
        public double physAttackMult { get; }
        public double physAttack { get; set; }
        public double baseMagicAttack { get; }
        public double magicAttackMult { get; }
        public double magicAttack { get; set; }
        public double basePhysArmor { get; }
        public double physArmorMult { get; }
        public double physArmor { get; set; }
        public double baseMagicArmor { get; }
        public double magicArmorMult { get; }
        public double magicArmor { get; set; }
        public List<string> weaknesses { get; }

        public Ability ability1 { get; }
        public Ability ability2 { get; }
        public Ability ability3 { get; }

        public int baseSpeed { get; }
        public int speed { get; set; }

        public bool alive { get; set; }
        public bool stunned { get; set; }
        public int stunLength { get; set; }
        public bool active { get; set; }
        public List<DoT> dots { get; set; }
        public List<Buff> buffs { get; set; }

        public Character(string im,
            int l, string n,
            double bHP, double hpM,
            string rN, int mR,
            double pAt, double pAtM, double mAt, double mAtM,
                double pAr, double pArM, double mAr, double mArM, List<string> weak,
            Ability ab1, Ability ab2, Ability ab3, 
            int ini) : base(im)
        {
            name = n;
            level = l;

            baseHealth = bHP;
            healthMult = hpM;
            maxHealth = calcStat(level, baseHealth, healthMult);
            currHealth = maxHealth;

            resourceName = rN;
            maxResource = mR;
            currResource = 0;

            basePhysAttack = pAt;
            physAttackMult = pAtM;
            physAttack = calcStat(level, basePhysAttack, physAttackMult);
            baseMagicAttack = mAt;
            magicAttackMult = mAtM;
            magicAttack = calcStat(level, baseMagicAttack, magicAttackMult);
            basePhysArmor = pAr;
            physArmorMult = pArM;
            physArmor = calcStat(level, basePhysArmor, physArmorMult);
            baseMagicArmor = mAr;
            magicArmorMult = mArM;
            magicArmor = calcStat(level, baseMagicArmor, magicArmorMult);
            weaknesses = weak;

            ability1 = ab1;
            ability2 = ab2;
            ability3 = ab3;
            
            baseSpeed = ini;
            speed = ((baseSpeed - level % 10) >= 0 ? (baseSpeed - level % 10) : 0);

            dots = new List<DoT>();
            buffs = new List<Buff>();
            alive = true;
            stunned = false;
            active = false;
        }

        public bool canCast(Ability ab)
        {
            if (currResource >= ab.cost)
            {
                return true;
            }
            return false;
        }

        //Cast a ability at the target(s)
        public void castAbility(Ability ab, List<Character> targets)
        {
            currResource -= ab.cost;
            foreach (Character target in targets)
            {
                switch (ab.abilityType)
                {
                    case "attack":
                        target.healthChange(calcDamageWithArmor(ab));
                        break;
                    case "heal":
                        target.healthChange(calcDamageWithoutArmor(ab));
                        break;
                    case "DoT":
                        target.addDoT(ab.name, calcDamageWithoutArmor(ab), ab.duration);
                        break;
                    case "HoT":
                        target.addDoT(ab.name, calcDamageWithoutArmor(ab), ab.duration);
                        break;
                    case "resurrect":
                        target.resurrect();
                        break;
                    case "stun":
                        target.stun(calcDamageWithoutArmor(ab), ab.duration);
                        break;
                    case "buff":
                        target.addBuff(ab.name, ab.modifier, ab.modifiedAmount, ab.duration);
                        break;
                    case "debuff":
                        target.addBuff(ab.name, ab.modifier, ab.modifiedAmount, ab.duration);
                        break;
                    default:
                        break;
                }
            }
        }

        public Character summon(Ability ab)
        {
            return null;
        }

        //Calculate the final stats of the character
        private double calcStat(int level, double baseStat, double characterMult)
        {
            double result = baseStat;

            for (int i = 1; i < level; i++)
            {
                result *= characterMult;
            }
            return result;
        }

        private double calcDamageWithArmor(Ability ab) 
        {
            int mult = 1;
            if (weakTo(ab)) 
            { 
                mult = 2;
            }
            return ((ab.physAttackDamage * physAttack - physArmor)
                + (ab.magicAttackDamage * magicAttack - magicArmor)) * mult;
        }
        private double calcDamageWithoutArmor(Ability ab)
        {
            int mult = 1;
            if (weakTo(ab))
            {
                mult = 2;
            }
            return (ab.physAttackDamage * physAttack + ab.magicAttackDamage * magicAttack) * mult;
        }

        private bool weakTo(Ability ab)
        {
            foreach (string weakness in weaknesses)
            {
                if (weakness == ab.damageType)
                {
                    return true;
                }
            }
            return false;
        }

        public void SetActive(bool active)
        {
            this.active = active;
        }

        //Adds a DoT to the target
        private void addDoT(string name, double amount, int dur)
        {
            if (alive)
            {
                dots.Add(new DoT(name, amount, dur));
            }
        }

        //Used for both taking and healing damage
        private void healthChange(double amount)
        {
            if(alive)
            {
                if(currHealth - amount <= 0)
                {
                    if (currHealth >= maxHealth * 0.8)
                    {
                        currHealth = 1;
                    }
                    else
                    {
                        die();
                    }
                }
                else if(currHealth - amount > maxHealth)
                {
                    currHealth = maxHealth;
                }
                else
                {
                    currHealth -= amount;
                }
            }
        }

        //The character dies
        public void die()
        {
            currHealth = 0;
            alive = false;
            stunned = false;
            dots.Clear();
            foreach (Buff buff in buffs)
            {
                removeBuffEffects(buff);
            }
            buffs.Clear();
            currResource = 0;
        }

        //Resurrects characters to 20% of their maximum health
        private void resurrect()
        {
            if(!alive)
            {
                alive = true;
                currHealth = maxHealth * 0.2;
            }
        }

        //Stuns the target and deals damage to them
        private void stun(double amount, int length)
        {
            if(alive)
            {
                stunned = true;
                healthChange(amount);
                stunLength = length;
            }
        }

        //Removes a DoT from the list
        public void removeDoT(DoT dot)
        {
            dots.Remove(dot);
        }

        public void addBuff(string name, string target, double amount, int dur)
        {
            if (alive)
            {
                Buff buff = new Buff(name, target, amount, dur);
                switch (buff.target)
                {
                    case "health":
                        maxHealth *= buff.amount;
                        break;
                    case "pAttack":
                        physAttack *= buff.amount;
                        break;
                    case "mAttack":
                        magicAttack *= buff.amount;
                        break;
                    case "pArmor":
                        physArmor *= buff.amount;
                        break;
                    case "mArmor":
                        magicArmor *= buff.amount;
                        break;
                    case "all":
                        maxHealth *= buff.amount;
                        physAttack *= buff.amount;
                        magicAttack *= buff.amount;
                        physArmor *= buff.amount;
                        magicArmor *= buff.amount;
                        break;
                    case "attack":
                        physAttack *= buff.amount;
                        magicAttack *= buff.amount;
                        break;
                    case "armor":
                        physArmor *= buff.amount;
                        magicArmor *= buff.amount;
                        break;
                    case "survival":
                        maxHealth *= buff.amount;
                        physArmor *= buff.amount;
                        magicArmor *= buff.amount;
                        break;
                    default:
                        break;
                }
                buffs.Add(buff);
            }
        }

        public void removeBuffEffects(Buff buff)
        {
            switch (buff.target)
            {
                case "health":
                    maxHealth /= buff.amount;
                    break;
                case "pAttack":
                    physAttack /= buff.amount;
                    break;
                case "mAttack":
                    magicAttack /= buff.amount;
                    break;
                case "pArmor":
                    physArmor /= buff.amount;
                    break;
                case "mArmor":
                    magicArmor /= buff.amount;
                    break;
                case "all":
                    maxHealth /= buff.amount;
                    physAttack /= buff.amount;
                    magicAttack /= buff.amount;
                    physArmor /= buff.amount;
                    magicArmor /= buff.amount;
                    break;
                case "attack":
                    physAttack /= buff.amount;
                    magicAttack /= buff.amount;
                    break;
                case "armor":
                    physArmor /= buff.amount;
                    magicArmor /= buff.amount;
                    break;
                case "survival":
                    maxHealth /= buff.amount;
                    physArmor /= buff.amount;
                    magicArmor /= buff.amount;
                    break;
                default:
                    break;
            }
        }

        public void removeBuff(Buff buff)
        {
            buffs.Remove(buff);
        }
    }
}
