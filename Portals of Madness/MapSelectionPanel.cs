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

        public MapSelectionPanel(Controller c)
        {
            BackColor = Color.Transparent;
            SelectedMission = 0;
            SelectedEncounter = 0;

            Size tmpSize = c.SetPanelResolution(this);
            int w = tmpSize.Width;
            int h = tmpSize.Height;

            //Set the size of the buttons
            int buttonWidth = w / 5;
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

            PictureBox MapPicture = new PictureBox
            {
                Location = new Point(367, 12),
                Size = new Size(113, 176),
                TabIndex = 3,
                TabStop = false
            };
            Controls.Add(MapPicture);

            MissionSelector = new ListBox
            {
                FormattingEnabled = true,
                Location = new Point(15, 12),
                Size = new Size(153, 147),
                TabIndex = 5
            };
            Controls.Add(MissionSelector);

            EncounterSelector = new ListBox
            {
                FormattingEnabled = true,
                Location = new Point(194, 12),
                Size = new Size(153, 147),
                TabIndex = 6
            };
            Controls.Add(EncounterSelector);

            string dirPath = $@"../../Missions/";
            int amount = Directory.GetDirectories(dirPath).Length;
            AllEncounters = new List<Encounters>();
            for (int i = 0; i < amount; i++)
            {
                string path = $@"../../Missions/{i}/Mission.xml";
                AllEncounters.Add(c.XMLOperations.MissionDeserializer(path));
                MissionSelector.Items.Add(AllEncounters[i].Name);
            }

            MissionSelector.MouseClick += SelectMission;
            EncounterSelector.MouseClick += SelectEncounter;

            RichTextBox MissionDesc = new RichTextBox
            {
                Location = new Point(12, 208),
                Size = new Size(468, 96),
                TabIndex = 8,
                Text = ""
            };
            Controls.Add(MissionDesc);
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
                foreach(var encounter in AllEncounters[index].Encounter)
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
