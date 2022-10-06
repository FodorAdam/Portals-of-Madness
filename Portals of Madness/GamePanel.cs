using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    public class GamePanel : Panel
    {
        public GameEngine Engine { get; set; }
        public Controller Controller { get; set; }
        public List<CharacterPicture> LeftSide { get; set; }
        public List<CharacterPicture> RightSide { get; set; }
        public Size ScreenSize { get; set; }
        public PlayerAbilityFrame AbilityFrame { get; set; }
        public PlayerCharacterFrame CharacterFrame { get; set; }
        public Ability SelectedAbility { get; set; }
        public bool Casting { get; set; }
        public string Side { get; set; }
        public Button ResultButton { get; set; }
        public ToolTip ToolTip { get; set; }
        public Label ActionLabel { get; set; }

        public GamePanel(Controller c, int mapNumber, int encounterNumber)
        {
            Setup(c);
            Engine = new GameEngine(Controller, this, mapNumber, encounterNumber);
        }

        public GamePanel(Controller c, int mapNumber, int encounterNumber, List<Character> pT)
        {
            Setup(c);
            Engine = new GameEngine(Controller, this, mapNumber, encounterNumber, pT);
        }

        //Sets up all the global variables
        public void Setup(Controller c)
        {
            Controller = c;
            Casting = false;
            ScreenSize = Controller.SetPanelResolution(this);
            ActionLabel = new Label
            {
                Location = new Point(ScreenSize.Width / 2 - ScreenSize.Width / 6, ScreenSize.Height * 2 / 15),
                Size = new Size(ScreenSize.Width / 3, ScreenSize.Height / 15),
                BackColor = Color.Black,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };
            Controls.Add(ActionLabel);

            LeftSide = new List<CharacterPicture>();
            RightSide = new List<CharacterPicture>();
            int picSize = ScreenSize.Width / 16;
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
                Location = new Point((ScreenSize.Width - ScreenSize.Height * 3 / 10) / 2,
                    ScreenSize.Height - ScreenSize.Height / 10 * 2),
                Size = new Size(ScreenSize.Height * 3 / 10, ScreenSize.Height / 10),
                Visible = false
            };
            Controls.Add(ResultButton);

            ToolTip = new ToolTip();
        }

        //Initializes the UI
        public void InitializeUI(string side)
        {
            InitializeAbilityFrame();
            Side = side;
            CharacterFrame = new PlayerCharacterFrame(ScreenSize, side);
            Controls.Add(CharacterFrame.CharacterImage);
            Controls.Add(CharacterFrame.HealthLabel);
            Controls.Add(CharacterFrame.ResourceLabel);
            Controls.Add(CharacterFrame);
        }

        public void InitializeAbilityFrame()
        {
            AbilityFrame = new PlayerAbilityFrame(ScreenSize);
            for (int i = 0; i < AbilityFrame.AbilityButtons.Count; i++)
            {
                Controls.Add(AbilityFrame.AbilityButtons[i]);
            }
            Controls.Add(AbilityFrame);
            AssignAbilityButtonClickFunctions();
            AssignAbilityButtonHoverFunctions();
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
                    if (i < team.Count())
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
            int baseX = screenSize.Width * 2 / 5;
            int mult = 1;
            if (side == "right")
            {
                baseX = screenSize.Width - screenSize.Width * 2 / 5 - size;
                mult = -1;
            }
            int baseY = screenSize.Height / 2;
            int i = 0;
            for (int j = 1; j <= 4; j++)
            {
                for (int k = 0; k < j; k++)
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
                if (SelectedAbility.TargetCount == 1)
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
                Casting = false;
                Engine.Manage();
            }
        }

        public void UpdateCharacterBars()
        {
            foreach (CharacterPicture Pic in LeftSide)
            {
                if (Pic.Character != null)
                {
                    Pic.UpdateBars();
                }
            }
            foreach (CharacterPicture Pic in RightSide)
            {
                if (Pic.Character != null)
                {
                    Pic.UpdateBars();
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
            if (SelectedAbility.TargetCount >= team.Count)
            {
                return team;
            }

            List<Character> targets = new List<Character>();
            int pos = team.FindIndex(x => x == target);
            targets.Add(target);
            for(int i = 1; i < SelectedAbility.TargetCount; i++)
            {
                targets.Add(Engine.CurrentCharacter.SelectNewTarget(team, pos, 0));
            }
            return targets;
        }

        //Assigns mouseover function to the buttons
        public void AssignAbilityButtonHoverFunctions()
        {
            for (int i = 0; i < AbilityFrame.AbilityButtons.Count; i++)
            {
                AbilityFrame.AbilityButtons[i].MouseEnter += AbilityButton_MouseEnter;
                AbilityFrame.AbilityButtons[i].MouseLeave += AbilityButton_MouseLeave;
            }
        }

        //Assigns click function to the buttons: if the ability's type is anything but random, it makes the ability
        //the current ability and if it is random, it chooses random targets to attack instantly
        public void AssignAbilityButtonClickFunctions()
        {
            for (int i = 0; i < AbilityFrame.AbilityButtons.Count; i++)
            {
                AbilityFrame.AbilityButtons[i].Click += AbilityButton_Click;
            }
        }

        private void AbilityButton_MouseEnter(object sender, EventArgs e)
        {
            AbilityButton abButton = (AbilityButton)sender;
            ToolTip.Show(abButton.ability.ToString(), abButton);
        }

        private void AbilityButton_MouseLeave(object sender, EventArgs e)
        {
            AbilityButton abButton = (AbilityButton)sender;
            ToolTip.Hide(abButton);
        }

        private void AbilityButton_Click(object sender, EventArgs e)
        {
            AbilityButton abButton = (AbilityButton)sender;
            SelectedAbility = null;
            Ability ab = abButton.ability;
            if (Engine.CurrentCharacter.CanCast(ab))
            {
                Console.WriteLine($"{ab.Name} was cast");
                if (ab.Modifier == "random")
                {
                    Casting = false;
                    List<Character> targets = new List<Character>();
                    for (int j = 0; j < ab.TargetCount; j++)
                    {
                        targets.Add(Engine.CurrentCharacter.SelectRandomTarget(
                                ab.Target == "ally" ? Engine.PlayerTeam :
                                ab.Target == "enemy" ? Engine.EnemyTeam : Engine.InitiativeTeam));
                    }
                    Engine.CurrentCharacter.CastAbility(ab, targets);
                    Engine.Manage();
                }
                else
                {
                    SelectedAbility = ab;
                    Cast();
                }
            }
        }

        private void Cast()
        {
            Casting = true;
            //targetArrowsSetup();
        }

        public void ShowResultButton(bool res)
        {
            AbilityFrame.Hide();
            ResultButton.Show();
            if (res)
            {
                ResultButton.Text = "Victory!";
            }
            else
            {
                ResultButton.Text = "You lost.";
            }
        }

        public List<Character> GetRefreshedPlayerCharacters()
        {
            List<Character> result = Engine.PlayerTeam;
            foreach(Character c in result)
            {
                c.Reset();
                if(Side == "right")
                {
                    c.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }
            }
            return result;
        }
    }
}
