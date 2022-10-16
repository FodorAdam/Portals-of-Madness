using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    internal class CharacterSelectionPanel : Panel
    {
        public Button ButtonStartMission { get; set; }
        public Button ButtonReturn { get; set; }
        public Button ButtonAdd { get; set; }
        public Button ButtonRemove { get; set; }
        public ListBox CharacterSelector { get; set; }
        public ListBox CharacterSelected { get; set; }
        public PictureBox CharacterPictureBox { get; set; }
        public RichTextBox CharacterDesc { get; set; }
        public RichTextBox CharacterStory { get; set; }
        List<Character> CharacterList { get; set; }
        List<Character> SelectedCharacterList { get; set; }
        public Controller Controller { get; set; }

        public CharacterSelectionPanel(Controller c)
        {
            Controller = c;
            BackColor = Color.Transparent;
            CharacterList = new List<Character>();
            SelectedCharacterList = new List<Character>();

            Size tmpSize = c.SetPanelResolution(this);
            int w = tmpSize.Width;
            int h = tmpSize.Height;

            //Set the size of the buttons
            int buttonWidth = w / 5;
            int pictureSize = w / 7;
            int buttonHeight = h / 10;
            int buttonWidthSmall = w / 20;
            int buttonHeightSmall = h / 20;

            ButtonStartMission = new Button
            {
                Location = new Point(w - buttonWidth * 3 / 2, h - buttonHeight * 2),
                Size = new Size(buttonWidth, buttonHeight),
                TabIndex = 0,
                Text = "Start Mission",
                UseVisualStyleBackColor = true
            };
            Controls.Add(ButtonStartMission);

            ButtonReturn = new Button
            {
                Location = new Point(buttonWidth / 2, h - buttonHeight * 2),
                Size = new Size(buttonWidth, buttonHeight),
                TabIndex = 1,
                Text = "Return to Main Menu",
                UseVisualStyleBackColor = true
            };
            Controls.Add(ButtonReturn);

            ButtonAdd = new Button
            {
                Size = new Size(buttonWidthSmall, buttonHeightSmall),
                Location = new Point(buttonHeight / 2 + pictureSize + (int)(buttonWidthSmall * 0.5), buttonHeightSmall),
                TabIndex = 2,
                Text = "Add",
                UseVisualStyleBackColor = true
            };
            Controls.Add(ButtonAdd);

            ButtonRemove = new Button
            {
                Size = new Size(buttonWidthSmall, buttonHeightSmall),
                Location = new Point(buttonHeight / 2 + pictureSize + (int)(buttonWidthSmall * 0.5), buttonHeightSmall * 2),
                TabIndex = 3,
                Text = "Remove",
                UseVisualStyleBackColor = true
            };
            Controls.Add(ButtonRemove);

            CharacterPictureBox = new PictureBox
            {
                Size = new Size(pictureSize, pictureSize),
                Location = new Point(w / 2 - pictureSize / 2, buttonHeight / 2),
                BackColor = Color.Gray,
                SizeMode = PictureBoxSizeMode.StretchImage,
                TabIndex = 4,
                TabStop = false
            };
            Controls.Add(CharacterPictureBox);

            CharacterSelector = new ListBox
            {
                FormattingEnabled = true,
                Size = new Size(pictureSize, buttonHeight * 37 / 8 + pictureSize),
                Location = new Point(buttonHeight / 2, buttonHeight / 2),
                Font = new Font("Tahoma", 12, FontStyle.Bold),
                TabIndex = 5
            };
            Controls.Add(CharacterSelector);

            CharacterSelected = new ListBox
            {
                FormattingEnabled = true,
                Size = new Size(pictureSize, buttonHeight * 37 / 8 + pictureSize),
                Location = new Point((w - pictureSize * 3 - buttonHeight / 2) / 2, buttonHeight / 2),
                Font = new Font("Tahoma", 12, FontStyle.Bold),
                TabIndex = 6
            };
            Controls.Add(CharacterSelected);

            ButtonAdd.Click += ButtonSelectCharacter;
            ButtonRemove.Click += ButtonDeSelectCharacter;
            CharacterSelector.MouseDoubleClick += SelectCharacter;
            CharacterSelected.MouseDoubleClick += DeSelectCharacter;
            CharacterSelector.MouseClick += ShowCharacterStats1;
            CharacterSelected.MouseClick += ShowCharacterStats2;

            CharacterDesc = new RichTextBox
            {
                Location = new Point(w / 2 + buttonWidth / 2, buttonHeight / 2),
                Size = new Size(w / 2 - buttonWidth + buttonHeight, pictureSize),
                TabIndex = 8,
                Text = "",
                Font = new Font("Tahoma", 12, FontStyle.Bold),
                ReadOnly = true
            };
            Controls.Add(CharacterDesc);

            CharacterStory = new RichTextBox
            {
                Location = new Point(w / 2 - pictureSize / 2, buttonHeight + pictureSize),
                Size = new Size(CharacterDesc.Width + CharacterDesc.Location.X - CharacterPictureBox.Location.X, buttonHeight * 4),
                TabIndex = 9,
                Text = "",
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                ReadOnly = true
            };
            Controls.Add(CharacterStory);
        }

        public void UpdateAllAvailableCharacters()
        {
            var XMLCharacterList = (XMLCharacters)Controller.XMLOperations.GenericDeserializer<XMLCharacters>($@"../../Characters/Characters.xml");
            var cEnum = XMLCharacterList.XmlCharacter.Where(a => a.Collectable == true).Select(a => a);
            List<XMLCharacter> XCList = new List<XMLCharacter>();
            XCList.AddRange(cEnum);

            foreach (XMLCharacter ch in XCList)
            {
                CharacterList.Add(Controller.XMLOperations.ConvertToCharacter(ch));
                CharacterSelector.Items.Add(ch.Name);
            }
        }

        private void SelectCharacter(object sender, MouseEventArgs e)
        {
            int index = CharacterSelector.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches && CharacterSelected.Items.Count < 5)
            {
                CharacterSelected.Items.Add(CharacterSelector.Items[index]);
                Character tmpChar = CharacterList.Where(a => a.Name == (string)CharacterSelector.Items[index]).Select(a => a).First();
                SelectedCharacterList.Add(tmpChar);
                CharacterSelector.Items.Remove(CharacterSelector.Items[index]);
            }
        }

        private void DeSelectCharacter(object sender, MouseEventArgs e)
        {
            int index = CharacterSelected.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                CharacterSelector.Items.Add(CharacterSelected.Items[index]);
                Character tmpChar = CharacterList.Where(a => a.Name == (string)CharacterSelected.Items[index]).Select(a => a).First();
                SelectedCharacterList.Remove(tmpChar);
                CharacterSelected.Items.Remove(CharacterSelected.Items[index]);
            }
        }

        private void ButtonSelectCharacter(object sender, EventArgs e)
        {
            if (CharacterSelector.SelectedItem != null && CharacterSelected.Items.Count < 5)
            {
                CharacterSelected.Items.Add(CharacterSelector.SelectedItem);
                Character tmpChar = CharacterList.Where(a => a.Name == (string)CharacterSelector.SelectedItem).Select(a => a).First();
                SelectedCharacterList.Add(tmpChar);
                CharacterSelector.Items.Remove(CharacterSelector.SelectedItem);
            }
        }

        private void ButtonDeSelectCharacter(object sender, EventArgs e)
        {
            if (CharacterSelected.SelectedItem != null)
            {
                CharacterSelector.Items.Add(CharacterSelected.SelectedItem);
                Character tmpChar = CharacterList.Where(a => a.Name == (string)CharacterSelected.SelectedItem).Select(a => a).First();
                SelectedCharacterList.Remove(tmpChar);
                CharacterSelected.Items.Remove(CharacterSelected.SelectedItem);
            }
        }

        private void ShowCharacterStats1(object sender, MouseEventArgs e)
        {
            int index = CharacterSelector.IndexFromPoint(e.Location);
            Character tmpChar = null;

            if (index != ListBox.NoMatches)
            {
                tmpChar = CharacterList.Where(a => a.Name == (string)CharacterSelector.SelectedItem).Select(a => a).First();
                CharacterPictureBox.Image = Controller.ImageConverter(tmpChar.BaseImage, "profile");
                CharacterDesc.Text = tmpChar.ToString();
                CharacterStory.Text = tmpChar.Story;
            }
        }

        private void ShowCharacterStats2(object sender, MouseEventArgs e)
        {
            int index = CharacterSelected.IndexFromPoint(e.Location);
            Character tmpChar = null;

            if (index != ListBox.NoMatches)
            {
                tmpChar = CharacterList.Where(a => a.Name == (string)CharacterSelected.SelectedItem).Select(a => a).First();
                CharacterPictureBox.Image = Controller.ImageConverter(tmpChar.BaseImage, "profile");
                CharacterDesc.Text = tmpChar.ToString();
                CharacterStory.Text = tmpChar.Story;
            }
        }

        public List<Character> GetSelectedCharacterList()
        {
            return SelectedCharacterList;
        }
    }
}
