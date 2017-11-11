using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HasamiShogi
{
    class Tábla
    {
        char[,] mátrix;
        public char[,] Mátrix
        {
            get { return mátrix; }
            set { mátrix = value; }
        }
        char[] betűk;
        public char[] Betűk
        {
            get { return betűk; }
        }
        char[] számok;
        public char[] Számok
        {
            get { return számok; }
        }

        public Tábla()
        {
            mátrix = new char[9, 9];

            for (int i = 0; i < mátrix.GetLength(0); i++)
                for (int j = 0; j < mátrix.GetLength(0); j++)
                    mátrix[i, j] = '-';

            betűk = new char[9];
            int j1 = 0;
            for (int i = 65; i <= 73; i++)
            {
                betűk[j1] = (char) i;
                j1++;
            }

            számok = new char[9];
            int j2 = 0;
            for (int i = 49; i <= 57; i++)
            {
                számok[j2] = (char) i;
                j2++;
            }
        }

        public void Rajzol()
        {
            Console.Clear();
            
            if (Program.játékosok[0] != null && Program.játékosok[1] != null)
            {
                Console.Write("(");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(Program.játékosok[0].Pont);
                Console.ResetColor();
                Console.Write(":");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(Program.játékosok[1].Pont);
                Console.ResetColor();
                Console.Write(")  ");
            }    
            else
            {
                Console.Write("(");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("0");
                Console.ResetColor();
                Console.Write(":");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("0");
                Console.ResetColor();
                Console.Write(")  ");
            }       
            Console.ResetColor();

            for (int i = 1; i <= 9; i++)
            {

                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(i);
                Console.ResetColor();
                Console.Write("]  ");
            }
            Console.WriteLine();
            Console.WriteLine();

            for (int i = 0; i < mátrix.GetLength(0); i++)
            {
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(betűk[i]);
                Console.ResetColor();
                Console.Write("] \t");
                
                for (int j = 0; j < mátrix.GetLength(0); j++)
                    Console.Write(mátrix[i, j] + "    ");
                Console.WriteLine("\n");
            }
        }
    }
}