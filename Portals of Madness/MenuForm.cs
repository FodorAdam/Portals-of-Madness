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
    public partial class MenuForm : Form
    {
        Controller controller;

        public MenuForm()
        {
            InitializeComponent();

            controller = new Controller();
            Point tmpPoint = controller.Resolution(this);
            int w = tmpPoint.X;
            int h = tmpPoint.Y;

            //Set the size of the buttons
            int buttonWidth = w / 5;
            int buttonHeight = h / 10;

            buttonNewGame.Size = new Size(buttonWidth, buttonHeight);
            buttonNewGame.Location = new Point
                (w / 2 - buttonNewGame.Width / 2, h / 2 - buttonNewGame.Height / 2 - (3 * buttonHeight / 5));

            buttonContinue.Size = new Size(buttonWidth, buttonHeight);
            buttonContinue.Location = new Point
                (w / 2 - buttonContinue.Width / 2, h / 2 - buttonContinue.Height / 2 + (3 * buttonHeight / 5));
            //TODO: If there is no saved game, hide the continue
            /*if (true)
            {
                buttonContinue.Visible = false;
            }*/

            buttonInfo.Size = new Size(buttonWidth, buttonHeight);
            buttonInfo.Location = new Point
                (w / 2 - buttonInfo.Width / 2, h / 2 - buttonInfo.Height / 2 + 3 * (3 * buttonHeight / 5));
        }

        //Start a new game by starting the tutorial mission
        private void buttonNewGame_Click(object sender, EventArgs e)
        {
            controller.NextMap(0);
            controller.ShowOtherForm("g");
        }

        //Continue the game
        private void buttonContinue_Click(object sender, EventArgs e)
        {
            controller.ShowOtherForm("s");
        }

        //Show the how to play form
        private void buttonInfo_Click(object sender, EventArgs e)
        {
            controller.ShowOtherForm("i");
        }
    }
}
