using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    public partial class GameForm : Form
    {
        public GameEngine Engine { get; set; }
        public Controller Controller { get; set; }
        public List<CharacterPicture> LeftSide { get; set; }
        public List<CharacterPicture> RightSide { get; set; }
        public Size ScreenSize { get; set; }
        public PlayerAbilityFrame AbilityFrame { get; set; }
        public PlayerCharacterFrame CharacterFrame { get; set; }
        public DialogBox DialogBox { get; set; }
        public Ability SelectedAbility { get; set; }
        public bool Casting { get; set; }
        public string Side { get; set; }
        public Button ResultButton { get; set; }

        public GameForm()
        {
            InitializeComponent();
            Setup();
            Engine = new GameEngine(this);
        }

        public GameForm(int mapNumber, List<Character> pT)
        {
            InitializeComponent();
            Setup();
            Engine = new GameEngine(this, pT, mapNumber);
        }

        //Sets up all the global variables
        public void Setup()
        {
            Casting = false;
            Controller = new Controller();
            ScreenSize = Controller.Resolution(this);

            LeftSide = new List<CharacterPicture>();
            RightSide = new List<CharacterPicture>();
            int picSize = ScreenSize.Width / 24;
            for (int i = 0; i < 10; i++)
            {
                LeftSide.Add(new CharacterPicture());
                SetupBoxes(LeftSide[i], picSize);
                RightSide.Add(new CharacterPicture());
                SetupBoxes(RightSide[i], picSize);
            }

            PlacePictureBoxes("left", LeftSide, picSize, ScreenSize);
            PlacePictureBoxes("right", RightSide, picSize, ScreenSize);

            ResultButton = new Button
            {
                Location = new Point(Width / 2, Height / 2),
                Visible = false
            };
            Controls.Add(ResultButton);
        }

        //Initializes the UI
        public void InitializeUI(string side)
        {
            AbilityFrame = new PlayerAbilityFrame(ScreenSize);
            for (int i = 0; i < AbilityFrame.AbilityButtons.Count; i++)
            {
                Controls.Add(AbilityFrame.AbilityButtons[i]);
            }
            Controls.Add(AbilityFrame);
            this.Side = side;
            CharacterFrame = new PlayerCharacterFrame(ScreenSize, side);
            Controls.Add(CharacterFrame.characterImage);
            Controls.Add(CharacterFrame.healthLabel);
            Controls.Add(CharacterFrame.resourceLabel);
            Controls.Add(CharacterFrame);
            //dialogBox = new DialogBox(screenSize);
            //Controls.Add(dialogBox);
        }

        //Sets up the character pictures
        public void SetupBoxes(CharacterPicture charPic, int picSize)
        {
            charPic.Size = new Size(picSize, picSize);
            charPic.SizeMode = PictureBoxSizeMode.StretchImage;
            charPic.BackColor = Color.Transparent;
            Controls.Add(charPic);
        }

        //Assigns the characters to their pictures
        public void PlaceCharacters(List<Character> team, string side)
        {
            if ("left".Equals(side))
            {
                for (int i = 0; i < 10; i++)
                {
                    if(i < team.Count())
                    {
                        LeftSide[i].Character = team[i];
                        LeftSide[i].Image = team[i].Image;
                        LeftSide[i].InitializeBars();
                        Controls.Add(LeftSide[i].HealthBar);
                        Controls.Add(LeftSide[i].ResourceBar);
                        LeftSide[i].Click += AssignCharacterClickFunctions;
                    }
                    else
                    {
                        LeftSide[i].Character = null;
                        LeftSide[i].Image = null;
                        Controls.Remove(LeftSide[i].HealthBar);
                        Controls.Remove(LeftSide[i].ResourceBar);
                        LeftSide[i].Click -= AssignCharacterClickFunctions;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    if (i < team.Count())
                    {
                        RightSide[i].Character = team[i];
                        RightSide[i].Image = team[i].Image;
                        RightSide[i].InitializeBars();
                        RightSide[i].Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        Controls.Add(RightSide[i].HealthBar);
                        Controls.Add(RightSide[i].ResourceBar);
                        RightSide[i].Click += AssignCharacterClickFunctions;
                    }
                    else
                    {
                        RightSide[i].Character = null;
                        RightSide[i].Image = null;
                        Controls.Remove(RightSide[i].HealthBar);
                        Controls.Remove(RightSide[i].ResourceBar);
                        RightSide[i].Click -= AssignCharacterClickFunctions;
                    }
                }
            }
        }

        //Places the character picture boxes in a triangular pattern
        public void PlacePictureBoxes(string side, List<CharacterPicture> list, int size, Size screenSize)
        {
            int baseX = screenSize.Width / 3;
            int mult = 1;
            if(side == "right")
            {
                baseX = screenSize.Width - screenSize.Width / 3 - size;
                mult = -1;
            }
            int baseY = screenSize.Height / 2;
            int i = 0;
            for(int j=1; j <= 4; j++)
            {
                for(int k=0; k < j; k++)
                {
                    list[i].Location = new Point(baseX - (int)((j - 1) * 1.4 * size * mult),
                        baseY + (j - 1) * size - (int)(k * 1.4 * size));
                    ++i;
                }
            }
        }

        //Make this select more than 1 target if necessary
        public void AssignCharacterClickFunctions(object sender, EventArgs e)
        {
            CharacterPicture t = (CharacterPicture)sender;
            Character target = t.Character;
            if (Casting && CorrectTarget(t))
            {
                if(SelectedAbility.TargetCount == 1)
                {
                    Engine.CurrentCharacter.CastAbility(SelectedAbility, target);
                }
                else
                {
                    List<Character> targets;
                    targets = SelectAimedTargets(target, SelectedAbility.Target == "ally" ? Engine.PlayerTeam :
                        SelectedAbility.Target == "enemy" ? Engine.EnemyTeam : Engine.InitiativeTeam);
                    Engine.CurrentCharacter.CastAbility(SelectedAbility, targets);
                }
                UpdateCharacterBars();
                Engine.Manage();
            }
        }

        public void UpdateCharacterBars()
        {
            foreach (CharacterPicture Pic in LeftSide)
            {
                if(Pic.Character != null)
                {
                    Pic.UpdatePanelWidth();
                }
            }
            foreach (CharacterPicture Pic in RightSide)
            {
                if (Pic.Character != null)
                {
                    Pic.UpdatePanelWidth();
                }
            }
        }

        public bool CorrectTarget(CharacterPicture t)
        {
            return (SelectedAbility.Target == "ally" && Side == "left" && t.Location.X < ScreenSize.Width / 2) ||
                (SelectedAbility.Target == "enemy" && Side == "left" && t.Location.X > ScreenSize.Width / 2) ||
                (SelectedAbility.Target == "ally" && Side == "right" && t.Location.X > ScreenSize.Width / 2) ||
                (SelectedAbility.Target == "enemy" && Side == "right" && t.Location.X < ScreenSize.Width / 2) ||
                SelectedAbility.Target == "all";
        }

        public List<Character> SelectAimedTargets(Character target, List<Character> team)
        {
            if(SelectedAbility.TargetCount >= team.Count)
            {
                return team;
            }

            List<Character> targets = new List<Character>();
            int pos = team.FindIndex(x => x == target);
            if(pos + SelectedAbility.TargetCount / 2 >= team.Count)
            {
                for(int i = 1; i <= SelectedAbility.TargetCount; i++)
                {
                    targets.Add(team[team.Count - i]);
                }
            }
            else if(pos - SelectedAbility.TargetCount / 2 < 0)
            {
                for (int i = 0; i < SelectedAbility.TargetCount; i++)
                {
                    targets.Add(team[i]);
                }
            }
            else
            {
                for (int i = 0; i < SelectedAbility.TargetCount; i++)
                {
                    targets.Add(team[pos - SelectedAbility.TargetCount / 2 + i]);
                }
            }
            return targets;
        }

        //Assigns functions to the buttons: if the ability's type is anything but random, it makes the ability
        //the current ability and if it is random, it chooses random targets to attack instantly
        public void AssignAbilityButtonFunctions()
        {
            for(int i=0; i<AbilityFrame.AbilityButtons.Count; i++)
            {
                AbilityFrame.AbilityButtons[i].MouseEnter += AbilityButton_MouseEnter;
                AbilityFrame.AbilityButtons[i].MouseLeave += AbilityButton_MouseLeave;
                AbilityFrame.AbilityButtons[i].Click += AbilityButton_Click;
            }
        }

        private void AbilityButton_MouseEnter(object sender, EventArgs e)
        {
            AbilityButton abButton = (AbilityButton)sender;
            toolTip.Show(abButton.ability.ToString(), abButton);
        }

        private void AbilityButton_MouseLeave(object sender, EventArgs e)
        {
            AbilityButton abButton = (AbilityButton)sender;
            toolTip.Hide(abButton);
        }

        private void AbilityButton_Click(object sender, EventArgs e)
        {
            AbilityButton abButton = (AbilityButton)sender;
            SelectedAbility = null;
            Ability ab = abButton.ability;
            if (ab.AbilityType == "random" && Engine.CurrentCharacter.CanCast(ab))
            {
                Casting = false;
                for (int j = 0; j < ab.TargetCount; j++)
                {
                    Engine.CurrentCharacter.CastAbility(ab,
                        Engine.CurrentCharacter.SelectRandomTarget(
                            ab.Target == "ally" ? Engine.PlayerTeam :
                            ab.Target == "enemy" ? Engine.EnemyTeam : Engine.InitiativeTeam));
                }
                UpdateCharacterBars();
                Engine.Manage();
            }
            else
            {
                SelectedAbility = ab;
                Cast();
            }
        }

        private void Cast()
        {
            if(Engine.CurrentCharacter.CanCast(SelectedAbility))
            {
                Casting = true;
                //targetArrowsSetup();
            }
            else
            {
                //targetArrows = new ArrayList();
            }
        }

        public void ShowResultButton(bool res)
        {
            ResultButton.Visible = true;
            if (res)
            {
                if(Engine.CurrentMission.EncounterNumber == Engine.CurrentMission.EncounterContainer.encounter.Length - 1)
                {
                    ResultButton.Text = "Victory!";
                    ResultButton.Click += ToSelectionForm;
                }
                else
                {
                    ResultButton.Text = "To the next encounter.";
                    ResultButton.Visible = false;
                    ResultButton.Click += ContinueTheMission;
                }
            }
            else
            {
                {
                    ResultButton.Text = "You lost.";
                    ResultButton.Click += ToSelectionForm;
                }
            }
        }

        private void ContinueTheMission(object sender, EventArgs e)
        {
            Engine.Manage();
        }

        private void ToSelectionForm(object sender, EventArgs e)
        {
            Controller.ChangeForm(this, "s");
        }
    }
}
