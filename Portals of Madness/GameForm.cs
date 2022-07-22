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

        public void Setup()
        {
            controller = new Controller();
            abilityFrame = new PlayerAbilityFrame();
            characterFrame = new PlayerCharacterFrame();
            dialogBox = new DialogBox();
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

        public void SetupBoxes(CharacterPicture charPic, int picSize)
        {
            charPic.pictureBox = new PictureBox();
            charPic.pictureBox.Size = new Size(picSize, picSize);
            charPic.pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            charPic.pictureBox.BackColor = Color.Transparent;
            Controls.Add(charPic.pictureBox);
        }

        public void PlaceCharacters(List<Character> team, string side)
        {
            if ("left".Equals(side))
            {
                for (int i = 0; i < team.Count(); i++)
                {
                    leftSide[i].character = team[i];
                    leftSide[i].pictureBox.Image = team[i].image;
                    leftSide[i].InitializeBars();
                    Controls.Add(leftSide[i].healthBar);
                    Controls.Add(leftSide[i].resourceBar);
                }
                
            }
            else
            {
                for (int i = 0; i < team.Count(); i++)
                {
                    rightSide[i].character = team[i];
                    rightSide[i].pictureBox.Image = team[i].image;
                    rightSide[i].InitializeBars();
                    rightSide[i].pictureBox.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    Controls.Add(rightSide[i].healthBar);
                    Controls.Add(rightSide[i].resourceBar);
                }
            }
        }

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
                    list[i].pictureBox.Location = new Point(baseX - (j - 1) * 2 * size * mult, baseY + (j - 1) * size - k * 2 * size);
                    ++i;
                }
            }
        }
    }
}
