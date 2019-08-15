using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphic_MAC_problem
{
    
    class state : ICloneable
    {
        //선, 식, 선, 식, 배
        int[] arr = new int[5] { 0, 0, 0, 0, 0 };
        int thisindex = -1;
        int parentindex = -1;

        public object Clone()
        {
            state newstate = new state();
            arr.CopyTo(newstate.arr, 0);
            return newstate;
        }

        public void input(int missionary, int cannibals)
        {
            arr[0] = missionary;
            arr[1] = cannibals;
        }
        public void writeans(int missionary, int cannibals)
        {
            arr[2] = missionary;
            arr[3] = cannibals;
            arr[4] = 1;
        }

        public bool req_move(int mis, int can)
        {
            if (mis + can == 0) return false;

            int index_mis = 0;
            int index_can = 1;
            if (arr[4] == 1)
            {
                index_mis = 2;
                index_can = 3;
            }

            int tmp_mis = arr[index_mis] - mis;
            int tmp_can = arr[index_can] - can;

            if (tmp_mis < 0 || tmp_can < 0) return false;

            //move
            arr[index_mis] = tmp_mis;
            arr[index_can] = tmp_can;
            if (arr[4] == 1)
            {
                index_mis = 0;
                index_can = 1;
                arr[4] = 0;
            }
            else
            {
                index_mis += 2;
                index_can += 2;
                arr[4] = 1;
            }

            arr[index_mis] += mis;
            arr[index_can] += can;

            return true;
        }

        public bool check_live()
        {
            if ((arr[0] < arr[1] && arr[0] != 0) || (arr[2] < arr[3] && arr[2] != 0)) return false;
            else return true;
        }

        //같으면 true
        public bool compare(state s)
        {
            for (int i = 0; i < 5; i++)
            {
                if (s.arr[i] != arr[i]) return false;
            }
            return true;
        }

        public void pushindex(int parent, int thisindex)
        {
            parentindex = parent;
            this.thisindex = thisindex;
        }

        public void getindex(List<state> glist, Stack<int> st)
        {
            st.Push(thisindex);
            if (parentindex != -1)
            {
                glist[parentindex].getindex(glist, st);
            }
        }

        public int[] getarr() { return arr; }
        public int getindex() { return thisindex; }
    }

    class helper
    {
        List<state> glist = new List<state>();
        Queue<state> que = new Queue<state>();
        int boatmax;

        void pushque(state s)
        {
            que.Enqueue(s);
            glist.Add(s);
        }

        //있으면 true
        bool check_exist(state s)
        {
            for (int i = 0; i < glist.Count; i++)
            {
                if (glist[i].compare(s)) return true;
            }
            return false;
        }

        void enqueue(state s, int parentindex)
        {
            if (!check_exist(s))
            {
                s.pushindex(parentindex, glist.Count);
                pushque(s);
            }
        }

        void sons(state current)
        {
            state clone = current.Clone() as state;
            

            for (int mis = 0; mis <= boatmax; mis++)
            {
                for (int can = 0; can <= boatmax - mis; can++)
                {
                    if (clone.req_move(mis, can))
                    {
                        if (clone.check_live())
                        {
                            enqueue(clone, current.getindex());
                        
                        }
                        clone = current.Clone() as state;
                    }
                }
            }
        }


        public state getans(int missionary, int cannibals, int boatmax)
        {
            this.boatmax = boatmax;
            state s = new state();
            s.input(missionary, cannibals);
            s.pushindex(-1, 0);
            pushque(s);
            state ans = new state();
            ans.writeans(missionary, cannibals);

            while (que.Count != 0)
            {
                state current = que.Dequeue();
                if (current.compare(ans))
                {
                    return current;
                }

                sons(current);
            }

            return null;
        }

        public List<state> getglist() { return glist; }

        public void clear()
        {
            glist.Clear();
            que.Clear();
        }
    }
}
