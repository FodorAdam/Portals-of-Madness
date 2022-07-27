using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    public class PlayerAbilityFrame : Panel
    {
        public List<AbilityButton> abButtons { get; set; }

        public PlayerAbilityFrame(Size screensize)
        {
            Width = screensize.Width / 10;
            Height = screensize.Height / 10;
            Location = new Point((screensize.Width - Width) / 2, screensize.Height - Height);
            int buttonLength = Height;
            abButtons = new List<AbilityButton>();
            abButtons.Add(new AbilityButton());
            abButtons.Add(new AbilityButton());
            abButtons.Add(new AbilityButton());
            for(int i = 0; i < abButtons.Count; i++)
            {
                abButtons[i].Height = buttonLength;
                abButtons[i].Width = buttonLength;
                abButtons[i].Location = new Point(Location.X + i * buttonLength + 5, Location.Y);
            }
        }

        public void UpdateButtons(Character ch)
        {
            abButtons[0].UpdateButton(ch.ability1);
            abButtons[1].UpdateButton(ch.ability2);
            abButtons[2].UpdateButton(ch.ability3);
        }
    }
}
