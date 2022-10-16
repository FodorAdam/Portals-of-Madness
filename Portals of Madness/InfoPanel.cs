using System;
using System.Drawing;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    public partial class InfoPanel : Panel
    {
        public Button ButtonBack { get; set; }

        public InfoPanel(Controller c)
        {
            BackColor = Color.Transparent;

            Size tmpSize = c.SetPanelResolution(this);
            int w = tmpSize.Width;
            int h = tmpSize.Height;

            //Set the size of the buttons
            int buttonWidth = w / 5;
            int buttonHeight = h / 10;

            ButtonBack = new Button
            {
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(w / 2 - buttonWidth / 2, h - 3 * buttonHeight),
                Text = "Back",
                TabIndex = 0,
                AutoSize = true,
                UseVisualStyleBackColor = true
            };
            Controls.Add(ButtonBack);

            Label labelTitle = new Label
            {
                Location = new Point(w / 2 - buttonWidth / 2, buttonHeight / 2),
                Text = "How to play",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Tahoma", 24, FontStyle.Bold),
                Size = new Size(buttonWidth, buttonHeight),
                BackColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                TabIndex = 2
            };
            Controls.Add(labelTitle);

            Label labelExplanation = new Label
            {
                Location = new Point(w / 4, buttonHeight * 2),
                MaximumSize = new Size(w / 2, 0),
                Text = "The game is divided into several turns. Every living character on the " +
                "field gets to use an ability. Every character has at most 3 abilities. The first one is " +
                "always free, the other two always costs some the character's unique resource. There are 3 " +
                "types of resource in the game: \n" +
                "-Rage: Generates faster the more health a character is missing.\n" +
                "-Focus: Generates faster the more health a character isn't missing.\n" +
                "-Mana: Generates at a steady rate.\n" +
                "Abilites can be used in multiple ways. Some you just click on and it casts automatically. " +
                "Others need you to pick a target after you click on them. Some are single target, some hit " +
                "multiple enemies. You can hover over all of them to see what they do. " +
                "Every character has a speed stat. You can see who will act next on the top of the screen.",
                AutoSize = true,
                Font = new Font("Tahoma", 12, FontStyle.Bold),
                Size = new Size(35, 20),
                TabIndex = 3,
                BackColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D
            };
            Controls.Add(labelExplanation);
        }
    }
}
