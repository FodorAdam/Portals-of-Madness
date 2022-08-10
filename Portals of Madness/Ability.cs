using System;
using System.Drawing;

namespace Portals_of_Madness
{
    public class Ability
    {
        //Name
        public string Name { get; }

        //Resource cost
        public int Cost { get; }

        //How many turns need to pass between uses
        public int Cooldown { get; }

        //Remaining cooldown
        public int CooldownRemaining { get; set; }

        //Physical attack damage
        public double PhysicalAttackDamage { get; }

        //Magical attack damage
        public double MagicAttackDamage { get; }

        //The type of the damage
        public string DamageType { get; }

        //Whom does it target (ally, enemy, all or a specific character in the case of summons)
        public string Target { get; }

        //How long it lasts (only for HotS, DoTs, buffs, debuffs and stuns)
        public int Duration { get; }

        //Amount of people it targets
        public int TargetCount { get; }

        //Type (DoT, heal, etc...)
        public string AbilityType { get; }

        //In the case of attacks, heals and stuns it can be either random or aimed
        //In the case of buffs and debuffs it is the stat that gets modified
        public string Modifier { get; }

        //The amount of modifications buffs and debuffs do
        public double ModifiedAmount { get; }

        //The icon showed on the buttons
        public Image IconImage { get; }

        //The sprite that shows the ability in action
        public Image AttackSprite { get; }

        public Ability(string n, int co, int cd, double fAD, double mAD, int dur, string aT, string tT,
            int tC, string abT, string aM, double mA, string iI, string sp)
        {
            Name = n;
            Cost = co;
            Cooldown = cd;
            PhysicalAttackDamage = fAD;
            PhysicalAttackDamage = mAD;
            Duration = dur;
            DamageType = aT;
            Target = tT;
            TargetCount = tC;
            AbilityType = abT;
            Modifier = aM;
            ModifiedAmount = mA;
            try
            {
                IconImage = Image.FromFile($@"../../Art/Sprites/Spells/{iI}.png");
            }
            catch
            {
                try
                {
                    IconImage = Image.FromFile($@"../../Art/Sprites/Spells/{iI}.jpg");
                }
                catch
                {
                    Console.WriteLine($"{iI}.jpg not found!");
                }
            }
            try
            {
                AttackSprite = Image.FromFile($@"../../Art/Sprites/Spells/{sp}.png");
            }
            catch
            {
                try
                {
                    AttackSprite = Image.FromFile($@"../../Art/Sprites/Spells/{sp}.jpg");
                }
                catch
                {
                    Console.WriteLine($"{sp}.jpg not found!");
                }
            }
        }

        public override string ToString()
        {
            if(AbilityType == "attack" || AbilityType == "heal")
            {
                return $"{Name}\nType: {AbilityType}\nTarget: {TargetCount} {Target}\n" +
                    $"Physical: {PhysicalAttackDamage}, magic: {MagicAttackDamage}\n" +
                    $"Element: {DamageType}";
            }
            else if (AbilityType == "stun" || AbilityType == "DoT" || AbilityType == "HoT")
            {
                return $"{Name}\nType: {AbilityType}\nTarget: {TargetCount} {Target}\n" +
                    $"Physical: {PhysicalAttackDamage}, magic: {MagicAttackDamage}\n" +
                    $"Element: {DamageType}\nDuration: {Duration}";
            }
            return $"{Name}\nType: {AbilityType}";
        }
    }
}
