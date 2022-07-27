﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Xml.Serialization;
using System.IO;

namespace Portals_of_Madness
{
    public class GameEngine
    {
        public int nextMap { get; set; }
        public List<Character> playerTeam { get; set; }
        public List<Character> enemyTeam { get; set; }
        public Mission mission { get; set; }
        public List<Character> initiativeTeam { get; set; }
        public List<Character> allCharacters { get; set; }
        public List<Ability> allAbilities { get; set; }
        public bool playerTurn { get; set; }
        public Character currentCharacter { get; set; }
        public int currentID { get; set; }
        public int encounterNumber { get; set; }
        public int turn { get; set; }
        public GameForm form { get; set; }
        public XMLOperations xmlOps { get; set; }

        //When called by GameForm after starting a new game
        public GameEngine(GameForm f)
        {
            nextMap = 0;
            DeXMLify();
            TutorialPTeamSetup();
            InitializeStuff(f);
        }

        //When called by GameForm after selecting a mission
        public GameEngine(GameForm f, List<Character> pT, int nM)
        {
            nextMap = nM;
            DeXMLify();
            playerTeam = pT;
            InitializeStuff(f);
        }

        public void DeXMLify()
        {
            xmlOps = new XMLOperations();
            allAbilities = new List<Ability>();
            try
            {
                XMLAbilities xabs = xmlOps.AbilityDeserializer($@"../../Abilities/Abilities.xml");
                foreach (XMLAbility xab in xabs.xmlAbility)
                {
                    allAbilities.Add(xmlOps.convToAbility(xab));
                }
            }
            catch { }
            allCharacters = new List<Character>();
            try
            {
                XMLCharacters xchs = xmlOps.CharacterDeserializer($@"../../Characters/Characters.xml");
                foreach(XMLCharacter xch in xchs.xmlCharacter)
                {
                    allCharacters.Add(xmlOps.convToCharacter(xch));
                }
            }
            catch { }
        }

        public void InitializeStuff(GameForm f)
        {
            form = f;
            playerTurn = false;
            currentID = -1;
            encounterNumber = 1;
            Setup();
        }

        public void TutorialPTeamSetup()
        {
            playerTeam = new List<Character>();
            playerTeam.Add(allCharacters.Where(a => a.name == "Eddie").Select(a => a).First());
            playerTeam.Add(allCharacters.Where(a => a.name == "Lance").Select(a => a).First());
            playerTeam.Add(allCharacters.Where(a => a.name == "Sam").Select(a => a).First());
        }

        public void Setup()
        {
            turn = 0;
            encounterNumber = 1;
            mission = new Mission(nextMap);
            form.InitializeUI(mission.PlayerSide());
            form.PlaceCharacters(playerTeam, mission.PlayerSide());
            if (mission.IsEncounter())
            {
                encounterSetup();
            }
            else
            {
                showResult(true);
            }
        }

        //Resets the enemies, puts them in their place, creates the order in which the characters get to go and starts the game
        public void encounterSetup()
        {
            enemyTeam = new List<Character>();
            mission.LoadNextEnemies();
            foreach(Character c in mission.Enemies)
            {
                enemyTeam.Add(c);
            }
            try
            {
                form.BackgroundImage =
                    Image.FromFile($@"../../Art/Backgrounds/{mission.encounters.encounter[mission.encounterNumber].background1}.png");
            }
            catch
            {
                try
                {
                    form.BackgroundImage =
                        Image.FromFile($@"../../Art/Backgrounds/{mission.encounters.encounter[mission.encounterNumber].background1}.jpg");
                }
                catch { }
            }
            string enemySide = (mission.PlayerSide().Equals("left") ? "right" : "left");
            form.PlaceCharacters(enemyTeam, enemySide);
            createInitiative();
            manage();
        }

        public void nextEncounter()
        {
            if (mission.IsEncounter())
            {
                encounterSetup();
            }
            else
            {
                showResult(true);
            }
        }

        //TODO: End screen after either winning or losing
        private void showResult(bool res)
        {
            foreach (Character c in playerTeam)
            {
                c.currHealth = c.maxHealth;
                c.alive = true;
                c.stunned = false;
                c.currResource = 0;
            }
        }

