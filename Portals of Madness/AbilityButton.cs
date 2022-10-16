using System.Drawing;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    public class AbilityButton : Button
    {
        public Ability Ab { get; set; }
        public AbilityButton()
        {
            BackColor = Color.Purple;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderColor = Color.Black;
            FlatAppearance.BorderSize = 3;
            BackgroundImageLayout = ImageLayout.Stretch;
        }

        public void UpdateButton(Ability ab)
        {
            Ab = ab;
            BackgroundImage = Ab.IconImage;
        }
    }
}
