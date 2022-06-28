﻿using System;
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
        GameEngine engine;

        public GameForm()
        {
            InitializeComponent();

            engine = new GameEngine();
            engine.Resolution(this);
        }
    }
}