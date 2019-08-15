using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormlib;

namespace graphic_MAC_problem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Timer_State.getinstance(this);
            DoubleBuffering.getinstance().setInstance(this);
            Form_input.binding(this);

            MainProgram.init(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //state s = help.getans(5, 5, 1);
            //if(s == null)
            //{
            //    MessageBox.Show("not found");
            //}
            //else
            //{
            //    MessageBox.Show("found");
            //}

            //input_visible(false);
        }
        
        public void close()
        {
            Invoke(new Action(delegate () 
            {
                Close();
            }));
            
        }
    }

}
