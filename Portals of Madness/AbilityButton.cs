using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    public class AbilityButton : Button
    {
        public Ability ability { get; set; }
        public AbilityButton()
        {
            BackColor = Color.Purple;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderColor = Color.Black;
            FlatAppearance.BorderSize = 1;
        }

        public void UpdateButton(Ability ab)
        {
            ability = ab;
            BackgroundImage = ability.imageIcon;
        }
    }
}
