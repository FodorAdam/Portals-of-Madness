using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    public class PlayerAbilityFrame : Panel
    {
        public List<AbilityButton> AbilityButtons { get; set; }

        public PlayerAbilityFrame(Size FrameSize)
        {
            Size = FrameSize;
            int buttonLength = Height - Height / 5;
            BackColor = Color.Black;
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
            }
        }

        public void UpdateButtons(Character ch)
        {
            for(int i=0; i < AbilityButtons.Count; i++)
            {
                AbilityButtons[i].UpdateButton(ch.Abilities[i]);
                if(ch.CurrentResource >= ch.Abilities[i].Cost || ch.Abilities[i].Cost == 0)
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
