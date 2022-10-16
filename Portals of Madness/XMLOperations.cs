using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Portals_of_Madness
{
    public class XMLOperations
    {
        public List<Ability> AllAbilities;

        public XMLOperations()
        {
            AllAbilities = new List<Ability>();
        }

        private void SetUpAllAbilities()
        {
            XMLAbilities xabs = (XMLAbilities)GenericDeserializer<XMLAbilities>($@"../../Abilities/Abilities.xml");
            foreach (XMLAbility xab in xabs.XmlAbility)
            {
                AllAbilities.Add(ConvertToAbility(xab));
            }
            Console.WriteLine("---------------Abilities---------------");
            foreach (Ability ab in AllAbilities)
            {
                Console.WriteLine(ab);
                Console.WriteLine("---------------------------------------");
            }
        }

        public Object GenericDeserializer<T>(string path)
        {
            T obj;
            XmlSerializer des = new XmlSerializer(typeof(T));
            try
            {
                TextReader sr = new StreamReader(path);
                obj = (T)des.Deserialize(sr);
                sr.Close();
                return obj;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
            }
            return null;
        }

        public Ability GetAblilityByName(string abn)
        {
            foreach (Ability ab in AllAbilities)
            {
                if (ab.Name.Equals(abn))
                {
                    return ab;
                }
            }
            return null;
        }

        public Ability ConvertToAbility(XMLAbility x)
        {
            return new Ability(x.Name, x.Cost, x.Cooldown, x.PhysAttackDamage, x.MagicAttackDamage,
                x.Duration, x.DamageType, x.Target, x.TargetCount, x.AbilityType, x.Modifier, x.ModifiedAmount,
                x.ImageIcon, x.Sprite);
        }

        public Character ConvertToCharacter(XMLCharacter x)
        {
            if(AllAbilities.Count == 0)
            {
                SetUpAllAbilities();
            }
            return new Character(x.ImageSet, x.Id, x.Level, x.XP, x.Name, x.Story, x.CharacterClass, x.BaseHealth, x.HealthMult,
                x.ResourceName, x.MaxResource, x.BasePhysAttack, x.PhysAttackMult, x.BaseMagicAttack,
                x.MagicAttackMult, x.BasePhysArmor, x.PhysArmorMult, x.BaseMagicArmor,
                x.MagicArmorMult, x.Weaknesses,
                GetAblilityByName(x.Ability1Name), GetAblilityByName(x.Ability2Name), GetAblilityByName(x.Ability3Name),
                x.BaseSpeed, x.Rarity, x.Collectable, x.AIName);
        }
    }

    [XmlRoot("XMLAbilities")]
    public class XMLAbilities
    {
        [XmlElement("XMLAbility")]
        public XMLAbility[] XmlAbility { get; set; }
    }

    public class XMLAbility
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("cost")]
        public int Cost { get; set; }

        [XmlElement("cooldown")]
        public int Cooldown { get; set; }


        [XmlElement("physAttackDamage")]
        public double PhysAttackDamage { get; set; }


        [XmlElement("magicAttackDamage")]
        public double MagicAttackDamage { get; set; }


        [XmlElement("damageType")]
        public string DamageType { get; set; }


        [XmlElement("target")]
        public string Target { get; set; }


        [XmlElement("duration")]
        public int Duration { get; set; }


        [XmlElement("targetCount")]
        public int TargetCount { get; set; }


        [XmlElement("abilityType")]
        public string AbilityType { get; set; }

        [XmlElement("modifier")]
        public string Modifier { get; set; }

        [XmlElement("modifiedAmount")]
        public double ModifiedAmount { get; set; }

        [XmlElement("imageIcon")]
        public string ImageIcon { get; set; }

        [XmlElement("sprite")]
        public string Sprite { get; set; }
    }

    [XmlRoot("XMLCharacters")]
    public class XMLCharacters
    {
        [XmlElement("XMLCharacter")]
        public XMLCharacter[] XmlCharacter { get; set; }
    }

    public class XMLCharacter
    {
        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("imageSet")]
        public string ImageSet { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("story")]
        public string Story { get; set; }

        [XmlElement("level")]
        public int Level { get; set; }

        [XmlElement("xp")]
        public int XP { get; set; }

        [XmlElement("characterClass")]
        public string CharacterClass { get; set; }

        [XmlElement("baseHealth")]
        public double BaseHealth { get; set; }

        [XmlElement("healthMult")]
        public double HealthMult { get; set; }

        [XmlElement("resourceName")]
        public string ResourceName { get; set; }

        [XmlElement("maxResource")]
        public int MaxResource { get; set; }

        [XmlElement("basePhysAttack")]
        public double BasePhysAttack { get; set; }

        [XmlElement("physAttackMult")]
        public double PhysAttackMult { get; set; }

        [XmlElement("baseMagicAttack")]
        public double BaseMagicAttack { get; set; }

        [XmlElement("magicAttackMult")]
        public double MagicAttackMult { get; set; }

        [XmlElement("basePhysArmor")]
        public double BasePhysArmor { get; set; }

        [XmlElement("physArmorMult")]
        public double PhysArmorMult { get; set; }

        [XmlElement("baseMagicArmor")]
        public double BaseMagicArmor { get; set; }

        [XmlElement("magicArmorMult")]
        public double MagicArmorMult { get; set; }

        [XmlElement("weaknesses")]
        public string Weaknesses { get; set; }

        [XmlElement("ability1Name")]
        public string Ability1Name { get; set; }

        [XmlElement("ability2Name")]
        public string Ability2Name { get; set; }

        [XmlElement("ability3Name")]
        public string Ability3Name { get; set; }

        [XmlElement("baseSpeed")]
        public int BaseSpeed { get; set; }

        [XmlElement("rarity")]
        public string Rarity { get; set; }

        [XmlElement("acquired")]
        public bool Acquired { get; set; }

        [XmlElement("aiName")]
        public string AIName { get; set; }

        [XmlElement("collectable")]
        public bool Collectable { get; set; }
    }

    [XmlRoot("Encounters")]
    public class Encounters
    {
        [XmlElement("id")]
        public int Id { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("lore")]
        public string Lore { get; set; }

        [XmlElement("unlocked")]
        public string Unlocked { get; set; }

        [XmlElement("previous")]
        public string Previous { get; set; }

        [XmlElement("side")]
        public string Side { get; set; }

        [XmlElement("Encounter")]
        public Encounter[] Encounter { get; set; }
    }

    public class Encounter
    {
        [XmlElement("id")]
        public int Id { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("background1")]
        public string Background1 { get; set; }

        [XmlElement("background2")]
        public string Background2 { get; set; }

        [XmlElement("background2movement")]
        public string Background2Movement { get; set; }

        [XmlElement("foreground")]
        public string Foreground { get; set; }

        [XmlElement("foregroundmovement")]
        public string ForegroundMovement { get; set; }

        [XmlElement("optional")]
        public bool Optional { get; set; }

        [XmlElement("Fights")]
        public Fights Fights { get; set; }
    }

    public class Fights
    {
        [XmlElement("Fight")]
        public Fight[] Fight { get; set; }
    }

    public class Fight
    {
        [XmlElement("id")]
        public int Id { get; set; }

        [XmlElement("enemies")]
        public string Enemies { get; set; }

        [XmlElement("amount")]
        public int Amount { get; set; }

        [XmlElement("type")]
        public string Type { get; set; }
    }

    [XmlRoot("Dialogs")]
    public class Dialogs
    {
        [XmlElement("id")]
        public int Id { get; set; }


        [XmlElement("Dialog")]
        public Dialog[] Dialog { get; set; }
    }

    public class Dialog
    {
        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("type")]
        public string Type { get; set; }

        [XmlElement("image")]
        public string Image { get; set; }

        [XmlElement("Lines")]
        public Lines Lines { get; set; }
    }

    public class Lines
    {
        [XmlElement("Line")]
        public Line[] Line { get; set; }
    }

    public class Line
    {
        [XmlElement("speaker")]
        public string Speaker { get; set; }

        [XmlElement("side")]
        public string Side { get; set; }

        [XmlElement("str")]
        public string Str { get; set; }
    }
}
