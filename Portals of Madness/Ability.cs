using System;
using System.Drawing;
using System.IO;

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

        public Ability(string n = "Basic Ability", int co = 0, int cd = 0, double fAD = 1, double mAD = 1, int dur = 0,
            string aT = "blunt", string tT = "enemy", int tC = 1, string abT = "attack", string aM = "aimed",
            double mA = 0, string iI = "nature_bolt", string sp = "")
        {
            Name = n;
            Cost = co;
            Cooldown = cd;
            PhysicalAttackDamage = fAD;
            MagicAttackDamage = mAD;
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
                Console.WriteLine($"{iI} not found!");
            }

            /*try
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
                    Console.WriteLine($"{sp} not found!");
                }
            }*/
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
