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
        public List<AbilityButton> AbilityButtons { get; set; }

        public PlayerAbilityFrame(Size screensize)
        {
            Height = screensize.Height / 10;
            int buttonLength = Height - Height / 5;
            Width = Height *3;
            BackColor = Color.Black;
            Location = new Point((screensize.Width - Width) / 2, screensize.Height - Height * 2);
            AbilityButtons = new List<AbilityButton>
            {
                new AbilityButton(),
                new AbilityButton(),
                new AbilityButton()
            };
            for (int i = 0; i < AbilityButtons.Count; i++)
            {
                AbilityButtons[i].Height = buttonLength;
                AbilityButtons[i].Width = buttonLength;
                int extra = Height / 10;
                AbilityButtons[i].Location = new Point(Location.X + i * buttonLength + (i + 1) * extra,
                    Location.Y + Height / 10);
            }
        }

        public void UpdateButtons(Character ch)
        {
            for(int i=0; i < AbilityButtons.Count; i++)
            {
                AbilityButtons[i].UpdateButton(ch.Abilities[i]);
                if(ch.CurrentResource >= ch.Abilities[i].Cost)
                {
                    AbilityButtons[i].FlatAppearance.BorderColor = Color.Green;
                }
                else
                {
                    AbilityButtons[i].FlatAppearance.BorderColor = Color.Red;
                }
            }
        }

        public void SetVisibility(bool b)
        {
            Visible = b;
            foreach(AbilityButton button in AbilityButtons)
            {
                button.Visible = b;
            }
        }
    }
}
