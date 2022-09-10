using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    public class Controller
    {
        public GameForm GameForm { get; set; }
        public int NextMap { get; set; }
        public List<Character> PlayerTeam { get; set; }
        public XMLOperations XMLOperations { get; set; }

        public Controller()
        {
            XMLOperations = new XMLOperations();
        }

        //Set the size of the form to the resolution, then maximize it
        public Size SetFormResolution(Form f)
        {
            f.Location = new Point(0, 0);
            Size tmpSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            f.ClientSize = tmpSize;
            f.WindowState = FormWindowState.Maximized;

            return tmpSize;
        }

        public Size SetPanelResolution(Panel p)
        {
            p.Location = new Point(0, 0);
            Size tmpSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            p.ClientSize = tmpSize;

            return tmpSize;
        }

        //Used to close the current form and swap to another
        public void ChangeForm(Form f, string formChar)
        {
            f.Close();
            ShowOtherForm(formChar);
        }

        //Swaps to another form
        public void ShowOtherForm(string formChar)
        {
            switch (formChar)
            {
                case ("g"):
                    GameForm = new GameForm(this);
                    GameForm.ShowDialog();
                    break;
                case ("g+"):
                    GameForm = new GameForm(this, NextMap, PlayerTeam);
                    GameForm.ShowDialog();
                    break;
                default:
                    break;
            }
        }

        //Sets the next map to get loaded
        public void SetNextMap(int n)
        {
            NextMap = n;
        }
    }
}
