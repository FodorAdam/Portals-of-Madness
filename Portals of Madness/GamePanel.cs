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
        public Dialogs DialogContainer { get; set; }
        public int DialogIndex { get; set; }
        public int LineIndex { get; set; }
        public int MaxLines { get; set; }
        public Button ButtonNext { get; set; }
        public Button ButtonStart { get; set; }
        public Label DialogLabelBox { get; set; }
        public PictureBox LeftCharacter { get; set; }
        public PictureBox RightCharacter { get; set; }
        public XMLCharacters XMLCharacterList { get; set; }
        private int PicSize { get; set; }

        public GamePanel(Controller c, int mapNumber, int encounterNumber)
        {
            Setup(c, mapNumber, encounterNumber);
            Engine = new GameEngine(Controller, this, mapNumber, encounterNumber);
        }

        public GamePanel(Controller c, int mapNumber, int encounterNumber, List<Character> pT)
        {
            Setup(c, mapNumber, encounterNumber);
            Engine = new GameEngine(Controller, this, mapNumber, encounterNumber, pT);
        }

        //Sets up all the global variables
        public void Setup(Controller c, int NextMap, int NextEncounter)
        {
            Controller = c;

            FillUpCharacters();

            string path = $@"../../Missions/{NextMap}/Dialog.xml";
            try
            {
                DialogContainer = (Dialogs)Controller.XMLOperations.GenericDeserializer<Dialogs>(path);
            }
            catch
            {
                Console.WriteLine($"{NextMap}/Dialog.xml not found!");
            }

            ScreenSize = Controller.SetPanelResolution(this);

            AbilityFrame = new PlayerAbilityFrame(new Size(ScreenSize.Width / 5, ScreenSize.Height / 10));
            SetupCombatDialog(NextEncounter);

            Casting = false;
            ActionLabel = new Label
            {
                Location = new Point(ScreenSize.Width / 2 - ScreenSize.Width / 6, ScreenSize.Height * 2 / 15),
                Size = new Size(ScreenSize.Width / 3, ScreenSize.Height / 15),
                BackColor = Color.Black,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Tahoma", 12, FontStyle.Bold),
                Visible = false
            };
            Controls.Add(ActionLabel);

            LeftSide = new List<CharacterPicture>();
            RightSide = new List<CharacterPicture>();
            PicSize = ScreenSize.Width / 16;
            for (int i = 0; i < 10; i++)
            {
                LeftSide.Add(new CharacterPicture());
                SetupBoxes(LeftSide[i]);
                RightSide.Add(new CharacterPicture());
                SetupBoxes(RightSide[i]);
            }

            PlacePictureBoxes("left", LeftSide, ScreenSize);
            PlacePictureBoxes("right", RightSide, ScreenSize);

            ResultButton = new Button
            {
                Location = new Point(ScreenSize.Width / 2 - AbilityFrame.Width / 2,
                ScreenSize.Height - AbilityFrame.Height * 2),
                Size = new Size(ScreenSize.Width / 5, ScreenSize.Height / 10),
                Visible = false
            };
            Controls.Add(ResultButton);

            ToolTip = new ToolTip();
        }

        private void SetupCombatDialog(int NextEncounter)
        {
            int w = ScreenSize.Width;
            int h = ScreenSize.Height;

            //Set the size of the buttons
            int buttonWidth = w / 5;
            int buttonHeight = h / 10;

            ButtonNext = new Button
            {
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(w / 2 - buttonWidth / 2, h - 2 * buttonHeight),
                Text = "Continue",
                TabIndex = 0,
                AutoSize = true,
                UseVisualStyleBackColor = true,
                Visible = false
            };
            Controls.Add(ButtonNext);

            ButtonStart = new Button
            {
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(w / 2 - buttonWidth / 2, h - 2 * buttonHeight),
                Text = "End Dialog",
                TabIndex = 1,
                AutoSize = true,
                UseVisualStyleBackColor = true,
                Visible = false
            };
            Controls.Add(ButtonStart);

            DialogLabelBox = new Label
            {
                Location = new Point(w * 5 / 18, h / 3),
                Size = new Size(w * 8 / 18, h / 6),
                TabIndex = 4,
                Text = "",
                BackColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                Font = new Font("Tahoma", 12, FontStyle.Bold),
            };
            Controls.Add(DialogLabelBox);

            LeftCharacter = new PictureBox
            {
                Size = new Size(w / 6, h / 3),
                Location = new Point(w / 18, h / 6),
                BackColor = Color.Gray,
                SizeMode = PictureBoxSizeMode.StretchImage,
                TabIndex = 5,
                TabStop = false
            };
            Controls.Add(LeftCharacter);

            RightCharacter = new PictureBox
            {
                Size = new Size(w / 6, h / 3),
                Location = new Point(w - w * 4 / 18, h / 6),
                BackColor = Color.Gray,
                SizeMode = PictureBoxSizeMode.StretchImage,
                TabIndex = 6,
                TabStop = false
            };
            Controls.Add(RightCharacter);

            DialogIndex = -1;
            for (int i = 0; i < DialogContainer.Dialog.Length; i++)
            {
                int ID;
                if (int.TryParse(DialogContainer.Dialog[i].Id, out _))
                {
                    ID = int.Parse(DialogContainer.Dialog[i].Id);
                }
                else
                {
                    string tmp = DialogContainer.Dialog[i].Id.Substring(1);
                    ID = int.Parse(tmp);
                }

                if (ID == NextEncounter && DialogContainer.Dialog[i].Type == "Combat")
                {
                    DialogIndex = i;
                    MaxLines = DialogContainer.Dialog[DialogIndex].Lines.Line.Length;
                    Console.WriteLine("Dialog index: " + DialogIndex);
                }
            }

            if(DialogIndex == -1)
            {
                LeftCharacter.Hide();
                RightCharacter.Hide();
                DialogLabelBox.Hide();
            }
            else
            {
                AbilityFrame.Hide();
                UpdateDialog();
            }
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            LeftCharacter.Hide();
            RightCharacter.Hide();
            DialogLabelBox.Hide();
            ButtonStart.Hide();
            AbilityFrame.Show();
        }

        public void FillUpCharacters()
        {
            try
            {
                XMLCharacterList = (XMLCharacters)Controller.XMLOperations.GenericDeserializer<XMLCharacters>($@"../../Characters/Characters.xml");
            }
            catch
            {
                Console.WriteLine($"Characters.xml not found!");
            }
        }

        public void UpdateDialog()
        {
            string stringid = DialogContainer.Dialog[DialogIndex].Lines.Line[LineIndex].Speaker;
            try
            {
                var cEnum = XMLCharacterList.XmlCharacter.Where(a => a.Id.Contains(stringid)).Select(a => a).First();
                Character speaker = Controller.XMLOperations.ConvertToCharacter(cEnum);

                if (DialogContainer.Dialog[DialogIndex].Lines.Line[LineIndex].Side == "left")
                {
                    LeftCharacter.Image = Controller.ImageConverter(speaker.BaseImage, "profile");
                    RightCharacter.Image = null;
                }
                else
                {
                    RightCharacter.Image = Controller.ImageConverter(speaker.BaseImage, "profile");
                    RightCharacter.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    LeftCharacter.Image = null;
                }

                DialogLabelBox.Text = $"{speaker.Name}: {DialogContainer.Dialog[DialogIndex].Lines.Line[LineIndex].Str}";
            }
            catch
            {
                DialogLabelBox.Text = $"{stringid}: {DialogContainer.Dialog[DialogIndex].Lines.Line[LineIndex].Str}";
            }
        }

        private void ButtonNext_Click(object sender, EventArgs e)
        {
            ++LineIndex;
            Console.WriteLine($"{LineIndex} / {MaxLines}");
            if (LineIndex + 1 < MaxLines)
            {
                UpdateDialog();
            }
            else
            {
                UpdateDialog();
                ButtonNext.Hide();
                ButtonStart.Show();
            }
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
            if(DialogIndex != -1)
            {
                AbilityFrame.Hide();
                ButtonNext.Show();
            }
            ButtonNext.Click += ButtonNext_Click;
            ButtonStart.Click += ButtonStart_Click;
        }

        public void InitializeAbilityFrame()
        {
            AbilityFrame.Location = new Point(ScreenSize.Width / 2 - AbilityFrame.Width / 2,
                ScreenSize.Height - AbilityFrame.Height * 2);
            for (int i = 0; i < AbilityFrame.AbilityButtons.Count; i++)
            {
                Controls.Add(AbilityFrame.AbilityButtons[i]);
                int extra = AbilityFrame.Height / 3;
                AbilityFrame.AbilityButtons[i].Location =
                    new Point(AbilityFrame.Location.X + i * AbilityFrame.AbilityButtons[i].Width + (i + 1) * extra,
                    AbilityFrame.Location.Y + AbilityFrame.Height / 10);
            }
            Controls.Add(AbilityFrame);
            AssignAbilityButtonClickFunctions();
            AssignAbilityButtonHoverFunctions();
        }

        //Sets up the character pictures
        public void SetupBoxes(CharacterPicture charPic)
        {
            charPic.SizeMode = PictureBoxSizeMode.StretchImage;
            charPic.BackColor = Color.Transparent;
            Controls.Add(charPic);
        }

        //Assigns the characters to their pictures
        public void AddCharacter(Character ch, string side, int loc)
        {
            if ("left".Equals(side))
            {
                AddCharacterInCorrectSide(LeftSide, side, ch, loc);
            }
            else
            {
                AddCharacterInCorrectSide(RightSide, side, ch, loc);
            }
        }

        private void AddCharacterInCorrectSide(List<CharacterPicture> CharPicList, string side, Character ch, int loc)
        {
            if (loc < CharPicList.Count())
            {
                CharPicList[loc].Character = ch;
                if (CharPicList[loc].Character.CharacterClass == "miniboss")
                {
                    CharPicList[loc].Size = new Size(PicSize * 11 / 10, PicSize * 11 / 10);
                }
                else
                {
                    CharPicList[loc].Size = new Size(PicSize, PicSize);
                }
                CharPicList[loc].Image = ch.Image;
                CharPicList[loc].InitializeBars();
                if (side == "right")
                {
                    CharPicList[loc].Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }
                Controls.Add(CharPicList[loc].HealthBar);
                Controls.Add(CharPicList[loc].ResourceBar);
                CharPicList[loc].Click += AssignCharacterClickFunctions;
            }
        }

        //Assigns the characters to their pictures
        public void PlaceCharacters(List<Character> team, string side)
        {
            if ("left".Equals(side))
            {
                PlaceCharactersInCorrectSide(LeftSide, side, team);
            }
            else
            {
                PlaceCharactersInCorrectSide(RightSide, side, team);
            }
        }

        //Assigns the characters to their pictures
        public void DisplaceCharacters(List<Character> team, string side)
        {
            if ("left".Equals(side))
            {
                ClearMissingCharacters(LeftSide, team);
            }
            else
            {
                ClearMissingCharacters(RightSide, team);
            }
        }

        private void CharPicInit(List<CharacterPicture> CharPicList, string side, Character ch, int index)
        {
            CharPicList[index].Image = ch.Image;
            CharPicList[index].InitializeBars();
            if (side == "right")
            {
                CharPicList[index].Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
            }
            Controls.Add(CharPicList[index].HealthBar);
            Controls.Add(CharPicList[index].ResourceBar);
            CharPicList[index].Click += AssignCharacterClickFunctions;
        }

        public void ClearMissingCharacters(List<CharacterPicture> CharPicList, List<Character> team)
        {
            for (int i = 0; i < 10; i++)
            {
                if (i >= team.Count())
                {
                    CharPicList[i].Character = null;
                    CharPicList[i].Image = null;
                    CharPicList[i].HideBars();
                    Controls.Remove(CharPicList[i].HealthBar);
                    Controls.Remove(CharPicList[i].ResourceBar);
                    CharPicList[i].Click -= AssignCharacterClickFunctions;
                }
            }
        }

        private void PlaceCharactersInCorrectSide(List<CharacterPicture> CharPicList, string side, List<Character> team)
        {
            if(team[0].CharacterClass == "boss")
            {
                CharPicList[0].Character = team[0];
                for (int i = 1; i < 10; i++)
                {
                    CharPicList[i].Character = null;
                    CharPicList[i].Image = null;
                    Controls.Remove(CharPicList[i].HealthBar);
                    Controls.Remove(CharPicList[i].ResourceBar);
                    CharPicList[i].Location = new Point(0, 0);
                    CharPicList[i].Click -= AssignCharacterClickFunctions;
                }

                int w = ScreenSize.Width;
                int h = ScreenSize.Height;
                CharPicList[0].Size = new Size(PicSize * 4, PicSize * 4);
                if(side == "right")
                {
                    CharPicList[0].Location = new Point(w / 2 + PicSize, h / 4);
                }
                else
                {
                    CharPicList[0].Location = new Point(w / 2 - PicSize, h / 4);
                }
                CharPicInit(CharPicList, side, team[0], 0);
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    if (i < team.Count())
                    {
                        CharPicList[i].Character = team[i];
                        if (CharPicList[i].Character.CharacterClass == "miniboss")
                        {
                            CharPicList[i].Size = new Size(PicSize * 11 / 10, PicSize * 11 / 10);
                        }
                        else
                        {
                            CharPicList[i].Size = new Size(PicSize, PicSize);
                        }
                        CharPicInit(CharPicList, side, team[i], i);
                    }
                    else
                    {
                        CharPicList[i].Character = null;
                        CharPicList[i].Image = null;
                        Controls.Remove(CharPicList[i].HealthBar);
                        Controls.Remove(CharPicList[i].ResourceBar);
                        CharPicList[i].Click -= AssignCharacterClickFunctions;
                    }
                }
            }
        }

        //Places the character picture boxes in a triangular pattern
        public void PlacePictureBoxes(string side, List<CharacterPicture> list, Size screenSize)
        {
            int baseX = screenSize.Width * 2 / 5;
            int mult = 1;
            if (side == "right")
            {
                baseX = screenSize.Width - screenSize.Width * 2 / 5 - PicSize;
                mult = -1;
            }
            int baseY = screenSize.Height / 2;
            int i = 0;
            for (int j = 1; j <= 4; j++)
            {
                for (int k = 0; k < j; k++)
                {
                    list[i].Location = new Point(baseX - (int)((j - 1) * 1.4 * PicSize * mult),
                        baseY + (j - 1) * PicSize - (int)(k * 1.4 * PicSize));
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
                Casting = false;
                if (SelectedAbility.TargetCount == 1)
                {
                    Engine.CurrentCharacter.CastAbility(SelectedAbility, target);
                    Engine.ActionEventHandler($"{Engine.CurrentCharacter.Name} used {SelectedAbility.Name} on {target.Name}", Engine.CurrentCharacter);
                }
                else
                {
                    List<Character> targets;
                    targets = SelectAimedTargets(target, SelectedAbility.Target == "ally" ? Engine.PlayerTeam :
                        SelectedAbility.Target == "enemy" ? Engine.EnemyTeam : Engine.InitiativeTeam);
                    Engine.CurrentCharacter.CastAbility(SelectedAbility, targets);
                    Engine.ActionEventHandler($"{Engine.CurrentCharacter.Name} used {SelectedAbility.Name} on multiple targets", Engine.CurrentCharacter);
                }
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

        public void UpdateCharacterImages(Character c)
        {
            foreach (CharacterPicture Pic in LeftSide)
            {
                if (Pic.Character == c)
                {
                    Pic.Image = c.Image;
                }
            }
            foreach (CharacterPicture Pic in RightSide)
            {
                if (Pic.Character == c)
                {
                    Pic.Image = c.Image;
                    Pic.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
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
            ToolTip.Show(abButton.Ab.ToString(), abButton);
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
            Ability ab = abButton.Ab;
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
                    if (targets.Count > 1)
                    {
                        Engine.ActionEventHandler($"{Engine.CurrentCharacter.Name} used {ab.Name} on multiple targets", Engine.CurrentCharacter);
                    }
                    else
                    {
                        Engine.ActionEventHandler($"{Engine.CurrentCharacter.Name} used {ab.Name} on {targets[0].Name}", Engine.CurrentCharacter);
                    }
                    Engine.Manage();
                }
                else
                {
                    SelectedAbility = ab;
                    Cast();
                }
            }
        }

        /// <summary>
        /// Sets the Casting variable to True to allow manual casting of the ability pressed on targets, also TODO: set up arrow setup
        /// </summary>
        private void Cast()
        {
            Casting = true;
            //targetArrowsSetup();
        }

        public void ShowResultButton(bool res)
        {
            AbilityFrame.Hide();
            ResultButton.Show();
            ResultButton.Text = "Continue";
            ActionLabel.Show();
            if (res)
            {
                ActionLabel.Text = "Victory!";
            }
            else
            {
                ActionLabel.Text = "You lost.";
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
