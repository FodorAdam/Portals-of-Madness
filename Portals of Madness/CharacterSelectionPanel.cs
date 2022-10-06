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
        List<Character> CharacterList { get; set; }
        List<Character> SelectedCharacterList { get; set; }

        public CharacterSelectionPanel(Controller c)
        {
            BackColor = Color.Transparent;
            CharacterList = new List<Character>();
            SelectedCharacterList = new List<Character>();

            Size tmpSize = c.SetPanelResolution(this);
            int w = tmpSize.Width;
            int h = tmpSize.Height;

            //Set the size of the buttons
            int buttonWidth = w / 5;
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
                Location = new Point(15 + buttonWidth / 2 + (int)(buttonWidthSmall * 0.5), buttonHeightSmall),
                TabIndex = 2,
                Text = "Add",
                UseVisualStyleBackColor = true
            };
            Controls.Add(ButtonAdd);

            ButtonRemove = new Button
            {
                Size = new Size(buttonWidthSmall, buttonHeightSmall),
                Location = new Point(15 + buttonWidth / 2 + (int)(buttonWidthSmall * 0.5), buttonHeightSmall * 2),
                TabIndex = 3,
                Text = "Remove",
                UseVisualStyleBackColor = true
            };
            Controls.Add(ButtonRemove);

            CharacterPictureBox = new PictureBox
            {
                Size = new Size(buttonHeight, buttonHeight),
                Location = new Point(w / 2 - buttonHeight / 2, h - buttonHeight * 2),
                BackColor = Color.Gray,
                SizeMode = PictureBoxSizeMode.StretchImage,
                TabIndex = 4,
                TabStop = false
            };
            Controls.Add(CharacterPictureBox);

            CharacterSelector = new ListBox
            {
                FormattingEnabled = true,
                Size = new Size(buttonWidth / 2, buttonWidth / 2),
                Location = new Point(15, 15),
                TabIndex = 5
            };
            Controls.Add(CharacterSelector);

            var XMLCharacterList = c.XMLOperations.CharacterDeserializer($@"../../Characters/Characters.xml");
            var cEnum = XMLCharacterList.XmlCharacter.Where(a => a.Collectable == true).Select(a => a);
            List<XMLCharacter> XCList = new List<XMLCharacter>();
            XCList.AddRange(cEnum);

            foreach (XMLCharacter ch in XCList)
            {
                CharacterList.Add(c.XMLOperations.ConvertToCharacter(ch));
                CharacterSelector.Items.Add(ch.Name);
            }

            CharacterSelected = new ListBox
            {
                FormattingEnabled = true,
                Size = new Size(buttonWidth / 2, buttonWidth / 2),
                Location = new Point(15 + buttonWidth, 15),
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
                Location = new Point(12, 208),
                Size = new Size(468, 96),
                TabIndex = 8,
                Text = ""
            };
            Controls.Add(CharacterDesc);
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
                CharacterPictureBox.Image = ImageConverter(tmpChar.BaseImage);
                CharacterDesc.Text = tmpChar.ToString();
            }
        }

        private void ShowCharacterStats2(object sender, MouseEventArgs e)
        {
            int index = CharacterSelected.IndexFromPoint(e.Location);
            Character tmpChar = null;

            if (index != ListBox.NoMatches)
            {
                tmpChar = CharacterList.Where(a => a.Name == (string)CharacterSelected.SelectedItem).Select(a => a).First();
                CharacterPictureBox.Image = ImageConverter(tmpChar.BaseImage);
                CharacterDesc.Text = tmpChar.ToString();
            }
        }

        public Image ImageConverter(string name)
        {
            Image image = null;
            try
            {
                image = Image.FromFile($@"../../Art/Sprites/Characters/{name}/profile.png");
            }
            catch
            {
                Console.WriteLine($"{name}/profile missing");
            }
            return image;
        }

        public List<Character> GetSelectedCharacterList()
        {
            return SelectedCharacterList;
        }
    }
}
