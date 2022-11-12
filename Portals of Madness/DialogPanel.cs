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
        public Button ButtonContAlt { get; set; }
        public Button ButtonFirstStart { get; set; }
        public Button ButtonBack { get; set; }
        public Button ButtonCont { get; set; }
        public Label DialogLabelBox { get; set; }
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
        public bool Alt { get; set; }
        private Point ButtonPoint { get; set; }
        private Point ButtonPointMoved { get; set; }

        public DialogPanel(Controller c)
        {
            BackColor = Color.Transparent;
            Controller = c;

            Size tmpSize = c.SetPanelResolution(this);
            int w = tmpSize.Width;
            int h = tmpSize.Height;

            //Set the size of the buttons
            int buttonWidth = w / 5;
            int buttonHeight = h / 10;

            ButtonPoint = new Point(w / 2 - buttonWidth / 2, h - 3 * buttonHeight);
            ButtonPointMoved = new Point(w / 2 - buttonWidth, h - 3 * buttonHeight);

            ButtonNext = new Button
            {
                Size = new Size(buttonWidth, buttonHeight),
                Location = ButtonPoint,
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
                Location = ButtonPoint,
                Text = "Start Mission",
                TabIndex = 1,
                AutoSize = true,
                UseVisualStyleBackColor = true,
                Visible = false
            };
            Controls.Add(ButtonStart);

            ButtonContAlt = new Button
            {
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(w / 2 + buttonWidth, h - 3 * buttonHeight),
                Text = "Skip Optional Mission",
                TabIndex = 2,
                AutoSize = true,
                UseVisualStyleBackColor = true,
                Visible = false
            };
            Controls.Add(ButtonContAlt);

            ButtonFirstStart = new Button
            {
                Size = new Size(buttonWidth, buttonHeight),
                Location = ButtonPoint,
                Text = "Start Mission",
                TabIndex = 3,
                AutoSize = true,
                UseVisualStyleBackColor = true,
                Visible = false
            };
            Controls.Add(ButtonFirstStart);

            ButtonBack = new Button
            {
                Size = new Size(buttonWidth, buttonHeight),
                Location = ButtonPoint,
                Text = "Back to Map Selection",
                TabIndex = 4,
                AutoSize = true,
                UseVisualStyleBackColor = true,
                Visible = false
            };
            Controls.Add(ButtonBack);

            ButtonCont = new Button
            {
                Size = new Size(buttonWidth, buttonHeight),
                Location = ButtonPoint,
                Text = "End Dialog",
                TabIndex = 5,
                AutoSize = true,
                UseVisualStyleBackColor = true,
                Visible = false
            };
            Controls.Add(ButtonCont);

            DialogLabelBox = new Label
            {
                Location = new Point(w * 5 / 18, h / 3),
                Size = new Size(w * 8 / 18, h / 6),
                TabIndex = 6,
                Text = "",
                BackColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                Font = new Font("Tahoma", 12, FontStyle.Bold),
            };
            Controls.Add(DialogLabelBox);

            LeftCharacter = new PictureBox
            {
                Size = new Size(w / 6, h / 3),
                Location = new Point(w / 18, h / 6),
                BackColor = Color.Gray,
                SizeMode = PictureBoxSizeMode.StretchImage,
                TabIndex = 7,
                TabStop = false
            };
            Controls.Add(LeftCharacter);

            RightCharacter = new PictureBox
            {
                Size = new Size(w / 6, h / 3),
                Location = new Point(w - w * 4 / 18, h / 6),
                BackColor = Color.Gray,
                SizeMode = PictureBoxSizeMode.StretchImage,
                TabIndex = 8,
                TabStop = false
            };
            Controls.Add(RightCharacter);
        }

        public void FillUpCharacters()
        {
            try
            {
                XMLCharacterList = (XMLCharacters)Controller.XMLOperations.GenericDeserializer<XMLCharacters>($@"../../Characters/Characters.xml");
            }
            catch
            {
                Console.WriteLine($"Characters.xml not found!");
            }
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
                DialogContainer = (Dialogs)Controller.XMLOperations.GenericDeserializer<Dialogs>(path);
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
                    ButtonContAlt.Hide();
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
            try
            {
                var cEnum = XMLCharacterList.XmlCharacter.Where(a => a.Id.Contains(stringid)).Select(a => a).First();
                Character speaker = Controller.XMLOperations.ConvertToCharacter(cEnum);

                if (DialogContainer.Dialog[DialogIndex].Lines.Line[LineIndex].Side == "left")
                {
                    LeftCharacter.Image = Controller.ImageConverter(speaker.BaseImage, "profile");
                    RightCharacter.Image = null;
                }
                else
                {
                    RightCharacter.Image = Controller.ImageConverter(speaker.BaseImage, "profile");
                    RightCharacter.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    LeftCharacter.Image = null;
                }

                DialogLabelBox.Text = $"{speaker.Name}: {DialogContainer.Dialog[DialogIndex].Lines.Line[LineIndex].Str}";
            }
            catch
            {
                DialogLabelBox.Text = $"{stringid}: {DialogContainer.Dialog[DialogIndex].Lines.Line[LineIndex].Str}";
            }
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
                    if (Alt)
                    {
                        ButtonStart.Location = ButtonPointMoved;
                        ButtonContAlt.Show();
                        Alt = false;
                    }
                }
                else
                {
                    ButtonStart.Location = ButtonPoint;
                    ButtonStart.Show();
                }
            }
        }

        public void StartFirst(bool b)
        {
            FirstTimeStartup = b;
        }

        public void ContinueWithNext(bool b)
        {
            ContinueDialog = b;
        }

        public void HasAltPath(bool b)
        {
            Alt = b;
        }

        public void EndLast(bool b)
        {
            LastTimeEnd = b;
        }
    }
}
