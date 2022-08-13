using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    public class PlayerCharacterFrame : Panel
    {
        public PictureBox characterImage { get; set; }
        public Label healthLabel { get; set; }
        public Label resourceLabel { get; set; }

        public PlayerCharacterFrame(Size screensize, string side)
        {
            BackColor = Color.DarkGray;
            Height = screensize.Height / 4;
            Width = screensize.Width / 8;

            characterImage = new PictureBox
            {
                Width = Width - Width / 5,
                Height = Height - Height / 4,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.DarkGray
            };

            healthLabel = new Label
            {
                ForeColor = Color.Red,
                Width = Width - Width / 5,
                Height = Height - Height * 7 / 8,
                BackColor = Color.DarkGray,
                TextAlign = ContentAlignment.MiddleCenter
            };

            resourceLabel = new Label
            {
                Width = Width - Width / 5,
                Height = Height - Height * 7 / 8,
                BackColor = Color.DarkGray,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Height = Height / 4 + characterImage.Height + healthLabel.Height + resourceLabel.Height;

            int xCoord = 0;
            if (side == "right")
            {
                xCoord = screensize.Width - Width;
            }
            int yCoord = screensize.Height - Height;

            Location = new Point(xCoord, yCoord);

            characterImage.Location = new Point(xCoord + Width / 10,
                yCoord + Height / 10);
            healthLabel.Location = new Point(xCoord + Width / 10,
                characterImage.Location.Y + characterImage.Height);
            resourceLabel.Location = new Point(xCoord + Width / 10,
                healthLabel.Location.Y + healthLabel.Height);
        }

        public void UpdateFrame(Character c)
        {
            characterImage.Image = ImageConverter(c.BaseImage);
            healthLabel.Text = $"{c.CurrentHealth}/{c.MaxHealth}";
            resourceLabel.Text = $"{c.CurrentResource}/{c.MaxResource}";
            if (c.ResourceName == "focus")
            {
                resourceLabel.ForeColor = Color.FromArgb(252, 76, 2);
            }
            else if (c.ResourceName == "rage")
            {
                resourceLabel.ForeColor = Color.FromArgb(159, 29, 53);
            }
            else
            {
                resourceLabel.ForeColor = Color.Blue;
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
                try
                {
                    image = Image.FromFile($@"../../Art/Sprites/Characters/{name}/profile.jpg");
                }
                catch
                {
                    try
                    {
                        image = Image.FromFile($@"../../Art/Sprites/Characters/{name}/base.png");
                    }
                    catch
                    {
                        image = Image.FromFile($@"../../Art/Sprites/Characters/{name}/base.jpg");
                    }
                }
            }
            return image;
        }
    }
}
