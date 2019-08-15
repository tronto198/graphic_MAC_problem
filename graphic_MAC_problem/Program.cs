using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormlib;

namespace graphic_MAC_problem
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    static class MainProgram
    {
        static helper help = new helper();
        static DoubleBuffering db = DoubleBuffering.getinstance();
        static Graphic_first_Viewer viewer;
        static Graphic_result_Viewer result = new Graphic_result_Viewer();
        static Threading_Timer_v0 timer = new Threading_Timer_v0();

        public delegate void timerwork();
        public static timerwork callback_work;
        static Form1 form;

        static void emptyfunc() { }

        public static void init(Form1 form_)
        {
            form = form_;
            timer.setInterval(10);
            callback_work += emptyfunc;
            timer.setCallback(()=>
            {
                callback_work();
            });
            timer.Start();

            viewer = new Graphic_first_Viewer();
            viewer.init();
            viewer.display();
            viewer.focusd(true);
        }

        public static void data_in(int m, int c, int b)
        {
            viewer.undisplay();
            viewer.focusd(false);
            state s = help.getans(m, c, b);

            List<state> glist = help.getglist();
            
            if(s == null)
            {
                help.clear();
                MessageBox.Show("not found");
                viewer.display();
                viewer.focusd(true);
            }
            else
            {
                List<int[]> ans = new List<int[]>();
                Stack<int> st = new Stack<int>();

                s.getindex(glist, st);

                while(st.Count != 0)
                {
                    state ss = glist[st.Pop()];
                    int[] arr = ss.getarr();
                    ans.Add(arr);
                    
                }

                result.init(ans);
                result.display();
                result.focus(true);
                result.start();
            }
        }

        public static void end()
        {
            result.undisplay();
            result.focus(false);
            result.clear();
            help.clear();

            viewer.init();
            viewer.display();
            viewer.focusd(true);
        }
        
        public static void close()
        {
            form.close();
        }
    }
}
