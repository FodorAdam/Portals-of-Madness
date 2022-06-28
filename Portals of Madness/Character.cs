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
        public double amount;
        public int duration;

        public DoT(double a, int d)
        {
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
        private readonly string name;
        private int level;
        private readonly string characterClass;

        private readonly double baseHealth;
        private readonly double healthMult;
        private double maxHealth;
        private double currHealth;

        private readonly string resourceName;
        private readonly int maxResource;
        private int currResource;

        private readonly double basePhysAttack;
        private readonly double physAttackMult;
        private double physAttack;
        private readonly double baseMagicAttack;
        private readonly double magicAttackMult;
        private double magicAttack;
        private readonly double basePhysArmor;
        private readonly double physArmorMult;
        private double physArmor;
        private readonly double baseMagicArmor;
        private readonly double magicArmorMult;
        private double magicArmor;
        private readonly List<string> weaknesses;

        private readonly Ability ability1;
        private readonly Ability ability2;
        private readonly Ability ability3;

        private readonly int baseSpeed;
        private int speed;

        private bool alive;
        private bool stunned;
        private bool active;
        private List<DoT> dots;

        public Character(string im,
            int l, string n, string c,
            double bHP, double hpM,
            string rN, int mR,
            double pAt, double pAtM, double mAt, double mAtM,
                double pAr, double pArM, double mAr, double mArM, List<string> weak,
            Ability ab1, Ability ab2, Ability ab3, 
            int ini) : base(im)
        {
            name = n;
            level = l;
            characterClass = c;

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

            alive = true;
            stunned = false;
            active = false;
    }

    //Cast a ability at the target(s)
    public bool castAbility(Ability ab, List<Character> targets)
        {
            if (currResource >= ab.getCost())
            {
                setCurrResource(getCurrResource() - ab.getCost());
                foreach (Character target in targets)
                {
                    switch (ab.getAbilityType())
                    {
                        case "attack":
                            target.healthChange(calcDamageWithArmor(ab));
                            break;
                        case "heal":
                            target.healthChange(calcDamageWithoutArmor(ab));
                            break;
                        case "DoT":
                            target.addDoT(calcDamageWithoutArmor(ab), ab.getDuration());
                            break;
                        case "HoT":
                            target.addDoT(calcDamageWithoutArmor(ab), ab.getDuration());
                            break;
                        case "resurrect":
                            target.resurrect();
                            break;
                        case "stun":
                            target.stun(calcDamageWithoutArmor(ab));
                            break;
                        case "buff":
                            //TODO: Buffs
                            break;
                        case "debuff":
                            //TODO: Debuffs
                            break;
                        default:
                            break;
                    }
                }
                return true;
            }
            return false;
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
            return ((ab.getPhysAttackDamage() * getPhysAttack() - getPhysArmor())
                + (ab.getMagicAttackDamage() * getMagicAttack() - getMagicArmor())) * mult;
        }
        private double calcDamageWithoutArmor(Ability ab)
        {
            int mult = 1;
            if (weakTo(ab))
            {
                mult = 2;
            }
            return (ab.getPhysAttackDamage() * getPhysAttack() + ab.getMagicAttackDamage() * getMagicAttack()) * mult;
        }

        private bool weakTo(Ability ab)
        {
            foreach (string weakness in weaknesses)
            {
                if (weakness == ab.getAttackType())
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
        private void addDoT(double amount, int dur)
        {
            if (alive)
            {
                dots.Add(new DoT(amount, dur));
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
        private void stun(double amount)
        {
            if(alive)
            {
                stunned = true;
                healthChange(amount);
            }
        }

        public Ability getAbility1()
        {
            return ability1;
        }

        public Ability getAbility2()
        {
            return ability2;
        }

        public Ability getAbility3()
        {
            return ability3;
        }

        public string getName()
        {
            return name;
        }

        public double getMaxHealth()
        {
            return maxHealth;
        }

        public double getCurrHealth()
        {
            return currHealth;
        }

        public string getResourceName()
        {
            return resourceName;
        }

        public int getMaxResource()
        {
            return maxResource;
        }

        public int getCurrResource()
        {
            return currResource;
        }

        public double getPhysAttack()
        {
            return physAttack;
        }

        public double getMagicAttack()
        {
            return magicAttack;
        }

        public double getPhysArmor()
        {
            return physArmor;
        }

        public double getMagicArmor()
        {
            return magicArmor;
        }

        public List<string> getWeaknesses()
        {
            return weaknesses;
        }

        public bool isAlive()
        {
            return alive;
        }

        public void setCurrHealth(double currHealth)
        {
            this.currHealth = currHealth;
        }

        public void setCurrResource(int currResource)
        {
            this.currResource = currResource;
        }

        public void setAlive(bool alive)
        {
            this.alive = alive;
        }

        public int getSpeed()
        {
            return speed;
        }

        public bool isStunned()
        {
            return stunned;
        }

        public void setStunned(bool stunned)
        {
            this.stunned = stunned;
        }

        public List<DoT> getDoTs()
        {
            return dots;
        }

        public void removeDoT(DoT dot)
        {
            dots.Remove(dot);
        }

        public string getCharacterClass()
        {
            return characterClass;
        }

        public bool isActive()
        {
            return active;
        }

        public void setActive(bool ac)
        {
            active = ac;
        }
    }
}
