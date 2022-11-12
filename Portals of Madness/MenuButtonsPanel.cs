using System;
using System.Drawing;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    internal class MenuButtonsPanel : Panel
    {
        public Button ButtonNewGame { get; set; }
        public Button ButtonContinue { get; set; }
        public Button ButtonInfo { get; set; }
        public Button ButtonTest { get; set; }
        public Button ButtonLoad { get; set; }
        public Button ButtonExit { get; set; }

        public MenuButtonsPanel(Controller c)
        {
            BackColor = Color.Transparent;

            Size tmpSize = c.SetPanelResolution(this);

            int w = tmpSize.Width;
            int h = tmpSize.Height;

            //Set the size of the buttons
            int buttonWidth = w / 5;
            int buttonHeight = h / 10;

            ButtonNewGame = new Button
            {
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(w / 2 - buttonWidth / 2, h / 2 - buttonHeight / 2 - (3 * buttonHeight / 5)),
                TabIndex = 0,
                Text = "New Game",
                UseVisualStyleBackColor = true
            };

            ButtonContinue = new Button
            {
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(w / 2 - buttonWidth / 2, h / 2 - buttonHeight / 2 + (3 * buttonHeight / 5)),
                TabIndex = 1,
                Text = "Continue",
                UseVisualStyleBackColor = true
            };

            ButtonInfo = new Button
            {
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(w / 2 - buttonWidth / 2, h / 2 - buttonHeight / 2 + 3 * (3 * buttonHeight / 5)),
                TabIndex = 2,
                Text = "How to play",
                UseVisualStyleBackColor = true
            };

            ButtonTest = new Button
            {
                Size = new Size(buttonHeight, buttonHeight),
                Location = new Point(buttonHeight / 3, h / 2 - buttonHeight / 2 + 7 * (3 * buttonHeight / 5)),
                TabIndex = 2,
                Text = "T",
                UseVisualStyleBackColor = true
            };

            ButtonLoad = new Button
            {
                Size = new Size(buttonHeight, buttonHeight),
                Location = new Point(buttonHeight * 3 / 2, h / 2 - buttonHeight / 2 + 7 * (3 * buttonHeight / 5)),
                TabIndex = 2,
                Text = "L",
                UseVisualStyleBackColor = true
            };

            ButtonExit = new Button
            {
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(w / 2 - buttonWidth / 2, h / 2 - buttonHeight / 2 + 5 * (3 * buttonHeight / 5)),
                TabIndex = 2,
                Text = "Quit",
                UseVisualStyleBackColor = true
            };

            Controls.Add(ButtonNewGame);
            Controls.Add(ButtonContinue);
            Controls.Add(ButtonInfo);
            Controls.Add(ButtonLoad);
            Controls.Add(ButtonTest);
            Controls.Add(ButtonExit);
        }
    }
}
