using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portals_of_Madness
{
    public class Ability
    {
        private readonly string name;
        private readonly int cost;
        private readonly int cooldown;
        private int cooldownRem;
        private readonly double physAttackDamage;
        private readonly double magicAttackDamage;
        private readonly string attackType;
        private readonly string targetType;
        private readonly int duration;
        private readonly int targetCount;
        private readonly string abilityType;
        private readonly string abilityModifier;
        private readonly string imageIcon;
        private readonly string sprite;

    public Ability(string n, int co, int cd, double fAD, double mAD, int dur, string aT, string tT,
            int tC, string abT, string aM, string iI, string sp)
        {
            name = n;
            cost = co;
            cooldown = cd;
            physAttackDamage = fAD;
            physAttackDamage = mAD;
            duration = dur;
            attackType = aT;
            targetType = tT;
            targetCount = tC;
            abilityType = abT;
            abilityModifier = aM;
            imageIcon = iI;
            sprite = sp;
        }

        public string getName()
        {
            return name;
        }

        public int getCost()
        {
            return cost;
        }

        public int getCooldown()
        {
            return cooldown;
        }

        public int getCooldownRem()
        {
            return cooldownRem;
        }

        public double getPhysAttackDamage()
        {
            return physAttackDamage;
        }

        public double getMagicAttackDamage()
        {
            return magicAttackDamage;
        }

        public string getAttackType()
        {
            return attackType;
        }

        public string getTargetType()
        {
            return targetType;
        }

        public int getTargetCount()
        {
            return targetCount;
        }

        public string getAbilityType()
        {
            return abilityType;
        }

        public void setCooldownRem(int cooldownRem)
        {
            this.cooldownRem = cooldownRem;
        }

        public String getAbilityModifier()
        {
            return abilityModifier;
        }

        public string getImageIcon()
        {
            return imageIcon;
        }

        public string getSprite()
        {
            return sprite;
        }

        public int getDuration()
        {
            return duration;
        }
    }
}
