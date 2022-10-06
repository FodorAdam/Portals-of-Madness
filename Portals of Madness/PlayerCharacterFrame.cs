using System;
using System.Drawing;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    public class PlayerCharacterFrame : Panel
    {
        public PictureBox CharacterImage { get; set; }
        public Label HealthLabel { get; set; }
        public Label ResourceLabel { get; set; }
        public string Side { get; set; }

        public PlayerCharacterFrame(Size screensize, string side)
        {
            BackColor = Color.DarkGray;
            BorderStyle = BorderStyle.Fixed3D;
            Side = side;
            Height = screensize.Height / 4;
            Width = screensize.Width / 8;

            CharacterImage = new PictureBox
            {
                Width = Width - Width / 5,
                Height = Height - Height / 4,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderStyle = BorderStyle.Fixed3D,
                BackColor = Color.DarkGray
            };

            HealthLabel = new Label
            {
                ForeColor = Color.Red,
                Width = Width - Width / 5,
                Height = Height - Height * 7 / 8,
                BackColor = Color.DarkGray,
                TextAlign = ContentAlignment.MiddleCenter
            };

            ResourceLabel = new Label
            {
                Width = Width - Width / 5,
                Height = Height - Height * 7 / 8,
                BackColor = Color.DarkGray,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Height = Height / 4 + CharacterImage.Height + HealthLabel.Height + ResourceLabel.Height;

            int xCoord = 0;
            if (side == "right")
            {
                xCoord = screensize.Width - Width;
            }
            int yCoord = screensize.Height - Height;

            Location = new Point(xCoord, yCoord);

            CharacterImage.Location = new Point(xCoord + Width / 10,
                yCoord + Height / 10);
            HealthLabel.Location = new Point(xCoord + Width / 10,
                CharacterImage.Location.Y + CharacterImage.Height);
            ResourceLabel.Location = new Point(xCoord + Width / 10,
                HealthLabel.Location.Y + HealthLabel.Height);
        }

        public void UpdateFrame(Character c)
        {
            CharacterImage.Image = ImageConverter(c.BaseImage);

            HealthLabel.Text = $"{c.CurrentHealth}/{c.MaxHealth}";
            ResourceLabel.Text = $"{c.CurrentResource}/{c.MaxResource}";
            if (c.ResourceName == "focus")
            {
                ResourceLabel.ForeColor = Color.FromArgb(252, 76, 2);
            }
            else if (c.ResourceName == "rage")
            {
                ResourceLabel.ForeColor = Color.FromArgb(159, 29, 53);
            }
            else
            {
                ResourceLabel.ForeColor = Color.Blue;
            }
        }

        public Image ImageConverter(string name)
        {
            Image image = null;
            try
            {
                image = Image.FromFile($@"../../Art/Sprites/Characters/{name}/profile.png");
            }
            catch
            {
                Console.WriteLine($"{name}/profile missing");
            }
            return image;
        }
    }
}
