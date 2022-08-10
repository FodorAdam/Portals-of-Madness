using System;
using System.Collections.Generic;
using System.Linq;

namespace Portals_of_Madness
{
    public class Mission
    {
        public List<Character> Enemies { get; set; }
        public List<Ability> EveryAbility { get; set; }
        public XMLCharacters XMLCharacterList { get; set; }
        public Encounters EncounterContainer { get; set; }
        public int EncounterNumber { get; set; }
        public int FightNumber { get; set; }
        public XMLOperations XMLOps { get; set; }
        private static readonly Random Rand = new Random();

        public Mission(int number)
        {
            Enemies = new List<Character>();
            XMLOps = new XMLOperations();

            string path = $@"../../Missions/Mission{number}.xml";
            try
            {
                EncounterContainer = XMLOps.MissionDeserializer(path);
            }
            catch
            {
                Console.WriteLine($"Mission{number}.xml not found!");
            }

            EveryAbility = new List<Ability>();
            try
            {
                XMLAbilities xabs = XMLOps.AbilityDeserializer($@"../../Abilities/Abilities.xml");
                foreach (XMLAbility xab in xabs.xmlAbility)
                {
                    EveryAbility.Add(ConvertToAbility(xab));
                }
            }
            catch
            {
                Console.WriteLine($"Abilities.xml not found!");
            }

            try
            {
                XMLCharacterList = XMLOps.CharacterDeserializer($@"../../Characters/Characters.xml");
            }
            catch
            {
                Console.WriteLine($"Characters.xml not found!");
            }

            EncounterNumber = 0;
            FightNumber = 0;
        }

        public Ability ConvertToAbility(XMLAbility x)
        {
            return new Ability(x.name, x.cost, x.cooldown, x.physAttackDamage, x.magicAttackDamage,
                x.duration, x.damageType, x.target, x.targetCount, x.abilityType, x.modifier, x.modifiedAmount,
                x.imageIcon, x.sprite);
        }

        public void LoadNextEnemies()
        {
            Enemies.Clear();
            if (EncounterContainer.encounter[EncounterNumber].fights.fight[FightNumber].type.Equals("normal"))
            {
                for (int i = 0; i < EncounterContainer.encounter[EncounterNumber].fights.fight[FightNumber].amount; i++)
                {
                    Enemies.Add(SelectAICharacter());
                }
            }
            else
            {
                Enemies.AddRange(SelectBossFight());
            }
            ++FightNumber;
        }

        public Character SelectCharacter(string n)
        {
            var cEnum = XMLCharacterList.xmlCharacter.Where(a => a.id.Contains(n)).Select(a => a);
            List<XMLCharacter> cList = new List<XMLCharacter>();
            cList.AddRange(cEnum);
            int i = Rand.Next(0, cList.Count());
            return XMLOps.ConvertToCharacter(cList[i]);
        }

        public Character SelectAICharacter()
        {
            switch (EncounterContainer.encounter[EncounterNumber].fights.fight[FightNumber].enemies)
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

        public List<Character> SelectBossFight()
        {
            List<Character> characters = new List<Character>();
            switch (EncounterContainer.encounter[EncounterNumber].fights.fight[FightNumber].enemies)
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
            Enemies.Clear();
            if(EncounterContainer.encounter.Length - 1 < EncounterNumber)
            {
                return false;
            }
            return FightNumber < EncounterContainer.encounter[EncounterNumber].fights.fight.Length;
        }

        //Returns the side the player is on
        public string PlayerSide()
        {
            return EncounterContainer.side;
        }

        public void IncrementEncounterNumber()
        {
            ++EncounterNumber;
            FightNumber = 1;
        }
    }
}
