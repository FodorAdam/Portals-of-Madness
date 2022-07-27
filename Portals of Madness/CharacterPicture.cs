using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    public class CharacterPicture
    {
        public Character character { get; set; }
        public PictureBox pictureBox { get; set; }
        public int baseBarWidth { get; set; }
        public Panel healthBar { get; set; }
        public Panel resourceBar { get; set; }

        public CharacterPicture() { }

        public CharacterPicture(Character c, PictureBox p)
        {
            character = c;
            pictureBox = p;
            InitializeBars();
        }

        public void InitializeBars()
        {
            healthBar = new Panel();
            healthBar.Height = 5;
            baseBarWidth = pictureBox.Width;
            healthBar.Width = baseBarWidth;
            healthBar.BackColor = Color.Red;
            resourceBar = new Panel();
            resourceBar.Height = 5;
            resourceBar.Width = baseBarWidth;
            if (character.resourceName == "focus")
            {
                resourceBar.BackColor = Color.FromArgb(252, 76, 2);
            }
            else if (character.resourceName == "rage")
            {
                resourceBar.BackColor = Color.FromArgb(159, 29, 53);
            }
            else
            {
                resourceBar.BackColor = Color.Blue;
            }
            UpdatePanelLocations();
            UpdatePanelWidth();
        }

        public void UpdatePanelLocations()
        {
            healthBar.Location = new Point(pictureBox.Location.X, pictureBox.Location.Y + pictureBox.Height + 2);
            resourceBar.Location = new Point(pictureBox.Location.X, pictureBox.Location.Y + pictureBox.Height + healthBar.Height + 2);
        }

        public void UpdatePanelWidth()
        {
            healthBar.Width = (int)((character.currHealth / character.maxHealth) * baseBarWidth);
            resourceBar.Width = (int)(((double)character.currResource / (double)character.maxResource) * baseBarWidth);
        }
    }
}
