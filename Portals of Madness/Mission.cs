using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Portals_of_Madness
{
    public class Mission
    {
        public string name { get; set; }
        public List<Character> Enemies { get; set; }
        public List<Ability> EveryAbility { get; set; }
        public XMLCharacters XMLCharacterList { get; set; }
        public string background { get; set; }
        public Encounters encounters { get; set; }
        public int encounterNumber { get; set; }
        public int fightNumber { get; set; }
        public XMLOperations xmlOps { get; set; }
        private static Random rand = new Random();

        public Mission(int number)
        {
            Enemies = new List<Character>();
            xmlOps = new XMLOperations();

            string path = $@"../../Missions/Mission{number}.xml";
            try
            {
                encounters = xmlOps.MissionDeserializer(path);
            }
            catch { }
            EveryAbility = new List<Ability>();
            try
            {
                XMLAbilities xabs = xmlOps.AbilityDeserializer($@"../../Abilities/Abilities.xml");
                foreach (XMLAbility xab in xabs.xmlAbility)
                {
                    EveryAbility.Add(convToAbility(xab));
                }
            }
            catch { }
            try
            {
                XMLCharacterList = xmlOps.CharacterDeserializer($@"../../Characters/Characters.xml");
            }
            catch { }
            encounterNumber = 0;
            fightNumber = 0;
        }

        public Ability convToAbility(XMLAbility x)
        {
            return new Ability(x.name, x.cost, x.cooldown, x.physAttackDamage, x.magicAttackDamage,
                x.duration, x.damageType, x.target, x.targetCount, x.abilityType, x.modifier, x.modifiedAmount,
                x.imageIcon, x.sprite);
        }

        public void LoadNextEnemies()
        {
            Enemies.Clear();
            if (encounters.encounter[encounterNumber].fights.fight[fightNumber].type.Equals("normal"))
            {
                for (int i = 0; i < encounters.encounter[encounterNumber].fights.fight[fightNumber].amount; i++)
                {
                    Enemies.Add(SelectAICharacter());
                }
            }
            else
            {
                Enemies.AddRange(selectBossFight());
            }
            ++fightNumber;
        }

        public Character SelectCharacter(string n)
        {
            var cEnum = XMLCharacterList.xmlCharacter.Where(a => a.id.Contains("prisoner")).Select(a => a);
            List<XMLCharacter> cList = new List<XMLCharacter>();
            cList.AddRange(cEnum);
            int i = rand.Next(0, cList.Count());
            return xmlOps.convToCharacter(cList[i]);
        }

        public Character SelectAICharacter()
        {
            switch (encounters.encounter[encounterNumber].fights.fight[fightNumber].enemies)
            {
                case "prisonPack":
                    return SelectCharacter("prisoner");
                case "crazedCitizenPack":
                    break;
                case "guardPack":
                    return SelectCharacter("cityguard");
                default:
                    break;
            }
            return null;
        }

        public List<Character> selectBossFight()
        {
            List<Character> characters = new List<Character>();
            switch (encounters.encounter[encounterNumber].fights.fight[fightNumber].enemies)
            {
                case "prisonBoss":
                    characters.Add(SelectCharacter("prisoner"));
                    characters.Add(SelectCharacter("warden"));
                    characters.Add(SelectCharacter("prisoner"));
                    break;
                case "ratThief":
                    break;
                case "townCouncil":
                    break;
                default:
                    break;
            }
            return characters;
        }

        //Checks if there is an encounter remaining
        public bool IsEncounter()
        {
            Enemies = new List<Character>();
            return encounterNumber < encounters.encounter.Length;
        }

        //Returns the side the player is on
        public string PlayerSide()
        {
            return encounters.side;
        }

        public void IncrementEncounterNumber()
        {
            ++encounterNumber;
            fightNumber = 1;
        }
    }
}
