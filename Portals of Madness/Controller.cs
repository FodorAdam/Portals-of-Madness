using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    public class Controller
    {
        public GameForm gameForm { get; set; }
        public SelectionForm selectionForm { get; set; }
        public InfoForm infoForm { get; set; }
        public int nextMap { get; set; }
        public List<Character> playerTeam { get; set; }

        public Controller() { }

        public Controller(List<Character> pT, int nM)
        {
            nextMap = nM;
            playerTeam = pT;
        }

        //Set the size of the form to the resolution, then maximize it
        public Size Resolution(Form f)
        {
            f.Location = new Point(0, 0);
            Size tmpSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            f.ClientSize = tmpSize;
            f.WindowState = FormWindowState.Maximized;

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
                case ("s"):
                    selectionForm = new SelectionForm();
                    selectionForm.ShowDialog();
                    break;
                case ("g"):
                    gameForm = new GameForm();
                    gameForm.ShowDialog();
                    break;
                case ("g+"):
                    gameForm = new GameForm(nextMap, playerTeam);
                    gameForm.ShowDialog();
                    break;
                case ("i"):
                    infoForm = new InfoForm();
                    infoForm.ShowDialog();
                    break;
                default:
                    break;
            }
        }

        //Sets the next map to get loaded
        public void NextMap(int n)
        {
            nextMap = n;
        }
    }
}
