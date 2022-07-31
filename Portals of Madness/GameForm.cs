using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    public partial class GameForm : Form
    {
        public GameEngine engine { get; set; }
        public Controller controller { get; set; }
        public List<CharacterPicture> leftSide { get; set; }
        public List<CharacterPicture> rightSide { get; set; }
        public Size screenSize { get; set; }
        public PlayerAbilityFrame abilityFrame { get; set; }
        public PlayerCharacterFrame characterFrame { get; set; }
        public DialogBox dialogBox { get; set; }
        public Ability selectedAbility { get; set; }
        public bool casting { get; set; }
        public string side { get; set; }

        public GameForm()
        {
            InitializeComponent();
            Setup();
            engine = new GameEngine(this);
        }

        public GameForm(int mapNumber, List<Character> pT)
        {
            InitializeComponent();
            Setup();
            engine = new GameEngine(this, pT, mapNumber);
        }

        //Sets up all the global variables
        public void Setup()
        {
            casting = false;
            controller = new Controller();
            screenSize = controller.Resolution(this);
            leftSide = new List<CharacterPicture>();
            rightSide = new List<CharacterPicture>();
            int picSize = screenSize.Width / 24;
            for (int i = 0; i < 10; i++)
            {
                leftSide.Add(new CharacterPicture());
                SetupBoxes(leftSide[i], picSize);
                rightSide.Add(new CharacterPicture());
                SetupBoxes(rightSide[i], picSize);
            }
            PlacePictureBoxes("left", leftSide, picSize, screenSize);
            PlacePictureBoxes("right", rightSide, picSize, screenSize);
        }

        //Initializes the UI
        public void InitializeUI(string side)
        {
            abilityFrame = new PlayerAbilityFrame(screenSize);
            for (int i = 0; i < abilityFrame.abButtons.Count; i++)
            {
                Controls.Add(abilityFrame.abButtons[i]);
            }
            Controls.Add(abilityFrame);
            this.side = side;
            characterFrame = new PlayerCharacterFrame(screenSize, side);
            Controls.Add(characterFrame.characterImage);
            Controls.Add(characterFrame.healthLabel);
            Controls.Add(characterFrame.resourceLabel);
            Controls.Add(characterFrame);
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
                for (int i = 0; i < team.Count(); i++)
                {
                    leftSide[i].character = team[i];
                    leftSide[i].Image = team[i].image;
                    leftSide[i].InitializeBars();
                    Controls.Add(leftSide[i].healthBar);
                    Controls.Add(leftSide[i].resourceBar);
                    leftSide[i].Click += AssignCharacterClickFunctions;
                }
            }
            else
            {
                for (int i = 0; i < team.Count(); i++)
                {
                    rightSide[i].character = team[i];
                    rightSide[i].Image = team[i].image;
                    rightSide[i].InitializeBars();
                    rightSide[i].Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    Controls.Add(rightSide[i].healthBar);
                    Controls.Add(rightSide[i].resourceBar);
                    rightSide[i].Click += AssignCharacterClickFunctions;
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
            Character target = t.character;
            if (casting && CorrectTarget(t))
            {
                if(selectedAbility.targetCount == 1)
                {
                    engine.currentCharacter.castAbility(selectedAbility, target);
                }
                else
                {
                    List<Character> targets;
                    targets = SelectAimedTargets(target, selectedAbility.target == "ally" ? engine.playerTeam :
                        selectedAbility.target == "enemy" ? engine.enemyTeam : engine.initiativeTeam);
                    engine.currentCharacter.castAbility(selectedAbility, targets);
                }
                UpdateCharacterBars();
                engine.Manage();
            }
        }

        public void UpdateCharacterBars()
        {
            foreach (CharacterPicture Pic in leftSide)
            {
                if(Pic.character != null)
                {
                    Pic.UpdatePanelWidth();
                }
            }
            foreach (CharacterPicture Pic in rightSide)
            {
                if (Pic.character != null)
                {
                    Pic.UpdatePanelWidth();
                }
            }
        }

        public bool CorrectTarget(CharacterPicture t)
        {
            return (selectedAbility.target == "ally" && side == "left" && t.Location.X < screenSize.Width / 2) ||
                (selectedAbility.target == "enemy" && side == "left" && t.Location.X > screenSize.Width / 2) ||
                (selectedAbility.target == "ally" && side == "right" && t.Location.X > screenSize.Width / 2) ||
                (selectedAbility.target == "enemy" && side == "right" && t.Location.X < screenSize.Width / 2) ||
                selectedAbility.target == "all";
        }

        public List<Character> SelectAimedTargets(Character target, List<Character> team)
        {
            List<Character> targets = new List<Character>();
            int num = selectedAbility.targetCount;
            int pos = team.FindIndex(x => x == target);
            for(int i=0; i<team.Count; i++)
            {
                int newPos = pos + CalcPos(i);
                if(newPos >= 0 && newPos < team.Count)
                {
                    if (team[newPos].alive)
                    {
                        targets.Add(team[newPos]);
                    }
                }
            }
            return targets;
        }

        //Calculate positions in lists for multitarget abilities
        public int CalcPos(int i)
        {
            return ((i + 1) / 2) * (int)Math.Pow(-1, (double)i % 2);
        }

        //Assigns functions to the buttons: if the ability's type is anything but random, it makes the ability
        //the current ability and if it is random, it chooses random targets to attack instantly
        public void AssignAbilityButtonFunctions()
        {
            for(int i=0; i<abilityFrame.abButtons.Count; i++)
            {
                abilityFrame.abButtons[i].MouseMove += AbilityButton_MouseOver;
                abilityFrame.abButtons[i].Click += AbilityButton_Click;
            }
        }

        private void AbilityButton_MouseOver(object sender, EventArgs e)
        {
            AbilityButton abButton = (AbilityButton)sender;
            toolTip.SetToolTip(abButton, abButton.ability.ToString());
        }

        private void AbilityButton_Click(object sender, EventArgs e)
        {
            AbilityButton abButton = (AbilityButton)sender;
            Ability ab = abButton.ability;
            if (ab.abilityType == "random" && CanCast(ab))
            {
                for (int j = 0; j < ab.targetCount; j++)
                {
                    engine.currentCharacter.castAbility(ab,
                        engine.currentCharacter.SelectRandomTarget(
                            ab.target == "ally" ? engine.playerTeam :
                            ab.target == "enemy" ? engine.enemyTeam : engine.initiativeTeam));
                }
                UpdateCharacterBars();
                engine.Manage();
            }
            else
            {
                selectedAbility = ab;
                Cast();
            }
        }

        public bool CanCast(Ability ab)
        {
            return engine.currentCharacter.currResource >= ab.cost;
        }

        private void Cast()
        {
            if(CanCast(selectedAbility))
            {
                casting = true;
                //targetArrowsSetup();
            }
            else
            {
                //targetArrows = new ArrayList();
            }
        }
    }
}
