using System;
using System.Drawing;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    public partial class MenuForm : Form
    {
        readonly Controller Controller;
        readonly MapSelectionPanel MapSelection;
        readonly CharacterSelectionPanel CharacterSelection;
        readonly MenuButtonsPanel Buttons;
        readonly InfoPanel Info;
        public int MissionNumber { get; set; }
        public int EncounterNumber { get; set; }

        public MenuForm(Controller c)
        {
            InitializeComponent();

            Controller = c;
            Controller.SetFormResolution(this);
            MissionNumber = 0;
            EncounterNumber = 0;

            MapSelection = new MapSelectionPanel(Controller);
            MapSelection.Hide();
            Controls.Add(MapSelection);
            MapSelection.ButtonStartMission.Click += ButtonMission_Click;
            MapSelection.ButtonReturn.Click += BackToMenu_Click;

            CharacterSelection = new CharacterSelectionPanel(Controller);
            CharacterSelection.Hide();
            Controls.Add(CharacterSelection);
            CharacterSelection.ButtonReturn.Click += BackToMenu_Click;

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

        //Start a new game by starting the tutorial mission
        private void ButtonNewGame_Click(object sender, EventArgs e)
        {
            Controller.SetNextMap(0);
            Controller.ShowOtherForm("g");
        }

        //Continue the game
        private void ButtonContinue_Click(object sender, EventArgs e)
        {
            Buttons.Hide();
            MapSelection.Show();
        }

        //Continue the game
        private void ButtonMission_Click(object sender, EventArgs e)
        {
            MissionNumber = MapSelection.GetMissionNumber();
            EncounterNumber = MapSelection.GetEncounterNumber();
            MapSelection.Hide();
            CharacterSelection.Show();
        }

        //Show the how to play form
        private void ButtonInfo_Click(object sender, EventArgs e)
        {
            Buttons.Hide();
            Info.Show();
        }

        private void BackToMenu_Click(object sender, EventArgs e)
        {
            MapSelection.Hide();
            CharacterSelection.Hide();
            Info.Hide();
            Buttons.Show();
        }
    }
}
