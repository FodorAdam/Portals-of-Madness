using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Portals_of_Madness
{
    public class GameEngine
    {
        GameForm gameForm;
        SelectionForm selectionForm;
        InfoForm infoForm;

        public GameEngine()
        {

        }

        //Set the size of the form to the resolution, then maximize it
        public Point Resolution(Form f)
        {
            f.Location = new Point(0, 0);
            int h = Screen.PrimaryScreen.WorkingArea.Height;
            int w = Screen.PrimaryScreen.WorkingArea.Width;
            f.ClientSize = new Size(w, h);
            f.WindowState = FormWindowState.Maximized;

            return new Point(w, h);
        }

        public void ChangeForm(Form f, string formChar)
        {
            f.Close();
            ShowOtherForm(formChar);
        }

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
                case ("i"):
                    infoForm = new InfoForm();
                    infoForm.ShowDialog();
                    break;
                default:
                    break;
            }
        }
    }
}