        public void manage()
        {
            if (bothTeamsAlive() == 0)
            {
                currentSelect();
                if (!playerTurn)
                {
                    form.abilityFrame.Visible = false;
                    currentCharacter.Act(playerTeam, enemyTeam);
                    manage();
                }
                else
                {
                    form.abilityFrame.Visible = true;
                    form.abilityFrame.UpdateButtons(currentCharacter);
                }
            }
            else if (bothTeamsAlive() == -1)
            {
                showResult(false);
            }
            else if (bothTeamsAlive() == 1)
            {
                mission.IncrementEncounterNumber();
                nextEncounter();
            }
        }

        private int selectTarget()
        {
            Random rand = new Random();
            int target = rand.Next(playerTeam.Count());
            if (!playerTeam[target].alive)
            {
                target = selectTarget();
            }
            return target;
        }

        private void newTurn()
        {
            turn++;
            foreach (Character c in initiativeTeam)
            {
                if (c.alive)
                {
                    foreach (DoT dot in c.dots)
                    {
                        c.currHealth -= dot.amount;
                        dot.Tick();
                        if (dot.duration <= 0)
                        {
                            c.removeDoT(dot);
                        }
                    }

                    foreach (Buff buff in c.buffs)
                    {
                        buff.Tick();
                        if (buff.duration <= 0)
                        {
                            c.removeBuffEffects(buff);
                            c.removeBuff(buff);
                        }
                    }

                    if (c.currHealth <= 0)
                    {
                        c.die();
                    }
                    if (c.currHealth > c.maxHealth)
                    {
                        c.currHealth = c.maxHealth;
                    }
                    resourceGain(c);
                }
            }
        }

        private void resourceGain(Character c)
        {
            int gain = 3;

            switch (c.resourceName)
            {
                case "mana":
                    gain = 4;
                    break;
                case "rage":
                    if (c.currHealth < c.maxHealth * 0.3)
                    {
                        gain = 8;
                    }
                    else if (c.currHealth < c.maxHealth * 0.7)
                    {
                        gain = 4;
                    }
                    else
                    {
                        gain = 2;
                    }
                    break;
                case "focus":
                    if (c.currHealth < c.maxHealth * 0.3)
                    {
                        gain = 2;
                    }
                    else if (c.currHealth < c.maxHealth * 0.7)
                    {
                        gain = 4;
                    }
                    else
                    {
                        gain = 8;
                    }
                    break;
                default:
                    break;
            }

            if (c.currResource + gain <= c.maxResource)
            {
                c.currResource += gain;
            }
            else
            {
                c.currResource = c.maxResource;
            }
        }

        private void currentSelect()
        {
            if (currentID == -1 || (currentID + 1) >= initiativeTeam.Count())
            {
                currentID = 0;
                newTurn();
            }
            else
            {
                currentID++;
            }
            if (!initiativeTeam[currentID].alive)
            {
                currentSelect();
            }
            if (initiativeTeam[currentID].stunned)
            {
                --initiativeTeam[currentID].stunLength;
                if(initiativeTeam[currentID].stunLength <= 0)
                {
                    initiativeTeam[currentID].stunned = false;
                }
                currentSelect();
            }
            currentCharacter = initiativeTeam[currentID];
            playerTurn = false;
            foreach (Character P in playerTeam)
            {
                if (P.name.Equals(currentCharacter.name))
                {
                    playerTurn = true;
                }
            }
        }

        private int bothTeamsAlive()
        {
            bool playerAlive = false;
            bool enemyAlive = false;
            foreach (Character P in playerTeam)
            {
                if (P.currHealth > 0)
                {
                    playerAlive = true;
                }
            }
            foreach (Character E in enemyTeam)
            {
                if (E.currHealth > 0)
                {
                    enemyAlive = true;
                }
            }
            if (playerAlive && enemyAlive)
            {
                return 0;
            }
            else if (!playerAlive && enemyAlive)
            {
                return -1;
            }
            else if (playerAlive && !enemyAlive)
            {
                return 1;
            }
            return -1;
        }

        private void createInitiative()
        {
            currentID = -1;
            initiativeTeam = new List<Character>();
            initiativeTeam.AddRange(playerTeam);
            initiativeTeam.AddRange(enemyTeam);
            Character temp;
            for (int i = 0; i < initiativeTeam.Count() - 1; i++)
            {
                for (int j = i + 1; j < initiativeTeam.Count(); j++)
                {
                    if (initiativeTeam[j].speed <= initiativeTeam[i].speed)
                    {
                        temp = initiativeTeam[i];
                        initiativeTeam[i] = initiativeTeam[j];
                        initiativeTeam[j] = temp;
                    }
                }
            }
        }
    }
}
