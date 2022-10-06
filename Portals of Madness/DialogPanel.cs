using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    public class DialogPanel : Panel
    {
        Controller Controller { get; set; }
        public Button ButtonNext { get; set; }
        public Button ButtonStart { get; set; }
        public Button ButtonFirstStart { get; set; }
        public Button ButtonBack { get; set; }
        public Button ButtonCont { get; set; }
        public RichTextBox DialogRTBox { get; set; }
        public PictureBox LeftCharacter { get; set; }
        public PictureBox RightCharacter { get; set; }
        public Dialogs DialogContainer { get; set; }
        public XMLCharacters XMLCharacterList { get; set; }
        public int DialogIndex { get; set; }
        public int LineIndex { get; set; }
        public int MaxLines { get; set; }
        public bool FirstTimeStartup { get; set; }
        public bool LastTimeEnd { get; set; }
        public bool ContinueDialog { get; set; }

        public DialogPanel(Controller c)
        {
            BackColor = Color.Transparent;
            Controller = c;

            try
            {
                XMLCharacterList = c.XMLOperations.CharacterDeserializer($@"../../Characters/Characters.xml");
            }
            catch
            {
                Console.WriteLine($"Characters.xml not found!");
            }

            Size tmpSize = c.SetPanelResolution(this);
            int w = tmpSize.Width;
            int h = tmpSize.Height;

            //Set the size of the buttons
            int buttonWidth = w / 5;
            int buttonHeight = h / 10;

            ButtonNext = new Button
            {
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(w / 2 - buttonWidth / 2, h - 3 * buttonHeight),
                Text = "Continue",
                TabIndex = 0,
                AutoSize = true,
                UseVisualStyleBackColor = true
            };
            ButtonNext.Click += ButtonNext_Click;
            Controls.Add(ButtonNext);

            ButtonStart = new Button
            {
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(w / 2 - buttonWidth / 2, h - 3 * buttonHeight),
                Text = "End Dialog",
                TabIndex = 1,
                AutoSize = true,
                UseVisualStyleBackColor = true,
                Visible = false
            };
            Controls.Add(ButtonStart);

            ButtonFirstStart = new Button
            {
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(w / 2 - buttonWidth / 2, h - 3 * buttonHeight),
                Text = "End Dialog",
                TabIndex = 2,
                AutoSize = true,
                UseVisualStyleBackColor = true,
                Visible = false
            };
            Controls.Add(ButtonFirstStart);

            ButtonBack = new Button
            {
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(w / 2 - buttonWidth / 2, h - 3 * buttonHeight),
                Text = "End Dialog",
                TabIndex = 3,
                AutoSize = true,
                UseVisualStyleBackColor = true,
                Visible = false
            };
            Controls.Add(ButtonBack);

            ButtonCont = new Button
            {
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(w / 2 - buttonWidth / 2, h - 3 * buttonHeight),
                Text = "End Dialog",
                TabIndex = 3,
                AutoSize = true,
                UseVisualStyleBackColor = true,
                Visible = false
            };
            Controls.Add(ButtonCont);

            DialogRTBox = new RichTextBox
            {
                Location = new Point(w * 5 / 18, h / 3),
                Size = new Size(w * 8 / 18, h / 6),
                TabIndex = 4,
                Text = "",
                ReadOnly = true,
                SelectionFont = new Font("Tahoma", 20, FontStyle.Bold),
            };
            Controls.Add(DialogRTBox);

            LeftCharacter = new PictureBox
            {
                Size = new Size(w / 6, h / 3),
                Location = new Point(w / 18, h / 6),
                BackColor = Color.Gray,
                SizeMode = PictureBoxSizeMode.StretchImage,
                TabIndex = 5,
                TabStop = false
            };
            Controls.Add(LeftCharacter);

            RightCharacter = new PictureBox
            {
                Size = new Size(w / 6, h / 3),
                Location = new Point(w - w * 4 / 18, h / 6),
                BackColor = Color.Gray,
                SizeMode = PictureBoxSizeMode.StretchImage,
                TabIndex = 6,
                TabStop = false
            };
            Controls.Add(RightCharacter);
        }

        public bool SetupDialog(int MissionNumber, int EncounterNumber, string part, string alternative)
        {
            LineIndex = 0;
            DialogIndex = -1;
            MaxLines = 0;
            LeftCharacter.Image = null;
            RightCharacter.Image = null;

            string path = $@"../../Missions/{MissionNumber}/Dialog.xml";
            try
            {
                DialogContainer = Controller.XMLOperations.DialogDeserializer(path);
            }
            catch
            {
                Console.WriteLine($"{MissionNumber}/Dialog.xml not found!");
            }

            for (int i = 0; i < DialogContainer.Dialog.Length; i++)
            {
                int ID;
                string alt = "";
                if (int.TryParse(DialogContainer.Dialog[i].Id, out _))
                {
                    ID = int.Parse(DialogContainer.Dialog[i].Id);
                }
                else
                {
                    string tmp = DialogContainer.Dialog[i].Id.Substring(1);
                    ID = int.Parse(tmp);
                    alt += DialogContainer.Dialog[i].Id.First();
                }

                if (ID == EncounterNumber && part == DialogContainer.Dialog[i].Type && alt == alternative)
                {
                    ButtonStart.Hide();
                    ButtonFirstStart.Hide();
                    ButtonBack.Hide();
                    ButtonCont.Hide();
                    ButtonNext.Show();
                    DialogIndex = i;
                    MaxLines = DialogContainer.Dialog[DialogIndex].Lines.Line.Length;
                    UpdateDialog();
                    return true;
                }
            }

            return false;
        }

        public void UpdateDialog()
        {
            string stringid = DialogContainer.Dialog[DialogIndex].Lines.Line[LineIndex].Speaker;
            var cEnum = XMLCharacterList.XmlCharacter.Where(a => a.Id.Contains(stringid)).Select(a => a).First();
            Character speaker = Controller.XMLOperations.ConvertToCharacter(cEnum);

            if (DialogContainer.Dialog[DialogIndex].Lines.Line[LineIndex].Side == "left")
            {
                LeftCharacter.Image = ImageConverter(speaker.BaseImage);
            }
            else
            {
                RightCharacter.Image = ImageConverter(speaker.BaseImage);
                RightCharacter.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
            }

            DialogRTBox.Text = $"{speaker.Name}: {DialogContainer.Dialog[DialogIndex].Lines.Line[LineIndex].Str}";
        }

        private void ButtonNext_Click(object sender, EventArgs e)
        {
            ++LineIndex;
            if(LineIndex + 1 < MaxLines)
            {
                UpdateDialog();
            }
            else
            {
                UpdateDialog();
                ButtonNext.Hide();
                if (FirstTimeStartup)
                {
                    ButtonFirstStart.Show();
                    FirstTimeStartup = false;
                }
                else if (LastTimeEnd)
                {
                    ButtonBack.Show();
                    LastTimeEnd = false;
                }
                else if (ContinueDialog)
                {
                    ButtonCont.Show();
                    ContinueDialog = false;
                }
                else
                {
                    ButtonStart.Show();
                }
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

        public void StartFirst(bool b)
        {
            FirstTimeStartup = b;
        }

        public void ContinueWithNext(bool b)
        {
            ContinueDialog = b;
        }

        public void EndLast(bool b)
        {
            LastTimeEnd = b;
        }
    }
}
