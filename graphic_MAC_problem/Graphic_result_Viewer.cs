using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using WinFormlib;
using System.Windows.Forms;

namespace graphic_MAC_problem
{
    class Graphic_result_Viewer : Graphic_object
    {
        Queue<Motion> motionq = new Queue<Motion>();
        Brush riverbrush = new SolidBrush(Color.DarkBlue);

        Oneside leftside = new Oneside();
        Oneside rightside = new Oneside();
        Boat boat = new Boat();
        Task t = null;

        bool focused = false;
        bool paused = false;
        GTextbox pausestr = new GTextbox();
        GTextbox resetstr = new GTextbox();
        GTextbox countstr = new GTextbox();

        int count = 0;

        public Graphic_result_Viewer()
        {
            leftside.setLocation(0, 400);
            rightside.setLocation(660, 400);
            boat.setLocation(240, 400);
            pausestr.setFont(new Font("AR CENA", 20));
            resetstr.setFont(new Font("AR CENA", 20));
            countstr.setFont(new Font("AR CENA", 20));
            pausestr.setstring("Pause : Space");
            resetstr.setstring("Reset : Enter");
            countstr.setstring("0");
            pausestr.setLocation(420, 20);
            resetstr.setLocation(420, 50);
            countstr.setLocation(420, 80);
        }

        protected override void Draw()
        {
            DoubleBuffering.getinstance().getGraphics.FillRectangle(riverbrush, 180, 420, 550, 100);
        }

        public void init(List<int[]> process)
        {
            count = 0;
            countstr.setstring("0");
            int[] startstate = process[0];
            for(int i = 0;i < startstate[0]; i++)
            {
                Missonary m = new Missonary();
                leftside.add(m);
            }
            for(int i = 0;i < startstate[1]; i++)
            {
                Cannibals c = new Cannibals();
               
                leftside.add(c);
            }

            leftside.addend();
            rightside.addend();


            for(int i = 0; i < process.Count - 1; i++)
            {
                Motion m = new Motion();
                m.settarget(boat);
                m.setmax(100);
                double v_x = 230.0 / 100.0;
                int[] state = process[i];
                int[] nextstate = process[i + 1];
                if(state[4] == 1)
                {
                    v_x = -v_x;
                }

                m.setmove(v_x, 0);
                m.setstart(() =>
                {
                    rideboat(state, nextstate);
                });
                m.setend(new Action(delegate ()
                {
                    motionend(state);
                }));

                motionq.Enqueue(m);
            }
            
            
        }

        public void start()
        {
            if(t != null)
            {

            }
            t = Task.Factory.StartNew(() =>
            {
                motionq.Dequeue().start();

            });
        }
        public void clear()
        {
            motionq.Clear();
            leftside.clear();
            rightside.clear();
            boat.clear();
            t.Dispose();
            boat.setLocation(240, 400);

        }

        void motionend(int[] state)
        {
            getoffboat(state[4]);
            while (paused) ;

            if (!focused) clear();
            if (motionq.Count != 0)
            {
                motionq.Dequeue().start();
            }
            else
            {

            }
            count++;
            countstr.setstring("" + count);
        }
        void lastend()
        {

        }


        void rideboat(int[] state, int[] nextstate)
        {
            int movemis;
            int movecan;
            if (state[4] == 0)
            {
                movemis = state[0] - nextstate[0];
                movecan = state[1] - nextstate[1];

                for (int j = 0; j < movemis; j++)
                {
                    Graphic_person p = leftside.getmis();
                    boat.add(p);
                }
                for (int j = 0; j < movecan; j++)
                {
                    Graphic_person p = leftside.getcan();
                    boat.add(p);
                }
            }
            else
            {
                movemis = state[2] - nextstate[2];
                movecan = state[3] - nextstate[3];

                for (int j = 0; j < movemis; j++)
                {
                    Graphic_person p = rightside.getmis();
                    boat.add(p);
                }
                for (int j = 0; j < movecan; j++)
                {
                    Graphic_person p = rightside.getcan();
                    boat.add(p);
                }
            }
            boat.addend();
        }

        void getoffboat(int state)
        {
            List<Graphic_person> a = boat.getall();

            if(state == 0)
            {
                foreach(Graphic_person p in a)
                {
                    rightside.add(p);
                }
                rightside.addend();
            }
            else
            {
                foreach (Graphic_person p in a)
                {
                    leftside.add(p);
                }
                leftside.addend();
            }
            
        }
        

        public override void display()
        {
            base.display();
            leftside.display();
            rightside.display();
            boat.display();
            resetstr.display();
            pausestr.display();
            countstr.display();

            Key_input.Key_in += keyin;
        }

        public override void undisplay()
        {
            base.undisplay();
            leftside.undisplay();
            rightside.undisplay();
            boat.undisplay();
            resetstr.undisplay();
            pausestr.undisplay();
            countstr.undisplay();

            Key_input.Key_in -= keyin;
        }


        public void focus(bool b)
        {
            focused = b;
        }

        void keyin(Keys k)
        {
            if (!focused) return;

            if(k == Keys.Enter)
            {

                MainProgram.end();
            }
            else if(k == Keys.Space)
            {
                paused = !paused;
            }
        }
    }

