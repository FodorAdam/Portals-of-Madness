using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    public class DialogBox : Panel
    {
        public PictureBox leftCharacterImage { get; set; }
        public PictureBox rightCharacterImage { get; set; }
        public TextBox dialogTextBox { get; set; }
        public Button nextDialogButton { get; set; }
        public string[] dialogs { get; set; }
        public string[] characters { get; set; }
        public string playerSide { get; set; }
        public int index { get; set; }

        public DialogBox(Size screensize)
        {
            nextDialogButton.Click += nextDialogButton_ClickEvent;
        }

        public void SetupNewStuff(string dialog, string charSet, string ps)
        {
            Visible = true;
            playerSide = ps;
            index = 0;
            if (dialog != "")
            {
                dialogs = dialog.Split('*');
                dialogTextBox.Text = dialogs[index];
                characters = charSet.Split('*');
                if (playerSide == "left")
                {
                    leftCharacterImage.Image = ImageConverter(characters[index]);
                }
                else
                {
                    rightCharacterImage.Image = ImageConverter(characters[index]);
                }
            }
        }

        public Image ImageConverter(string name)
        {
            try
            {
                return Image.FromFile($@"../../Art/Sprites/Characters/{name}/profile.png");
            }
            catch
            {
                return Image.FromFile($@"../../Art/Sprites/Characters/{name}/base.png");
            }
        }

        private void nextDialogButton_ClickEvent(object sender, EventArgs e)
        {
            ++index;
            if (index < dialogs.Length)
            {
                dialogTextBox.Text = dialogs[index];
                if ((playerSide == "left" && index % 2 == 0) || (playerSide == "right" && index % 2 == 1))
                {
                    leftCharacterImage.Image = ImageConverter(characters[index]);
                }
                else
                {
                    rightCharacterImage.Image = ImageConverter(characters[index]);
                }
            }
            else
            {
                Visible = false;
            }
        }
    }
}
