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
                }
                
            }
            else
            {
                for (int i = 0; i < team.Count(); i++)
                {
                    rightSide[i].character = team[i];
                    rightSide[i].pictureBox.Image = team[i].image;
                    rightSide[i].pictureBox.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }
            }
        }

        public void PlacePictureBoxes(string side, List<CharacterPicture> list, int size, Size screenSize)
        {
            int baseX = screenSize.Width / 3;
            int mult = 1;
            if(side == "right")
            {
                baseX = screenSize.Width - screenSize.Width / 3;
                mult = -1;
            }
            int baseY = screenSize.Height / 2;
            list[9].pictureBox.Location = new Point(baseX,                   baseY);
            list[0].pictureBox.Location = new Point(baseX - 2 * size * mult, baseY - size);
            list[1].pictureBox.Location = new Point(baseX - 2 * size * mult, baseY + size);
            list[2].pictureBox.Location = new Point(baseX - 4 * size * mult, baseY - 2 * size);
            list[3].pictureBox.Location = new Point(baseX - 4 * size * mult, baseY);
            list[4].pictureBox.Location = new Point(baseX - 4 * size * mult, baseY + 2 * size);
            list[5].pictureBox.Location = new Point(baseX - 6 * size * mult, baseY - 3 * size);
            list[6].pictureBox.Location = new Point(baseX - 6 * size * mult, baseY - size);
            list[7].pictureBox.Location = new Point(baseX - 6 * size * mult, baseY + size);
            list[8].pictureBox.Location = new Point(baseX - 6 * size * mult, baseY + 3 * size);
        }
    }
}
