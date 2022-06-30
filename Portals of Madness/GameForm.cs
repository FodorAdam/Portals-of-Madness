using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Portals_of_Madness
{
    public partial class GameForm : Form
    {
        public GameEngine engine { get; set; }
        public Controller controller { get; set; }

        public GameForm()
        {
            InitializeComponent();

            engine = new GameEngine(this);
            controller = new Controller();
            controller.Resolution(this);
        }

        public GameForm(int mapNumber, List<Character> pT)
        {
            InitializeComponent();

            engine = new GameEngine(this, pT, mapNumber);
            controller = new Controller();
            controller.Resolution(this);
        }
    }
}
