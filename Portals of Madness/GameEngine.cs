using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Portals_of_Madness
{
    public class GameEngine
    {
        public int NextMap { get; set; }
        public List<Character> PlayerTeam { get; set; }
        public List<Character> EnemyTeam { get; set; }
        public Mission CurrentMission { get; set; }
        public List<Character> InitiativeTeam { get; set; }
        public List<Character> AllCharacters { get; set; }
        public List<Ability> AllAbilities { get; set; }
        public bool PlayerTurn { get; set; }
        public Character CurrentCharacter { get; set; }
        public int CurrentID { get; set; }
        public int Turn { get; set; }
        public GameForm Form { get; set; }
        public XMLOperations XMLOps { get; set; }

        //When called by GameForm after starting a new game
        public GameEngine(GameForm f)
        {
            NextMap = 0;
            DeXMLify();
            TutorialPTeamSetup();
            InitializeStuff(f);
        }

        //When called by GameForm after selecting a mission
        public GameEngine(GameForm f, List<Character> pT, int nM)
        {
            NextMap = nM;
            DeXMLify();
            PlayerTeam = pT;
            InitializeStuff(f);
        }

        public void DeXMLify()
        {
            XMLOps = new XMLOperations();
            AllAbilities = new List<Ability>();
            try
            {
                XMLAbilities xabs = XMLOps.AbilityDeserializer($@"../../Abilities/Abilities.xml");
                foreach (XMLAbility xab in xabs.xmlAbility)
                {
                    AllAbilities.Add(XMLOps.ConvertToAbility(xab));
                }
            }
            catch
            {
                Console.WriteLine("Abilities.xml not found.");
            }
            AllCharacters = new List<Character>();
            try
            {
                XMLCharacters xchs = XMLOps.CharacterDeserializer($@"../../Characters/Characters.xml");
                foreach(XMLCharacter xch in xchs.xmlCharacter)
                {
                    AllCharacters.Add(XMLOps.ConvertToCharacter(xch));
                }
            }
            catch
            {
                Console.WriteLine("Characters.xml not found.");
            }
        }

        public void InitializeStuff(GameForm f)
        {
            InitiativeTeam = new List<Character>();
            Form = f;
            PlayerTurn = false;
            CurrentID = -1;
            Setup();
        }

        public void TutorialPTeamSetup()
        {
            PlayerTeam = new List<Character>
            {
                AllCharacters.Where(a => a.Name == "Eddie").Select(a => a).First(),
                AllCharacters.Where(a => a.Name == "Lance").Select(a => a).First(),
                AllCharacters.Where(a => a.Name == "Sam").Select(a => a).First()
            };
        }

        public void Setup()
        {
            Turn = 0;
            CurrentMission = new Mission(NextMap);
            Form.InitializeUI(CurrentMission.PlayerSide());
            Form.PlaceCharacters(PlayerTeam, CurrentMission.PlayerSide());
            if (CurrentMission.IsEncounter())
            {
                EncounterSetup();
            }
            else
            {
                ShowResult(true);
            }
        }

        //Resets the enemies, puts them in their place, creates the order in which the characters get to go and starts the game
        public void EncounterSetup()
        {
            EnemyTeam = new List<Character>();
            CurrentMission.LoadNextEnemies();
            foreach(Character c in CurrentMission.Enemies)
            {
                EnemyTeam.Add(c);
            }
            try
            {
                Form.BackgroundImage =
                    Image.FromFile($@"../../Art/Backgrounds/{CurrentMission.EncounterContainer.encounter[CurrentMission.EncounterNumber].background1}.jpg");
            }
            catch
            {
                Console.WriteLine($"{CurrentMission.EncounterContainer.encounter[CurrentMission.EncounterNumber].background1} not found!");
            }
            string enemySide = (CurrentMission.PlayerSide().Equals("left") ? "right" : "left");
            Form.PlaceCharacters(EnemyTeam, enemySide);
            CreateInitiative();
            Manage();
        }

        public void NextFight()
        {
            if(CurrentMission.IsEncounter())
            {
                EncounterSetup();
            }
            else
            {
                CurrentMission.IncrementEncounterNumber();
                ShowResult(true);
            }
        }

        //End screen after either winning or losing
        private void ShowResult(bool res)
        {
            Form.ShowResultButton(res);
        }

        public void Manage()
        {
            if(AreBothTeamsAlive() == 0)
            {
                SelectCurrentCharacter();
                if(!PlayerTurn)
                {
                    Form.AbilityFrame.Visible = false;
                    Form.CharacterFrame.Visible = false;
                    CurrentCharacter.Act(PlayerTeam, EnemyTeam);
                    Form.UpdateCharacterBars();
                    Manage();
                }
                else
                {
                    Form.AbilityFrame.Visible = true;
                    Form.CharacterFrame.Visible = true;
                    Form.AbilityFrame.UpdateButtons(CurrentCharacter);
                    Form.AssignAbilityButtonFunctions();
                    Form.CharacterFrame.UpdateFrame(CurrentCharacter);
                }
            }
            else if (AreBothTeamsAlive() == -1)
            {
                ShowResult(false);
            }
            else if (AreBothTeamsAlive() == 1)
            {
                NextFight();
            }
        }

        private void StartNewTurn()
        {
            Turn++;
            foreach (Character c in InitiativeTeam)
            {
                if (c.Alive)
                {
                    foreach (DoT dot in c.DoTs)
                    {
                        c.CurrentHealth -= dot.Amount;
                        dot.Tick();
                        if (dot.Duration <= 0)
                        {
                            c.RemoveDoT(dot);
                        }
                    }

                    foreach (Buff buff in c.Buffs)
                    {
                        buff.Tick();
                        if (buff.Duration <= 0)
                        {
                            c.RemoveBuffEffects(buff);
                            c.RemoveBuff(buff);
                        }
                    }

                    if (c.CurrentHealth <= 0)
                    {
                        c.Die();
                    }

                    if (c.CurrentHealth > c.MaxHealth)
                    {
                        c.CurrentHealth = c.MaxHealth;
                    }
                    GainResources(c);
                }
            }
        }

        private void GainResources(Character c)
        {
            int gain = 3;

            switch (c.ResourceName)
            {
                case "mana":
                    gain = 4;
                    break;
                case "rage":
                    if (c.CurrentHealth < c.MaxHealth * 0.3)
                    {
                        gain = 8;
                    }
                    else if (c.CurrentHealth < c.MaxHealth * 0.7)
                    {
                        gain = 4;
                    }
                    else
                    {
                        gain = 2;
                    }
                    break;
                case "focus":
                    if (c.CurrentHealth < c.MaxHealth * 0.3)
                    {
                        gain = 2;
                    }
                    else if (c.CurrentHealth < c.MaxHealth * 0.7)
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

            if (c.CurrentResource + gain <= c.MaxResource)
            {
                c.CurrentResource += gain;
            }
            else
            {
                c.CurrentResource = c.MaxResource;
            }
        }

        private void SelectCurrentCharacter()
        {
            ++CurrentID;

            if (CurrentID == InitiativeTeam.Count())
            {
                CurrentID = 0;
                StartNewTurn();
            }

            if (!InitiativeTeam[CurrentID].Alive)
            {
                SelectCurrentCharacter();
            }

            if (InitiativeTeam[CurrentID].Stunned)
            {
                --InitiativeTeam[CurrentID].StunLength;
                if(InitiativeTeam[CurrentID].StunLength <= 0)
                {
                    InitiativeTeam[CurrentID].Stunned = false;
                }
                SelectCurrentCharacter();
            }

            CurrentCharacter = InitiativeTeam[CurrentID];
            PlayerTurn = false;
            foreach (Character P in PlayerTeam)
            {
                if(P.Equals(CurrentCharacter))
                {
                    PlayerTurn = true;
                }
            }
        }

        //Determines wether both teams have at least one member alive, return 0 if yes,
        //returns 1 if all enemies are dead, -1 if the player is dead
        private int AreBothTeamsAlive()
        {
            bool playerAlive = false;
            bool enemyAlive = false;
            foreach (Character P in PlayerTeam)
            {
                if (P.CurrentHealth > 0)
                {
                    playerAlive = true;
                }
            }
            foreach (Character E in EnemyTeam)
            {
                if (E.CurrentHealth > 0)
                {
                    enemyAlive = true;
                }
            }
            if (playerAlive && enemyAlive)
            {
                return 0;
            }
            else if (playerAlive && !enemyAlive)
            {
                return 1;
            }
            return -1;
        }

        private void CreateInitiative()
        {
            CurrentID = -1;
            InitiativeTeam.Clear();
            InitiativeTeam.AddRange(PlayerTeam);
            InitiativeTeam.AddRange(EnemyTeam);
            Character tmp;
            for (int i = 0; i < InitiativeTeam.Count() - 1; i++)
            {
                for (int j = i + 1; j < InitiativeTeam.Count(); j++)
                {
                    if (InitiativeTeam[j].Speed <= InitiativeTeam[i].Speed)
                    {
                        tmp = InitiativeTeam[i];
                        InitiativeTeam[i] = InitiativeTeam[j];
                        InitiativeTeam[j] = tmp;
                    }
                }
            }
        }
    }
}
