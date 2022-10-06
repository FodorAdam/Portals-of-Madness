﻿using System;
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

        public Mission(Controller c, int mission, int encounter)
        {
            Enemies = new List<Character>();
            XMLOps = c.XMLOperations;

            string path = $@"../../Missions/{mission}/Mission.xml";
            try
            {
                EncounterContainer = XMLOps.MissionDeserializer(path);
            }
            catch
            {
                Console.WriteLine($"{mission}/Mission.xml not found!");
            }

            EveryAbility = new List<Ability>();
            try
            {
                XMLAbilities xabs = XMLOps.AbilityDeserializer($@"../../Abilities/Abilities.xml");
                foreach (XMLAbility xab in xabs.XmlAbility)
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

            EncounterNumber = encounter;
            FightNumber = 0;
        }

        public Ability ConvertToAbility(XMLAbility x)
        {
            return new Ability(x.Name, x.Cost, x.Cooldown, x.PhysAttackDamage, x.MagicAttackDamage,
                x.Duration, x.DamageType, x.Target, x.TargetCount, x.AbilityType, x.Modifier, x.ModifiedAmount,
                x.ImageIcon, x.Sprite);
        }

        public void LoadNextEnemies()
        {
            Enemies.Clear();
            if (EncounterContainer.Encounter[EncounterNumber].Fights.Fight[FightNumber].Type.Equals("normal"))
            {
                for (int i = 0; i < EncounterContainer.Encounter[EncounterNumber].Fights.Fight[FightNumber].Amount; i++)
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
            var cEnum = XMLCharacterList.XmlCharacter.Where(a => a.Id.Contains(n)).Select(a => a);
            List<XMLCharacter> cList = new List<XMLCharacter>();
            cList.AddRange(cEnum);
            int i = Rand.Next(0, cList.Count());
            return XMLOps.ConvertToCharacter(cList[i]);
        }

        public Character SelectAICharacter()
        {
            switch (EncounterContainer.Encounter[EncounterNumber].Fights.Fight[FightNumber].Enemies)
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
            switch (EncounterContainer.Encounter[EncounterNumber].Fights.Fight[FightNumber].Enemies)
            {
                case "prisonBoss":
                    characters.Add(SelectCharacter("cityguard"));
                    characters.Add(SelectCharacter("warden"));
                    characters.Add(SelectCharacter("cityguard"));
                    break;
                case "ratThief":
                    characters.Add(SelectCharacter("clumsyEn"));
                    break;
                case "townCouncil":
                    characters.Add(SelectCharacter("cityguard"));
                    characters.Add(SelectCharacter("maxwellEn"));
                    characters.Add(SelectCharacter("godfrey"));
                    characters.Add(SelectCharacter("cityguard"));
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
            if(EncounterContainer.Encounter.Length - 1 < EncounterNumber)
            {
                return false;
            }
            return FightNumber < EncounterContainer.Encounter[EncounterNumber].Fights.Fight.Length;
        }

        //Returns the side the player is on
        public string PlayerSide()
        {
            return EncounterContainer.Side;
        }

        public void IncrementEncounterNumber()
        {
            ++EncounterNumber;
            FightNumber = 1;
        }
    }
}
