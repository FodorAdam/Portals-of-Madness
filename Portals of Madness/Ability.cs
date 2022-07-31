using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Portals_of_Madness
{
    public class Ability
    {
        //Name
        public string name { get; }

        //Resource cost
        public int cost { get; }

        //How many turns need to pass between uses
        public int cooldown { get; }

        //Remaining cooldown
        public int cooldownRem { get; set; }

        //Physical attack damage
        public double physAttackDamage { get; }

        //Magical attack damage
        public double magicAttackDamage { get; }

        //The type of the damage
        public string damageType { get; }

        //Whom does it target (ally, enemy, all or a specific character in the case of summons)
        public string target { get; }

        //How long it lasts (only for HotS, DoTs, buffs, debuffs and stuns)
        public int duration { get; }

        //Amount of people it targets
        public int targetCount { get; }

        //Type (DoT, heal, etc...)
        public string abilityType { get; }

        //In the case of attacks, heals and stuns it can be either random or aimed
        //In the case of buffs and debuffs it is the stat that gets modified
        public string modifier { get; }

        //The amount of modifications buffs and debuffs do
        public double modifiedAmount { get; }

        //The icon showed on the buttons
        public Image imageIcon { get; }

        //The sprite that shows the ability in action
        public Image sprite { get; }

        public Ability(string n, int co, int cd, double fAD, double mAD, int dur, string aT, string tT,
            int tC, string abT, string aM, double mA, string iI, string sp)
        {
            name = n;
            cost = co;
            cooldown = cd;
            physAttackDamage = fAD;
            physAttackDamage = mAD;
            duration = dur;
            damageType = aT;
            target = tT;
            targetCount = tC;
            abilityType = abT;
            modifier = aM;
            modifiedAmount = mA;
            try
            {
                imageIcon = Image.FromFile($@"../../Art/Sprites/Spells/{iI}.png");
            }
            catch
            {
                try
                {
                    imageIcon = Image.FromFile($@"../../Art/Sprites/Spells/{iI}.jpg");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.Message}");
                }
            }
            try
            {
                sprite = Image.FromFile($@"../../Art/Sprites/Spells/{sp}.png");
            }
            catch
            {
                try
                {
                    sprite = Image.FromFile($@"../../Art/Sprites/Spells/{sp}.jpg");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.Message}");
                }
            }
        }

        public override string ToString()
        {
            if(abilityType == "attack" || abilityType == "heal")
            {
                return $"{name}\nType: {abilityType}\nTarget: {targetCount} {target}\n" +
                    $"Physical: {physAttackDamage}, magic: {magicAttackDamage}\n" +
                    $"Element: {damageType}";
            }
            else if (abilityType == "stun" || abilityType == "DoT" || abilityType == "HoT")
            {
                return $"{name}\nType: {abilityType}\nTarget: {targetCount} {target}\n" +
                    $"Physical: {physAttackDamage}, magic: {magicAttackDamage}\n" +
                    $"Element: {damageType}\nDuration: {duration}";
            }
            return $"{name}\nType: {abilityType}";
        }
    }
}
