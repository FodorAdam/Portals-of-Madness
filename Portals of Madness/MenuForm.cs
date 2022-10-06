using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    public partial class MenuForm : Form
    {
        private readonly Controller Controller;
        public GamePanel Game;
        private readonly MapSelectionPanel MapSelection;
        private readonly CharacterSelectionPanel CharacterSelection;
        private readonly DialogPanel DialogP;
        private readonly MenuButtonsPanel Buttons;
        private readonly InfoPanel Info;
        public int MissionNumber { get; set; }
        public int MaxEncounterNumber { get; set; }
        public int EncounterNumber { get; set; }
        public bool NewGame { get; set; }

        public MenuForm(Controller c)
        {
            InitializeComponent();

            Controller = c;
            Controller.SetFormResolution(this);
            MissionNumber = 0;
            EncounterNumber = 0;
            NewGame = false;

            MapSelection = new MapSelectionPanel(Controller);
            MapSelection.Hide();
            Controls.Add(MapSelection);
            MapSelection.ButtonStartMission.Click += ButtonMission_Click;
            MapSelection.ButtonReturn.Click += BackToMenu_Click;

            CharacterSelection = new CharacterSelectionPanel(Controller);
            CharacterSelection.Hide();
            Controls.Add(CharacterSelection);
            CharacterSelection.ButtonReturn.Click += BackToMenu_Click;
            CharacterSelection.ButtonStartMission.Click += StartMissionOrDialog_Click;

            DialogP = new DialogPanel(Controller);
            DialogP.Hide();
            Controls.Add(DialogP);
            DialogP.ButtonStart.Click += StartMission_Click;
            DialogP.ButtonFirstStart.Click += StartNewGame_Click;
            DialogP.ButtonBack.Click += ButtonMapsFromEnd_Click;
            DialogP.ButtonCont.Click += ContinueIfLastHadEndingDialog_Click;

            Buttons = new MenuButtonsPanel(Controller);
            Controls.Add(Buttons);
            Buttons.ButtonNewGame.Click += ButtonNewGame_Click;
            Buttons.ButtonContinue.Click += ButtonContinue_Click;
            Buttons.ButtonInfo.Click += ButtonInfo_Click;

            Info = new InfoPanel(Controller);
            Info.Hide();
            Controls.Add(Info);
            Info.ButtonBack.Click += BackToMenu_Click;
        }

        //Start a new game, show dialog
        private void ButtonNewGame_Click(object sender, EventArgs e)
        {
            Buttons.Hide();
            MaxEncounterNumber = 6;
            NewGame = true;
            DialogP.StartFirst(true);
            if (DialogP.SetupDialog(0, 0, "Pre", ""))
            {
                DialogP.Show();
            }
        }

        //Starting the tutorial mission from dialog window
        private void StartNewGame_Click(object sender, EventArgs e)
        {
            DialogP.Hide();
            Game = new GamePanel(Controller, 0, 0);
            Game.ResultButton.Click += BackToMenuFromGame_Click;
            Controls.Add(Game);
        }

        //Start a mission or the associated dialog
        private void StartMissionOrDialog_Click(object sender, EventArgs e)
        {
            if (CharacterSelection.GetSelectedCharacterList().Count > 0)
            {
                CharacterSelection.Hide();
                if (DialogP.SetupDialog(MissionNumber, EncounterNumber, "Pre", ""))
                {
                    DialogP.Show();
                }
                else
                {
                    List<Character> Characters = CharacterSelection.GetSelectedCharacterList();
                    foreach(Character c in Characters)
                    {
                        c.Reset();
                    }
                    Game = new GamePanel(Controller, MissionNumber, EncounterNumber, Characters);
                    Game.ResultButton.Click += BackToMenuFromGame_Click;
                    Controls.Add(Game);
                }
            }
        }

        //Start mission from dialog window
        private void StartMission_Click(object sender, EventArgs e)
        {
            DialogP.Hide();
            List<Character> Characters = CharacterSelection.GetSelectedCharacterList();
            if(NewGame)
            {
                Characters = Game.GetRefreshedPlayerCharacters();
            }
            foreach (Character c in Characters)
            {
                c.Reset();
            }
            Game = new GamePanel(Controller, MissionNumber, EncounterNumber, Characters);
            Game.ResultButton.Click += BackToMenuFromGame_Click;
            Controls.Add(Game);
        }

        //Continue the game from save
        private void ButtonContinue_Click(object sender, EventArgs e)
        {
            NewGame = false;
            Buttons.Hide();
            MapSelection.Show();
        }

        //Continue the game from save
        private void ButtonMapsFromEnd_Click(object sender, EventArgs e)
        {
            NewGame = false;
            DialogP.Hide();
            MapSelection.Show();
        }

        //Select the mission
        private void ButtonMission_Click(object sender, EventArgs e)
        {
            MissionNumber = MapSelection.GetMissionNumber();
            MaxEncounterNumber = MapSelection.GetMaxEncounterNumber();
            EncounterNumber = MapSelection.GetEncounterNumber();
            NewGame = false;
            MapSelection.Hide();
            CharacterSelection.Show();
        }

        //Show the how to play form
        private void ButtonInfo_Click(object sender, EventArgs e)
        {
            Buttons.Hide();
            Info.Show();
        }

        //Returns to the main menu
        private void BackToMenu_Click(object sender, EventArgs e)
        {
            DialogP.Hide();
            MapSelection.Hide();
            CharacterSelection.Hide();
            Info.Hide();
            Buttons.Show();
        }

        //Start next mission if one is still availabe, return to mapselection if not or if the player died
        private void BackToMenuFromGame_Click(object sender, EventArgs e)
        {
            Game.Hide();
            if (!Game.Engine.EndRun)
            {
                if (DialogP.SetupDialog(MissionNumber, EncounterNumber, "Post", ""))
                {
                    if (EncounterNumber + 1 >= MaxEncounterNumber)
                    {
                        DialogP.EndLast(true);
                    }
                    else
                    {
                        DialogP.ContinueWithNext(true);
                    }

                    DialogP.Show();
                }
                else if (NextMission())
                {
                    ++EncounterNumber;

                    if (DialogP.SetupDialog(MissionNumber, EncounterNumber, "Pre", ""))
                    {
                        DialogP.Show();
                    }
                    else
                    {
                        List<Character> Characters = Game.GetRefreshedPlayerCharacters();
                        Game = new GamePanel(Controller, MissionNumber, EncounterNumber, Characters);
                        Game.ResultButton.Click += BackToMenuFromGame_Click;
                        Controls.Add(Game);
                    }
                }
                else
                {
                    MapSelection.Show();
                }
            }
            else
            {
                MapSelection.Show();
            }
        }

        private void ContinueIfLastHadEndingDialog_Click(object sender, EventArgs e)
        {
            DialogP.Hide();
            if (NextMission())
            {
                ++EncounterNumber;

                if (DialogP.SetupDialog(MissionNumber, EncounterNumber, "Pre", ""))
                {
                    DialogP.Show();
                }
                else
                {
                    List<Character> Characters = Game.GetRefreshedPlayerCharacters();
                    Game = new GamePanel(Controller, MissionNumber, EncounterNumber, Characters);
                    Game.ResultButton.Click += BackToMenuFromGame_Click;
                    Controls.Add(Game);
                }
            }
            else
            {
                MapSelection.Show();
            }
        }

        private bool NextMission()
        {
            if(EncounterNumber + 1 <= MaxEncounterNumber)
            {
                return true;
            }
            return false;
        }
    }
}
