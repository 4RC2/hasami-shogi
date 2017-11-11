using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HasamiShogi
{
    class Figura
    {
        int csapat;
        int[] pozíció;
        public int[] Pozíció
        {
            get { return pozíció; }
        }

        public Figura(int csapat, int x, int y)
        {
            this.csapat = csapat;
            pozíció = new int[2];
            
            if (!Program.betöltött)
            {
                pozíció[0] = x - 1;
                pozíció[1] = y - 1;
                Thread.Sleep(250);
            }
            else
            {
                pozíció[0] = x;
                pozíció[1] = y;
            } 

            Lép(pozíció);
        }
        
        public static Figura Get(Játékos játékos, int[] pozíció)
        {
            foreach (Figura figura in játékos.Figurák)
                if (figura != null && figura.Pozíció[0] == pozíció[0] && figura.Pozíció[1] == pozíció[1])
                    return figura;
            return null;
        }

        public void Lép(int[] hova)
        {
            pozíció = hova;
            Program.tábla.Mátrix[hova[0], hova[1]] = char.Parse(csapat.ToString());
            Program.tábla.Rajzol();
        }

        public void Lép(int[] honnan, int[] hova)
        {
            pozíció = hova;
            Program.tábla.Mátrix[honnan[0], honnan[1]] = '-';
            Program.tábla.Mátrix[hova[0], hova[1]] = char.Parse(csapat.ToString());
            Program.tábla.Rajzol();
            Leütés();
        }

        void Leütés()
        {
            bool támadóCsapat = false;

            switch (csapat)
            {
                case 1:
                    támadóCsapat = false;
                    break;
                case 2:
                    támadóCsapat = true;
                    break;
            }

            if (Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { pozíció[0], pozíció[1] - 1 }) != null
                && Get(Program.játékosok[Convert.ToInt32(támadóCsapat)], new int[] { pozíció[0], pozíció[1] - 2 }) != null)
            {
                Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { pozíció[0], pozíció[1] - 1 }).Kiesik();
            }

            else if (Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { pozíció[0], pozíció[1] + 1 }) != null
            && Get(Program.játékosok[Convert.ToInt32(támadóCsapat)], new int[] { pozíció[0], pozíció[1] + 2 }) != null)
            {
                Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { pozíció[0], pozíció[1] + 1 }).Kiesik();
            }
            else if (Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { pozíció[0] - 1, pozíció[1] }) != null
            && Get(Program.játékosok[Convert.ToInt32(támadóCsapat)], new int[] { pozíció[0] - 2, pozíció[1] }) != null)
            {
                Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { pozíció[0] - 1, pozíció[1] }).Kiesik();
            }
            else if (Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { pozíció[0] + 1, pozíció[1] }) != null
            && Get(Program.játékosok[Convert.ToInt32(támadóCsapat)], new int[] { pozíció[0] + 2, pozíció[1] }) != null)
            {
                Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { pozíció[0] + 1, pozíció[1] }).Kiesik();
            }

            else if (Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { 0, 0 }) != null
            && Get(Program.játékosok[Convert.ToInt32(támadóCsapat)], new int[] { 0, 1 }) != null
            && Get(Program.játékosok[Convert.ToInt32(támadóCsapat)], new int[] { 1, 0 }) != null)
            {
                Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { 0, 0 }).Kiesik();
            }
            else if (Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { 0, 8 }) != null
            && Get(Program.játékosok[Convert.ToInt32(támadóCsapat)], new int[] { 0, 7 }) != null
            && Get(Program.játékosok[Convert.ToInt32(támadóCsapat)], new int[] { 1, 8 }) != null)
            {
                Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { 0, 8 }).Kiesik();
            }
            else if (Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { 8, 0 }) != null
            && Get(Program.játékosok[Convert.ToInt32(támadóCsapat)], new int[] { 8, 1 }) != null
            && Get(Program.játékosok[Convert.ToInt32(támadóCsapat)], new int[] { 7, 0 }) != null)
            {
                Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { 8, 0 }).Kiesik();
            }
            else if (Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { 8, 8 }) != null
            && Get(Program.játékosok[Convert.ToInt32(támadóCsapat)], new int[] { 8, 7 }) != null
            && Get(Program.játékosok[Convert.ToInt32(támadóCsapat)], new int[] { 7, 8 }) != null)
            {
                Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { 8, 8 }).Kiesik();
            }
        }

        void Kiesik()
        {
            if (csapat == 1)
                Program.játékosok[1].Pont += 1;
            else if (csapat == 2)
                Program.játékosok[0].Pont += 1;

            Program.tábla.Mátrix[pozíció[0], pozíció[1]] = '-';
            Thread.Sleep(250);
            Program.tábla.Rajzol();

            pozíció[0] = -1;
            pozíció[1] = -1;
        }
    }
}