using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace Portals_of_Madness
{
    public class XMLOperations
    {
        List<Ability> allAbilities;

        public XMLOperations()
        {
            allAbilities = new List<Ability>();
            XMLAbilities xabs = AbilityDeserializer($@"../../Abilities/Abilities.xml");
            foreach(XMLAbility xab in xabs.xmlAbility)
            {
                allAbilities.Add(convToAbility(xab));
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

        public Ability getAblilityByName(string abn)
        {
            foreach (Ability ab in allAbilities)
            {
                if (ab.name.Equals(abn))
                {
                    return ab;
                }
            }
            return null;
        }

        public Ability convToAbility(XMLAbility x)
        {
            return new Ability(x.name, x.cost, x.cooldown, x.physAttackDamage, x.magicAttackDamage,
                x.duration, x.damageType, x.target, x.targetCount, x.abilityType, x.modifier, x.modifiedAmount,
                x.imageIcon, x.sprite);
        }

        public Character convToCharacter(XMLCharacter x)
        {
            return new Character(x.imageSet, x.id, x.level, x.name, x.characterClass, x.baseHealth, x.healthMult,
                x.resourceName, x.maxResource, x.basePhysAttack, x.physAttackMult, x.baseMagicAttack,
                x.magicAttackMult, x.basePhysAttack, x.physAttackMult, x.baseMagicArmor,
                x.magicArmorMult, x.weaknesses,
                getAblilityByName(x.ability1Name), getAblilityByName(x.ability2Name), getAblilityByName(x.ability3Name),
                x.baseSpeed, x.rarity, x.collectable, x.aiName);
        }
    }
}
