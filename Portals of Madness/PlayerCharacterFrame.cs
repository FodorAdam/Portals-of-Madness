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
            characterImage = new PictureBox();
            healthLabel = new Label();
            healthLabel.ForeColor = Color.Red;
            resourceLabel = new Label();
            characterImage.Width = screensize.Width / 20;
            characterImage.Height = screensize.Height / 20;
            healthLabel.Width = characterImage.Width;
            healthLabel.Height = characterImage.Height / 10;
            resourceLabel.Width = characterImage.Width;
            resourceLabel.Height = characterImage.Height / 10;
            if (side == "right")
            {
                characterImage.Location = new Point(screensize.Width - characterImage.Width,
                    screensize.Height - characterImage.Height - resourceLabel.Height - healthLabel.Height);
                healthLabel.Location = new Point(screensize.Width - characterImage.Width,
                    screensize.Height - characterImage.Height - resourceLabel.Height);
                resourceLabel.Location = new Point(screensize.Width - characterImage.Width,
                    screensize.Height - characterImage.Height);
            }
            else
            {
                characterImage.Location = new Point(0,
                    screensize.Height - characterImage.Height - resourceLabel.Height - healthLabel.Height);
                healthLabel.Location = new Point(0,
                    screensize.Height - characterImage.Height - resourceLabel.Height);
                resourceLabel.Location = new Point(0,
                    screensize.Height - characterImage.Height);
            }
        }

        public void UpdateFrame(string name, double curHealth, double maxHealth,
            double curResource, double maxResource, string resourceName)
        {
            characterImage.Image = ImageConverter(name);
            healthLabel.Text = $"{curHealth}/{maxHealth}";
            resourceLabel.Text = $"{curResource}/{maxResource}";
            if (resourceName == "focus")
            {
                resourceLabel.ForeColor = Color.FromArgb(252, 76, 2);
            }
            else if (resourceName == "rage")
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
            try
            {
                return Image.FromFile($@"../../Art/Sprites/Characters/{name}/profile.png");
            }
            catch
            {
                return Image.FromFile($@"../../Art/Sprites/Characters/{name}/base.png");
            }
        }
    }
}
