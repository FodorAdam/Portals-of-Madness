using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Portals_of_Madness
{
    public class XMLOperations
    {
        readonly List<Ability> AllAbilities;

        public XMLOperations()
        {
            AllAbilities = new List<Ability>();
            XMLAbilities xabs = AbilityDeserializer($@"../../Abilities/Abilities.xml");
            foreach(XMLAbility xab in xabs.xmlAbility)
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

        public XMLAbilities AbilityDeserializer(string path)
        {
            XMLAbilities obj = new XMLAbilities();
            XmlSerializer des = new XmlSerializer(typeof(XMLAbilities));
            try
            {
                TextReader sr = new StreamReader(path);
                obj = (XMLAbilities)des.Deserialize(sr);
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
            }
            return obj;
        }

        public XMLCharacters CharacterDeserializer(string path)
        {
            XMLCharacters obj = new XMLCharacters();
            XmlSerializer des = new XmlSerializer(typeof(XMLCharacters));
            try
            {
                TextReader sr = new StreamReader(path);
                obj = (XMLCharacters)des.Deserialize(sr);
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
            }
            return obj;
        }

        //Deserializes the mission XML file
        public Encounters MissionDeserializer(string path)
        {
            Encounters obj = new Encounters();
            XmlSerializer des = new XmlSerializer(typeof(Encounters));
            try
            {
                TextReader sr = new StreamReader(path);
                obj = (Encounters)des.Deserialize(sr);
                sr.Close();
            }
            catch { }
            return obj;
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
            return new Ability(x.name, x.cost, x.cooldown, x.physAttackDamage, x.magicAttackDamage,
                x.duration, x.damageType, x.target, x.targetCount, x.abilityType, x.modifier, x.modifiedAmount,
                x.imageIcon, x.sprite);
        }

        public Character ConvertToCharacter(XMLCharacter x)
        {
            return new Character(x.imageSet, x.id, x.level, x.name, x.characterClass, x.baseHealth, x.healthMult,
                x.resourceName, x.maxResource, x.basePhysAttack, x.physAttackMult, x.baseMagicAttack,
                x.magicAttackMult, x.basePhysArmor, x.physArmorMult, x.baseMagicArmor,
                x.magicArmorMult, x.weaknesses,
                GetAblilityByName(x.ability1Name), GetAblilityByName(x.ability2Name), GetAblilityByName(x.ability3Name),
                x.baseSpeed, x.rarity, x.collectable, x.aiName);
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

    [XmlRoot("XMLCharacters")]
    public class XMLCharacters
    {
        [XmlElement("XMLCharacter")]
        public XMLCharacter[] xmlCharacter { get; set; }
    }

    public class XMLCharacter
    {
        [XmlElement("id")]
        public string id { get; set; }

        [XmlElement("imageSet")]
        public string imageSet { get; set; }

        [XmlElement("name")]
        public string name { get; set; }

        [XmlElement("level")]
        public int level { get; set; }

        [XmlElement("characterClass")]
        public string characterClass { get; set; }

        [XmlElement("baseHealth")]
        public double baseHealth { get; set; }

        [XmlElement("healthMult")]
        public double healthMult { get; set; }

        [XmlElement("resourceName")]
        public string resourceName { get; set; }

        [XmlElement("maxResource")]
        public int maxResource { get; set; }

        [XmlElement("basePhysAttack")]
        public double basePhysAttack { get; set; }

        [XmlElement("physAttackMult")]
        public double physAttackMult { get; set; }

        [XmlElement("baseMagicAttack")]
        public double baseMagicAttack { get; set; }

        [XmlElement("magicAttackMult")]
        public double magicAttackMult { get; set; }

        [XmlElement("basePhysArmor")]
        public double basePhysArmor { get; set; }

        [XmlElement("physArmorMult")]
        public double physArmorMult { get; set; }

        [XmlElement("baseMagicArmor")]
        public double baseMagicArmor { get; set; }

        [XmlElement("magicArmorMult")]
        public double magicArmorMult { get; set; }

        [XmlElement("weaknesses")]
        public string weaknesses { get; set; }

        [XmlElement("ability1Name")]
        public string ability1Name { get; set; }

        [XmlElement("ability2Name")]
        public string ability2Name { get; set; }

        [XmlElement("ability3Name")]
        public string ability3Name { get; set; }

        [XmlElement("baseSpeed")]
        public int baseSpeed { get; set; }

        [XmlElement("rarity")]
        public string rarity { get; set; }

        [XmlElement("acquired")]
        public bool acquired { get; set; }

        [XmlElement("aiName")]
        public string aiName { get; set; }

        [XmlElement("collectable")]
        public bool collectable { get; set; }
    }

    [XmlRoot("Encounters")]
    public class Encounters
    {
        [XmlElement("id")]
        public int id { get; set; }

        [XmlElement("name")]
        public string name { get; set; }

        [XmlElement("side")]
        public string side { get; set; }

        [XmlElement("Encounter")]
        public Encounter[] encounter { get; set; }
    }

    public class Encounter
    {
        [XmlElement("id")]
        public int id { get; set; }

        [XmlElement("name")]
        public string name { get; set; }

        [XmlElement("background1")]
        public string background1 { get; set; }

        [XmlElement("optional")]
        public bool optional { get; set; }

        [XmlElement("startdialog")]
        public string startdialog { get; set; }

        [XmlElement("Fights")]
        public Fights fights { get; set; }

        [XmlElement("enddialog")]
        public string enddialog { get; set; }
    }

    public class Fights
    {
        [XmlElement("Fight")]
        public Fight[] fight { get; set; }
    }

    public class Fight
    {
        [XmlElement("id")]
        public int id { get; set; }

        [XmlElement("dialog")]
        public string dialog { get; set; }

        [XmlElement("enemies")]
        public string enemies { get; set; }

        [XmlElement("amount")]
        public int amount { get; set; }

        [XmlElement("type")]
        public string type { get; set; }
    }
}
