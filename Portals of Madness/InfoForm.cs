using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    public partial class InfoForm : Form
    {
        Controller controller;
        public InfoForm()
        {
            InitializeComponent();

            controller = new Controller();
            Size tmpSize = controller.Resolution(this);
            int w = tmpSize.Width;
            int h = tmpSize.Height;

            //Set the size of the buttons
            int buttonWidth = w / 5;
            int buttonHeight = h / 10;

            buttonBack.Size = new Size(buttonWidth, buttonHeight);
            buttonBack.Location = new Point
                (w / 2 - buttonBack.Width / 2, h - 3 * buttonBack.Height);

            labelTitle.Location = new Point(w / 4, 10);
            labelExplanation.Location = new Point(w / 4, h / 15);
            labelExplanation.MaximumSize = new Size(w / 2, 0);
            labelExplanation.Text="The game is divided into several turns. Every living character on the" +
                "field gets to use an ability. Every character has at most 3 abilities. The first one is" +
                "always free, the other two always costs some the character's unique resource. There are 3" +
                "types of resource in the game:" +
                "-Rage: Generates faster the more health a character is missing." +
                "-Focus: Generates faster the more health a character isn't missing." +
                "-Mana: Generates at a steady rate." +
                "Abilites can be used in multiple ways. Some you just click on and it casts automatically." +
                "Others need you to pick a target after you click on them. Some are single target, some hit" +
                "multiple enemies. You can hover over all of them to see what they do." +
                "Every character has a speed stat. You can see who will act next on the top of the screen.";
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            controller.ChangeForm(this, "");
        }
    }
}