    abstract class Graphic_carrier : Graphic_object
    {
        List<Graphic_person> carringlist = new List<Graphic_person>();
        Stack<Graphic_person> st_mis = new Stack<Graphic_person>();
        Stack<Graphic_person> st_can = new Stack<Graphic_person>();
        List<int> dd = new List<int>();

        int miss_x = 25;
        int cann_x = 100;
        

        public void add(Graphic_person p)
        {
            if(p is Missonary)
            {
                p.setLocation(
                       (float)x + miss_x, (float)y - p.getsize().Height - st_mis.Count * 15);
                st_mis.Push(p);
            }
            else
            {
                p.setLocation(
                    (float)x + cann_x, (float)y - p.getsize().Height - st_can.Count * 15);
                st_can.Push(p);
            }
            
            carringlist.Add(p);
            
        }

        public Missonary getmis()
        {
            Missonary m = st_mis.Pop() as Missonary;
            carringlist.Remove(m);
            return m;
        }
        public Cannibals getcan()
        {
            Cannibals c = st_can.Pop() as Cannibals;
            carringlist.Remove(c);
            return c;
        }

        public List<Graphic_person> getall()
        {
            List<Graphic_person> ans = carringlist;
            carringlist = new List<Graphic_person>();
            st_mis.Clear();
            st_can.Clear();
            return ans;
        }

        public override void display()
        {
            base.display();
            foreach (Graphic_person g in carringlist)
            {
                g.display();
            }

        }

        public override void undisplay()
        {
            base.undisplay();
            foreach(Graphic_person g in carringlist)
            {
                g.undisplay();
            }
        }

        public override void addx(double dx)
        {
            base.addx(dx);
            foreach(Graphic_person p  in carringlist)
            {
                p.addx(dx);
            }
        }
        public void clear()
        {
            carringlist.Clear();
            st_mis.Clear();
            st_can.Clear();
        }
        public void addend()
        {
            for(int i = 0;i < carringlist.Count; i++)
            {
                carringlist[i].undisplay();
                carringlist[i].display();
            }
        }


    }
    abstract class Graphic_person : Graphic_object
    {
        protected Pen pen = new Pen(new SolidBrush(Color.Black));
        Font font = new Font("AR CENA", 18, FontStyle.Bold);
        protected Brush personbrush;
        Brush namebrush = new SolidBrush(Color.Black);
        protected string name;
        Size personsize = new Size(50, 50);
        
        public Size getsize() { return personsize; }

        protected override void Draw()
        {
            db.getGraphics.FillRectangle(personbrush, (float)x, (float)y, personsize.Width, personsize.Height);
            db.getGraphics.DrawRectangle(pen, (float)x, (float)y, personsize.Width, personsize.Height);
            db.getGraphics.DrawString(name, font, namebrush, (float)x + 20, (float)y + 20);
        }
    }

    class Missonary : Graphic_person
    {
        public Missonary()
        {
            personbrush = new SolidBrush(Color.BlueViolet);
            name = "M";
        }
    }
    class Cannibals : Graphic_person
    {
        public Cannibals()
        {
            name = "C";
            personbrush = new SolidBrush(Color.IndianRed);
        }
    }


    class Oneside : Graphic_carrier
    {
        //250
        int x_size = 200;
        int y_size = 150;
        Brush sidebrush = new SolidBrush(Color.Green);
        protected override void Draw()
        {
            db.getGraphics.FillRectangle(sidebrush, (float)x, (float)y, x_size, y_size);
        }

    }
    class Boat : Graphic_carrier
    {
        int x_size = 150;
        int y_size = 50;
        int edge = 30;
        Brush boatbrush = new SolidBrush(Color.Orange);
        protected override void Draw()
        {
            db.getGraphics.FillPolygon(boatbrush, new PointF[4] {
                new PointF((float)x - edge, (float)y),
                new PointF((float)x + x_size + edge, (float)y),
                new PointF((float)x + x_size, (float)y + y_size),
                new PointF((float)x , (float)y + y_size)});
                //FillRectangle(boatbrush, Location.X, Location.Y, x_size, y_size);
        }
    }

    class Motion
    {
        Motion nextMotion = null;
        int maxcount = 0;
        int count = 0;
        internal double v_x = 0, v_y = 0;
        Graphic_object target = null;
        Action endAction = null;
        Action startAction = null;

        public void settarget(Graphic_object obj)
        {
            target = obj;
        }
        public void setstart(Action ac)
        {
            startAction = ac;
        }
        public void setend(Action ac)
        {
            endAction = ac;
        }
        public void setmove(double v_x, double v_y)
        {
            this.v_x = v_x;
            this.v_y = v_y;
        }

        public void setnext(Motion m)
        {
            nextMotion = m;
        }
        public void setmax(int max)
        {
            maxcount = max;
        }
        public void start()
        {
            if (startAction != null) startAction();
            MainProgram.callback_work += cal;

        }
        void end()
        {
            MainProgram.callback_work -= this.cal;
            count = 0;
            if (nextMotion != null)
            {
                nextMotion.start();
            }
            if (endAction != null)
            {
                endAction();
            }
        }

        void cal()
        {
            if (++count > maxcount)
            {
                end();
            }
            else
            {
                target.addx(v_x);
            }
        }
    }
}
