using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    public class GameEngine
    {
        public int NextMap { get; set; }
        public int NextEncounter { get; set; }
        public List<Character> PlayerTeam { get; set; }
        public List<Character> EnemyTeam { get; set; }
        public Mission CurrentMission { get; set; }
        public List<Character> InitiativeTeam { get; set; }
        public List<Character> AllCharacters { get; set; }
        public List<Ability> AllAbilities { get; set; }
        public bool PlayerTurn { get; set; }
        public Character CurrentCharacter { get; set; }
        public int CurrentID { get; set; }
        public int TurnNumber { get; set; }
        public GamePanel Panel { get; set; }
        public Controller Controller { get; set; }
        public bool EndRun { get; set; }

        //When called by GameForm after starting a new game
        public GameEngine(Controller c, GamePanel p, int nM, int nE)
        {
            Controller = c;
            NextMap = nM;
            NextEncounter = nE;
            DeXMLify();
            TutorialPTeamSetup();
            InitializeStuff(p);
        }

        //When called by GameForm after selecting a mission
        public GameEngine(Controller c, GamePanel p, int nM, int nE, List<Character> pT)
        {
            Controller = c;
            NextMap = nM;
            NextEncounter = nE;
            DeXMLify();
            PlayerTeam = pT;
            InitializeStuff(p);
        }

        public void DeXMLify()
        {
            AllAbilities = Controller.XMLOperations.AllAbilities;
            AllCharacters = new List<Character>();
            try
            {
                XMLCharacters xchs = (XMLCharacters)Controller.XMLOperations.GenericDeserializer<XMLCharacters>($@"../../Characters/Characters.xml");
                foreach(XMLCharacter xch in xchs.XmlCharacter)
                {
                    AllCharacters.Add(Controller.XMLOperations.ConvertToCharacter(xch));
                }
            }
            catch
            {
                Console.WriteLine("Characters.xml not found.");
            }
        }

        public void InitializeStuff(GamePanel p)
        {
            EndRun = false;
            Panel = p;
            PlayerTurn = false;
            CurrentID = -1;
            InitiativeTeam = new List<Character>();
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
            TurnNumber = 0;
            CurrentMission = new Mission(Controller, NextMap, NextEncounter);
            Panel.PlaceCharacters(PlayerTeam, CurrentMission.PlayerSide());
            Panel.InitializeUI(CurrentMission.PlayerSide());
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
            int tmp = PlayerTeam.Count();
            bool b = Controller.GEH.EncounterStartEvents(NextMap, NextEncounter, PlayerTeam, EnemyTeam);
            if (b && tmp > PlayerTeam.Count())
            {
                Panel.DisplaceCharacters(PlayerTeam, CurrentMission.PlayerSide());
                b = false;
            }
            else if (b && tmp < PlayerTeam.Count())
            {
                Panel.AddCharacter(PlayerTeam.Last(), CurrentMission.PlayerSide(), PlayerTeam.Count() - 1);
                b = false;
            }

            try
            {
                Panel.BackgroundImage =
                    Image.FromFile($@"../../Art/Backgrounds/{CurrentMission.EncounterContainer.Encounter[CurrentMission.EncounterNumber].Background1}.png");
                Panel.BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch
            {
                Console.WriteLine($"{CurrentMission.EncounterContainer.Encounter[CurrentMission.EncounterNumber].Background1} not found!");
            }
            string enemySide = (CurrentMission.PlayerSide().Equals("left") ? "right" : "left");
            Panel.PlaceCharacters(EnemyTeam, enemySide);
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
                ShowResult(true);
            }
        }

        //End screen after either winning or losing
        private void ShowResult(bool res)
        {
            Panel.ShowResultButton(res);
        }

        public void Manage()
        {
            Panel.UpdateCharacterBars();
            if (AreBothTeamsAlive() == 0)
            {
                bool currentCharacterSet = false;
                while(!currentCharacterSet)
                {
                    currentCharacterSet = SelectCurrentCharacter();
                }

                if(!PlayerTurn)
                {
                    Panel.AbilityFrame.SetVisibility(false);
                    ActionEventHandler(CurrentCharacter.Act(PlayerTeam, EnemyTeam), CurrentCharacter);
                    Manage();
                }
                else
                {
                    Panel.AbilityFrame.SetVisibility(true);
                    Panel.AbilityFrame.UpdateButtons(CurrentCharacter);
                    Panel.CharacterFrame.UpdateFrame(CurrentCharacter, Controller);
                }
            }
            else if (AreBothTeamsAlive() == -1)
            {
                ShowResult(false);
            }
            else if (AreBothTeamsAlive() == 1)
            {
                Console.WriteLine("----------------Enemies Defeated----------------");
                StartNewTurn();
                NextFight();
            }
        }

        private void StartNewTurn()
        {
            TurnNumber++;
            Console.WriteLine($"----------------Turn {TurnNumber}----------------");
            foreach (Character c in InitiativeTeam)
            {
                if (c.Alive)
                {
                    foreach (DoT dot in c.DoTs)
                    {
                        c.CurrentHealth += dot.Amount;
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
            Panel.UpdateCharacterBars();
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
                case "energy":
                    gain = 5;
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

        private bool SelectCurrentCharacter()
        {
            ++CurrentID;

            if (CurrentID == InitiativeTeam.Count())
            {
                CurrentID = 0;
                StartNewTurn();
            }

            if (!InitiativeTeam[CurrentID].Alive)
            {
                return false;
            }

            if (InitiativeTeam[CurrentID].WasStunned)
            {
                InitiativeTeam[CurrentID].WasStunned = false;
                StunEventHandler($"{InitiativeTeam[CurrentID].Name} is no longer winded");
                Panel.UpdateCharacterBars();
            }

            if (InitiativeTeam[CurrentID].Stunned)
            {
                --InitiativeTeam[CurrentID].StunLength;
                if(InitiativeTeam[CurrentID].StunLength <= 0)
                {
                    InitiativeTeam[CurrentID].Stunned = false;
                    InitiativeTeam[CurrentID].WasStunned = true;
                    StunEventHandler($"{InitiativeTeam[CurrentID].Name} is no longer stunned");
                    Panel.UpdateCharacterBars();
                }
                return false;
            }

            CurrentCharacter = InitiativeTeam[CurrentID];
            PlayerTurn = false;
            if(CurrentCharacter.AIType == "none")
            {
                PlayerTurn = true;
            }
                
            Console.WriteLine($"({CurrentID}) {CurrentCharacter.Name}");
            return true;
        }

        public void StunEventHandler(string msg)
        {
            Panel.ActionLabel.Visible = true;
            Panel.ActionLabel.Text = msg;
            Application.DoEvents();
            Thread.Sleep(new TimeSpan(0, 0, 0, 1, 500));
            Panel.ActionLabel.Visible = false;
            Application.DoEvents();
        }

        public void ActionEventHandler(string msg, Character actor)
        {
            Panel.ActionLabel.Visible = true;
            Panel.ActionLabel.Text = msg;
            actor.SetImageToAttack();
            Panel.UpdateCharacterImages(actor);
            Application.DoEvents();
            Thread.Sleep(new TimeSpan(0, 0, 0, 1, 500));
            Panel.ActionLabel.Visible = false;
            actor.SetImageToBase();
            Panel.UpdateCharacterImages(actor);
            Application.DoEvents();
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
            EndRun = true;
            return -1;
        }

        private void CreateInitiative()
        {
            CurrentID = -1;
            InitiativeTeam.Clear();
            List<Character> tmpList = new List<Character>();
            tmpList.AddRange(PlayerTeam);
            tmpList.AddRange(EnemyTeam);
            InitiativeTeam = tmpList.OrderBy(o => o.Speed).ToList();

            Console.WriteLine("--------------Initiative Team Start--------------");
            foreach (Character E in InitiativeTeam)
            {
                Console.WriteLine($"{E.Name}");
            }
            Console.WriteLine("---------------Initiative Team End---------------");
        }
    }
}
