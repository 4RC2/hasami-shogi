using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HasamiShogi
{
    class Figura // A játéktáblán mozgó figurákat reprezentáló, azok mozgását és támadását kezelő osztály
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
            
            // Figura pozíciójának beállítása a konstruktora alapján
            if (!Program.betöltött)
            {
                pozíció[0] = x - 1;
                pozíció[1] = y - 1;
                Thread.Sleep(250); // Program megnyitásakor lejátszódó "animáció" időzítése 
            }
            else
            {
                pozíció[0] = x;
                pozíció[1] = y;
            } 

            Lép(pozíció);
        }
        
        public static Figura Get(Játékos játékos, int[] pozíció) // Adott Játékoshoz és adott helyen tartózkodó Figurát visszaadó metódus
        {
            foreach (Figura figura in játékos.Figurák)
                if (figura != null && figura.Pozíció[0] == pozíció[0] && figura.Pozíció[1] == pozíció[1])
                    return figura;
            return null;
        }

        public void Lép(int[] hova) // Figura mozgatását kezelő metódus
        {
            pozíció = hova;
            Program.tábla.Mátrix[hova[0], hova[1]] = char.Parse(csapat.ToString());
            Program.tábla.Rajzol();
        }

        public void Lép(int[] honnan, int[] hova) // Figura mozgatását kezelő metódus, két paraméterrel
        {
            pozíció = hova;
            Program.tábla.Mátrix[honnan[0], honnan[1]] = '-';
            Program.tábla.Mátrix[hova[0], hova[1]] = char.Parse(csapat.ToString());
            Program.tábla.Rajzol();
            Leütés();
        }

        void Leütés() // Azt vizsgáló metódus, hogy az lépést végzett Figura okozott-e leütési helyzetet
        {
            bool támadóCsapat = false;

            switch (csapat) // Támadócsapat cseréje, a kódismétlés elkerülése végett
            {
                case 1:
                    támadóCsapat = false;
                    break;
                case 2:
                    támadóCsapat = true;
                    break;
            }

            if (Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { pozíció[0], pozíció[1] - 1 }) != null // Figurától 1-gyel balra van ellenséges?
                && Get(Program.játékosok[Convert.ToInt32(támadóCsapat)], new int[] { pozíció[0], pozíció[1] - 2 }) != null) // Figurától 2-vel balra van barátságos?
            {
                Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { pozíció[0], pozíció[1] - 1 }).Kiesik(); // Ellenséges Figura kiesik
            }

            else if (Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { pozíció[0], pozíció[1] + 1 }) != null // Figurától 1-gyel jobbra van ellenséges?
            && Get(Program.játékosok[Convert.ToInt32(támadóCsapat)], new int[] { pozíció[0], pozíció[1] + 2 }) != null) // Figurától 2-vel jobbra van barátságos?
            {
                Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { pozíció[0], pozíció[1] + 1 }).Kiesik(); // Ellenséges Figura kiesik
            }
            else if (Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { pozíció[0] - 1, pozíció[1] }) != null // Figura felett 1-gyel van ellenséges?
            && Get(Program.játékosok[Convert.ToInt32(támadóCsapat)], new int[] { pozíció[0] - 2, pozíció[1] }) != null) // Figura felett 2-vel van barátságos?
            {
                Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { pozíció[0] - 1, pozíció[1] }).Kiesik(); // Ellenséges Figura kiesik
            }
            else if (Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { pozíció[0] + 1, pozíció[1] }) != null // Figura alatt 1-gyel van ellenséges?
            && Get(Program.játékosok[Convert.ToInt32(támadóCsapat)], new int[] { pozíció[0] + 2, pozíció[1] }) != null) // Figura alatt 2-vel van barátságos?
            {
                Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { pozíció[0] + 1, pozíció[1] }).Kiesik(); // Ellenséges Figura kiesik
            }

            // Sarkak ellenőrzése (A1, A9, I1, I9)
            else if (Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { 0, 0 }) != null // A1-ben ellenséges Figura?
            && Get(Program.játékosok[Convert.ToInt32(támadóCsapat)], new int[] { 0, 1 }) != null // A2-ben barátságos Figura?
            && Get(Program.játékosok[Convert.ToInt32(támadóCsapat)], new int[] { 1, 0 }) != null) // B1-ben barátságos Figura?
            {
                Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { 0, 0 }).Kiesik(); // A1-ben lévő ellenséges Figura kiesik
            }
            else if (Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { 0, 8 }) != null // A9-ben ellenséges Figura?
            && Get(Program.játékosok[Convert.ToInt32(támadóCsapat)], new int[] { 0, 7 }) != null // A8-ban barátságos Figura?
            && Get(Program.játékosok[Convert.ToInt32(támadóCsapat)], new int[] { 1, 8 }) != null) // B9-ben barátságos Figura?
            {
                Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { 0, 8 }).Kiesik(); // A9-ben lévő ellenséges Figura kiesik
            }
            else if (Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { 8, 0 }) != null // I1-ben ellenséges Figura?
            && Get(Program.játékosok[Convert.ToInt32(támadóCsapat)], new int[] { 8, 1 }) != null // I2-ben barátságos Figura?
            && Get(Program.játékosok[Convert.ToInt32(támadóCsapat)], new int[] { 7, 0 }) != null) // H1-ben barátságos Figura?
            {
                Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { 8, 0 }).Kiesik(); // I1-ben lévő ellenséges Figura kiesik
            }
            else if (Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { 8, 8 }) != null // I9-ben ellenséges Figura?
            && Get(Program.játékosok[Convert.ToInt32(támadóCsapat)], new int[] { 8, 7 }) != null // I8-ban barátságos Figura?
            && Get(Program.játékosok[Convert.ToInt32(támadóCsapat)], new int[] { 7, 8 }) != null) // H9-ben barátságos Figura?
            {
                Get(Program.játékosok[Convert.ToInt32(!támadóCsapat)], new int[] { 8, 8 }).Kiesik(); // I9-ben lévő ellenséges Figura kiesik
            }
        }

        void Kiesik() // Figura kiesését kezelő metódus
        {
            // Másik Játékos pontot kap
            if (csapat == 1)
                Program.játékosok[1].Pont += 1;
            else if (csapat == 2)
                Program.játékosok[0].Pont += 1;

            Program.tábla.Mátrix[pozíció[0], pozíció[1]] = '-'; // Tábla feltöltése üres mezőt jelző karakterrel, a leütés helyén
            Thread.Sleep(250);
            Program.tábla.Rajzol();

            pozíció[0] = -1;
            pozíció[1] = -1;
        }
    }
}