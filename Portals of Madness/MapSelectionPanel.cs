using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System;

namespace Portals_of_Madness
{
    internal class MapSelectionPanel : Panel
    {
        public Button ButtonStartMission { get; set; }
        public Button ButtonReturn { get; set; }
        ListBox MissionSelector { get; set; }
        ListBox EncounterSelector { get; set; }
        List<Encounters> AllEncounters { get; set; }
        public int SelectedMission { get; set; }
        public int MaxEncounters { get; set; }
        public int SelectedEncounter { get; set; }
        public Controller Controller { get; set; }
        public RichTextBox MissionDesc { get; set; }
        public PictureBox MapPicture { get; set; }

        public MapSelectionPanel(Controller c)
        {
            Controller = c;
            BackColor = Color.Transparent;
            SelectedMission = 0;
            SelectedEncounter = 0;

            Size tmpSize = c.SetPanelResolution(this);
            int w = tmpSize.Width;
            int h = tmpSize.Height;

            //Set the size of the buttons
            int buttonWidth = w / 5;
            int pictureSize = w / 7;
            int buttonHeight = h / 10;

            ButtonStartMission = new Button
            {
                Location = new Point(w - buttonWidth * 3 / 2, h - buttonHeight * 2),
                Size = new Size(buttonWidth, buttonHeight),
                TabIndex = 0,
                Text = "Select Mission",
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

            MapPicture = new PictureBox
            {
                Location = new Point(w / 2, buttonHeight / 2),
                Size = new Size(pictureSize, pictureSize),
                TabIndex = 2,
                BackColor = Color.Gray,
                TabStop = false
            };
            Controls.Add(MapPicture);

            MissionSelector = new ListBox
            {
                FormattingEnabled = true,
                Size = new Size(pictureSize, buttonHeight * 37 / 8 + pictureSize),
                Location = new Point(buttonHeight / 2, buttonHeight / 2),
                Font = new Font("Tahoma", 12, FontStyle.Bold),
                TabIndex = 3
            };
            Controls.Add(MissionSelector);

            EncounterSelector = new ListBox
            {
                FormattingEnabled = true,
                Size = new Size(pictureSize * 2, buttonHeight * 37 / 8 + pictureSize),
                Location = new Point(pictureSize + buttonHeight, buttonHeight / 2),
                Font = new Font("Tahoma", 12, FontStyle.Bold),
                TabIndex = 4
            };
            Controls.Add(EncounterSelector);

            MissionSelector.MouseClick += SelectMission;
            EncounterSelector.MouseClick += SelectEncounter;

            MissionDesc = new RichTextBox
            {
                Location = new Point(w / 2, buttonHeight + pictureSize),
                Size = new Size(pictureSize * 3, pictureSize),
                TabIndex = 5,
                Font = new Font("Tahoma", 12, FontStyle.Bold),
                ReadOnly = true,
                Text = ""
            };
            Controls.Add(MissionDesc);
        }

        public void UpdateAllAvailableMissions()
        {
            MissionSelector.Items.Clear();
            string dirPath = $@"../../Missions/";
            int amount = Directory.GetDirectories(dirPath).Length;
            AllEncounters = new List<Encounters>();
            for (int i = 0; i < amount; i++)
            {
                string path = $@"../../Missions/{i}/Mission.xml";
                AllEncounters.Add((Encounters)Controller.XMLOperations.GenericDeserializer<Encounters>(path));
                MissionSelector.Items.Add(AllEncounters[i].Name);
            }
        }

        public int GetMissionNumber()
        {
            return SelectedMission;
        }

        public int GetMaxEncounterNumber()
        {
            return MaxEncounters;
        }

        public int GetEncounterNumber()
        {
            return SelectedEncounter;
        }

        private void SelectMission(object sender, MouseEventArgs e)
        {
            MaxEncounters = 0;
            int index = MissionSelector.IndexFromPoint(e.Location);
            EncounterSelector.Items.Clear();
            if (index != ListBox.NoMatches)
            {
                SelectedMission = index;
                MissionDesc.Text = AllEncounters[index].Lore;
                foreach (var encounter in AllEncounters[index].Encounter)
                {
                    EncounterSelector.Items.Add(encounter.Name);
                    ++MaxEncounters;
                }
            }
        }

        private void SelectEncounter(object sender, MouseEventArgs e)
        {
            int index = EncounterSelector.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                SelectedEncounter = index;
            }
        }
    }
}
