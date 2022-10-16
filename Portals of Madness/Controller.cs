using System;
using System.Drawing;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    public class Controller
    {
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

        public Image ImageConverter(string name, string what)
        {
            Image image = null;
            try
            {
                image = Image.FromFile($@"../../Art/Sprites/Characters/{name}/{what}.png");
            }
            catch
            {
                Console.WriteLine($"{name}/{what} is missing");
            }
            return image;
        }
    }
}
