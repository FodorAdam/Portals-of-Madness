﻿using System.Drawing;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    public class CharacterPicture : PictureBox
    {
        public Character Character { get; set; }
        public int BaseBarWidth { get; set; }
        public Panel HealthBar { get; set; }
        public Panel ResourceBar { get; set; }
        private bool Initialized { get; set; }

        public CharacterPicture()
        {
            Initialized = false;
        }

        public void InitializeBars()
        {
            Initialized = true;
            HealthBar = new Panel();
            HealthBar.Height = 5;
            BaseBarWidth = Width;
            HealthBar.Width = BaseBarWidth;
            HealthBar.BackColor = Color.Red;
            ResourceBar = new Panel();
            ResourceBar.Height = 5;
            ResourceBar.Width = BaseBarWidth;
            if (Character.ResourceName == "focus")
            {
                ResourceBar.BackColor = Color.FromArgb(252, 76, 2);
            }
            else if (Character.ResourceName == "rage")
            {
                ResourceBar.BackColor = Color.FromArgb(159, 29, 53);
            }
            else if (Character.ResourceName == "mana")
            {
                ResourceBar.BackColor = Color.Blue;
            }
            else if (Character.ResourceName == "energy")
            {
                ResourceBar.BackColor = Color.Yellow;
            }
            UpdatePanelLocations();
            UpdateBars();
        }

        public void UpdatePanelLocations()
        {
            HealthBar.Location = new Point(Location.X, Location.Y + Height + 2);
            ResourceBar.Location = new Point(Location.X, Location.Y + Height + HealthBar.Height + 2);
        }

        public void UpdateBars()
        {
            if (Character.Stunned)
            {
                HealthBar.BackColor = Color.Purple;
            }
            else if (Character.WasStunned)
            {
                HealthBar.BackColor = Color.MediumVioletRed;
            }
            else
            {
                HealthBar.BackColor = Color.Red;
            }
            HealthBar.Width = (int)(Character.CurrentHealth / Character.MaxHealth * BaseBarWidth);
            ResourceBar.Width = (int)(Character.CurrentResource / (double)Character.MaxResource * BaseBarWidth);
        }

        public void HideBars()
        {
            if (Initialized)
            {
                HealthBar.Width = 0;
                HealthBar.Hide();
                ResourceBar.Width = 0;
                ResourceBar.Hide();
            }
        }
    }
}
