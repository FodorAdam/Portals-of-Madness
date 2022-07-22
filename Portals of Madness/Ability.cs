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
                imageIcon = Image.FromFile($@"../../Art/Sprites/Characters/{iI}.png");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }
            try
            {
                sprite = Image.FromFile($@"../../Art/Sprites/Characters/{sp}.png");
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }
        }
    }

    [XmlRoot("XMLAbilities")]
    public class XMLAbilities
    {
        [XmlElement("XMLAbility")]
        public XMLAbility[] xmlAbility { get; set; }
    }

    public class XMLAbility
    {
        [XmlElement("name")]
        public string name { get; set; }

        [XmlElement("cost")]
        public int cost { get; set; }

        [XmlElement("cooldown")]
        public int cooldown { get; set; }


        [XmlElement("physAttackDamage")]
        public double physAttackDamage { get; set; }


        [XmlElement("magicAttackDamage")]
        public double magicAttackDamage { get; set; }


        [XmlElement("damageType")]
        public string damageType { get; set; }


        [XmlElement("target")]
        public string target { get; set; }


        [XmlElement("duration")]
        public int duration { get; set; }


        [XmlElement("targetCount")]
        public int targetCount { get; set; }


        [XmlElement("abilityType")]
        public string abilityType { get; set; }

        [XmlElement("modifier")]
        public string modifier { get; set; }

        [XmlElement("modifiedAmount")]
        public double modifiedAmount { get; set; }

        [XmlElement("imageIcon")]
        public string imageIcon { get; set; }

        [XmlElement("sprite")]
        public string sprite { get; set; }
    }
}
