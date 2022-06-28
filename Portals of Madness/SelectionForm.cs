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
    public partial class SelectionForm : Form
    {
        GameEngine engine;

        public SelectionForm()
        {
            InitializeComponent();

            engine = new GameEngine();
            engine.Resolution(this);
        }
    }
}
