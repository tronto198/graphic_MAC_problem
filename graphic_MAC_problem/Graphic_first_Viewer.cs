using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormlib;
using System.Drawing;
using System.Windows.Forms;

namespace graphic_MAC_problem
{
    abstract class Graphic_object
    {
        protected double x;
        protected double y;
        protected DoubleBuffering db = DoubleBuffering.getinstance();
        bool drawing = false;

        abstract protected void Draw();

        public virtual void display()
        {
            if (drawing) return;

            drawing = true;
            db.callback_work += Draw;
        }
        public virtual void undisplay()
        {
            if (!drawing) return;

            drawing = false;
            db.callback_work -= Draw;
        }

        public virtual void setLocation(double x_, double y_)
        {
            x = x_;
            y = y_;
        }

        public virtual void addx(double dx)
        {
            x += dx;
        }
        public virtual void addy(double dy)
        {
            y += dy;
        }
    }

    class Graphic_first_Viewer : Graphic_object
    {
        GTextbox titletb = new GTextbox();
        GTextbox entertb = new GTextbox();
        GnumberTextbox[] MACsettingBox = new GnumberTextbox[3];
        const int center = 420;
        int pointer = 0;
        bool focus = false;

        public Graphic_first_Viewer()
        {
            titletb.setFont(new Font("AR CENA", 32, FontStyle.Bold));
            titletb.setLocation(center, 130);
            titletb.setstring("M & C");
            entertb.setFont(new Font("AR CENA", 16));
            entertb.setLocation(center, 220 + 45 * 4);
            entertb.setstring("Enter to start");
            for(int i = 0;i < 3; i++)
            {
                MACsettingBox[i] = new GnumberTextbox();
                MACsettingBox[i].setFont(new Font("휴면편지체", 20));
                MACsettingBox[i].setLocation(center, 220 + 45 * i);
            }

            MACsettingBox[0].init(3);
            MACsettingBox[1].init(3);
            MACsettingBox[2].init(2);
            MACsettingBox[0].setstring("선교사 : ");
            MACsettingBox[1].setstring("식인종 : ");
            MACsettingBox[2].setstring("최대 : ");

        }

        public void init()
        {

            MACsettingBox[0].focus(true);
            MACsettingBox[1].focus(false);
            MACsettingBox[2].focus(false);
            pointer = 0;
        }

        void keyinput(Keys k)
        {
            if(k == Keys.Up)
            {
                MACsettingBox[pointer--].focus(false);
                if (pointer < 0) pointer = 0;
                MACsettingBox[pointer].focus(true);
            }
            else if (k == Keys.Down)
            {
                MACsettingBox[pointer++].focus(false);
                if (pointer > 2) pointer = 2;
                MACsettingBox[pointer].focus(true);
            }
            else if(k == Keys.Enter)
            {
                entered();
            }
            else if(k == Keys.Escape)
            {
                MainProgram.close();
            }
            else
            {
                MACsettingBox[pointer].keyinput(k);
            }
        }

        void entered()
        {
            int m = MACsettingBox[0].getvariable();
            int c = MACsettingBox[1].getvariable();
            int boat = MACsettingBox[2].getvariable();

            MainProgram.data_in(m, c, boat);
        }

        public void focusd(bool b)
        {
            if (focus == b) return;
            focus = b;
            if (b)
            {
                Key_input.Key_in += keyinput;
            }
            else
            {
                Key_input.Key_in -= keyinput;
            }
        }

        protected override void Draw()
        {
            
        }

        public override void display()
        {
            base.display();
            titletb.display();
            for (int i = 0; i < 3; i++)
            {
                MACsettingBox[i].display();
            }
            entertb.display();
        }

        public override void undisplay()
        {
            base.undisplay();
            titletb.undisplay();
            for(int i = 0; i < 3; i++)
            {
                MACsettingBox[i].undisplay();
            }
            entertb.undisplay();
        }
    }

    class GTextbox : Graphic_object
    {
        protected Font font = new Font("굴림", 13);
        protected Brush brush = new SolidBrush(Color.Black);
        protected StringFormat format = new StringFormat();
        protected string str = "";


        public GTextbox()
        {
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
        }

        override protected void Draw()
        {
            db.getGraphics.DrawString(str, font, brush, (float)x, (float)y, format);
        }

        public void setstring(string str)
        {
            this.str = str;
        }
        public void setFont(Font font)
        {
            this.font = font;
        }
        
    }

    class GnumberTextbox : GTextbox
    {
        int min = 0;
        int variable = 0;
        bool focused = false;
        string var_str = "";


        public void init(int v, int min = 2)
        {
            variable = v;
            this.min = min;
            
        }

        new public void setstring(string str)
        {
            base.setstring(str);
            if (focused) { this.var_str = "<  " + str + variable + "  >"; }
            else { this.var_str = str + variable; }
        }

        override protected void Draw()
        {
            db.getGraphics.DrawString(var_str, font, brush, (float)x, (float)y, format);
        }
        internal void keyinput(Keys k)
        {
            if (!focused) return;
            if(k == Keys.Left)
            {
                variable--;
                if (variable < min) variable = min;
            }
            else if(k == Keys.Right)
            {
                variable++;
            }

            setstring(str);
        }

        public void focus(bool b)
        {
            if (focused == b) return;
            focused = b;
            if (b)
            {
                font = new Font(font.FontFamily, font.Size + 2, FontStyle.Bold);
            }
            else
            {
                font = new Font(font.FontFamily, font.Size - 2, FontStyle.Regular);
            }
            setstring(str);
        }
        public int getvariable() { return variable; }
    }
}
