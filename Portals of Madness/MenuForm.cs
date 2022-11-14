using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        public List<int> OptionalEncounters { get; set; }
        public int EncounterNumber { get; set; }
        public bool HasCharacters { get; set; }
        public bool ShowContinue { get; set; }

        public MenuForm(Controller c)
        {
            InitializeComponent();

            BackgroundImage = Image.FromFile($@"../../Art/Backgrounds/title_screen.png");
            BackgroundImageLayout = ImageLayout.Stretch;

            Controller = c;
            Controller.SetFormResolution(this);

            FormBorderStyle = FormBorderStyle.FixedSingle;

            MissionNumber = 0;
            EncounterNumber = 0;
            HasCharacters = false;
            ShowContinue = File.Exists($@"../../Missions/0/Mission.xml");
            CopyFiles(false);
            OptionalEncounters = new List<int>();

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
            DialogP.ButtonContAlt.Click += ContinueAlt_Click;
            DialogP.ButtonFirstStart.Click += StartNewGame_Click;
            DialogP.ButtonBack.Click += ButtonMapsFromEnd_Click;
            DialogP.ButtonCont.Click += ContinueIfLastHadEndingDialog_Click;

            Buttons = new MenuButtonsPanel(Controller);
            Controls.Add(Buttons);
            Buttons.ButtonContinue.Visible = ShowContinue;
            Buttons.ButtonNewGame.Click += ButtonNewGame_Click;
            Buttons.ButtonContinue.Click += ButtonContinue_Click;
            Buttons.ButtonInfo.Click += ButtonInfo_Click;
            Buttons.ButtonLoad.Click += ButtonLoad_Click;
            Buttons.ButtonExit.Click += ButtonExit_Click;
            Buttons.ButtonTest.Click += ButtonTest_Click;

            Info = new InfoPanel(Controller);
            Info.Hide();
            Controls.Add(Info);
            Info.ButtonBack.Click += BackToMenu_Click;
        }

        private void CopyFiles(bool b)
        {
            try
            {
                File.Copy($@"../../Backups/Abilities.xml", $@"../../Abilities/Abilities.xml", b);
            }
            catch { }
            try
            {
                File.Copy($@"../../Backups/Characters.xml", $@"../../Characters/Characters.xml", b);
            }
            catch { }
            string dirPath = $@"../../Backups/";
            int amount = Directory.GetDirectories(dirPath).Length;
            for (int i = 0; i < amount; i++)
            {
                try
                {
                    Directory.CreateDirectory($@"../../Missions/{i}");
                }
                catch { }
                try
                {
                    File.Copy($@"../../Backups/{i}/Mission.xml", $@"../../Missions/{i}/Mission.xml", b);
                }
                catch { }
                try
                {
                    File.Copy($@"../../Backups/{i}/Dialog.xml", $@"../../Missions/{i}/Dialog.xml", b);
                }
                catch { }
            }
        }

        private void ButtonTest_Click(object sender, EventArgs e)
        {
            Controller.RunTests();
        }

        private void ButtonExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonLoad_Click(object sender, EventArgs e)
        {
            CopyFiles(true);
            Console.WriteLine("Game directories updated.");
        }

        //Start a new game, show dialog
        private void ButtonNewGame_Click(object sender, EventArgs e)
        {
            Buttons.Hide();
            CopyFiles(true);
            MaxEncounterNumber = 6;
            OptionalEncounters.Add(3);
            HasCharacters = true;
            ShowContinue = true;
            DialogP.FillUpCharacters();
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
                DialogP.FillUpCharacters();
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
            if(HasCharacters)
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

        //Skip optonal mission and start next mission from dialog window
        private void ContinueAlt_Click(object sender, EventArgs e)
        {
            DialogP.Hide();
            if (NextNextMission())
            {
                Controller.GEH.EncounterCompleteEvents(MissionNumber, EncounterNumber);
                NextMissionSetup(2);
            }
            else
            {
                ShowAndUpdateMaps();
            }
        }

        //Continue the game from save
        private void ButtonContinue_Click(object sender, EventArgs e)
        {
            HasCharacters = false;
            Buttons.Hide();
            ShowAndUpdateMaps();
        }

        //Continue the game from save
        private void ButtonMapsFromEnd_Click(object sender, EventArgs e)
        {
            HasCharacters = false;
            DialogP.Hide();
            ShowAndUpdateMaps();
        }

        //Select the mission
        private void ButtonMission_Click(object sender, EventArgs e)
        {
            MissionNumber = MapSelection.GetMissionNumber();
            MaxEncounterNumber = MapSelection.GetMaxEncounterNumber();
            EncounterNumber = MapSelection.GetEncounterNumber();
            HasCharacters = false;
            MapSelection.Hide();
            ShowAndUpdateCharacters();
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
            Buttons.ButtonContinue.Visible = true;
        }

        //Start next mission if one is still availabe, return to mapselection if not or if the player died
        private void BackToMenuFromGame_Click(object sender, EventArgs e)
        {
            Game.Hide();
            Controller.GEH.EncounterCompleteEvents(MissionNumber, EncounterNumber);
            HasCharacters = true;
            if (!Game.Engine.EndRun)
            {
                //If has post encounter dialog
                if (DialogP.SetupDialog(MissionNumber, EncounterNumber, "Post", ""))
                {
                    //If final encounter in the mission
                    if (EncounterNumber + 1 >= MaxEncounterNumber)
                    {
                        DialogP.EndLast(true);
                    }
                    //If not the final encounter
                    else
                    {
                        MapSelection.UpdateAllAvailableMissions();
                        if (Controller.XMLOperations.AllEncounters[MissionNumber].Encounter[EncounterNumber + 1].Optional)
                        {
                            DialogP.HasAltPath(true);
                        }
                        DialogP.ContinueWithNext(true);
                    }

                    DialogP.Show();
                }
                //If has next mission
                else if (NextMission())
                {
                    NextMissionSetup(1);
                }
                else
                {
                    ShowAndUpdateMaps();
                }
            }
            else
            {
                ShowAndUpdateMaps();
            }
        }

        private void NextMissionSetup(int add)
        {
            EncounterNumber += add;

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

        private void ShowAndUpdateCharacters()
        {
            CharacterSelection.UpdateAllAvailableCharacters();
            CharacterSelection.Show();
        }

        private void ShowAndUpdateMaps()
        {
            MapSelection.UpdateAllAvailableMissions();
            MapSelection.Show();
        }

        private void ContinueIfLastHadEndingDialog_Click(object sender, EventArgs e)
        {
            DialogP.Hide();
            if (NextMission())
            {
                Controller.GEH.EncounterCompleteEvents(MissionNumber, EncounterNumber);
                NextMissionSetup(1);
            }
            else
            {
                ShowAndUpdateMaps();
            }
        }

        private bool NextNextMission()
        {
            if (EncounterNumber + 2 <= MaxEncounterNumber)
            {
                return true;
            }
            return false;
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
