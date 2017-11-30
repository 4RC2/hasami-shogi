using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace HasamiShogi
{
    class Program
    {
        public static Tábla tábla = new Tábla();
        static char[] betűk = tábla.Betűk;
        static char[] számok = tábla.Számok;

        public static Játékos[] játékosok = new Játékos[2];
        static Játékos aktuálisJátékos;
        static bool másikJátékos = Convert.ToBoolean(0);

        static bool lépett = false;
        static bool feladta = false;
        static bool vége = false;
        public static bool betöltött = false;

        static bool hiba_bevitel = false;
        static bool hiba_mentés = false;
        static bool hiba_betöltés = false;

        static void Main(string[] args)
        {
            Console.Title = "Hasami shogi";
            tábla.Rajzol();
            Segítség();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Nyomj meg egy billentyűt a kezdéshez! ");
            Console.ResetColor();
            Console.ReadKey();

            játékosok[0] = new Játékos(1);
            játékosok[1] = new Játékos(2);

            while (!vége)
            {
                aktuálisJátékos = játékosok[Convert.ToInt32(másikJátékos)];
                ParancsBevitel();
                if (!hiba_bevitel && (lépett || betöltött))
                    másikJátékos = !másikJátékos;
                if (játékosok[0].Pont >= 8 || játékosok[1].Pont >= 8)
                    vége = true;
            }

            if (!feladta)
            {
                if (játékosok[0].Pont > játékosok[1].Pont)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("1. JÁTÉKOS megnyerte a játékot.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("2. JÁTÉKOS megnyerte a játékot.");
                    Console.ResetColor();
                }
            }

            Console.ReadKey();
        }

        static void ParancsBevitel()
        {
            do
            {
                hiba_bevitel = false;
                hiba_mentés = false;
                hiba_betöltés = false;
                lépett = false;
                betöltött = false;

                Console.Write("(");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(Convert.ToInt32(másikJátékos) + 1);
                Console.ResetColor();
                Console.Write(")");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(">>>> ");
                Console.ResetColor();
                string bemenet = Console.ReadLine().Trim();
                Console.Clear();
                tábla.Rajzol();

                if (bemenet.ToLower().StartsWith("lép") || bemenet.ToLower().StartsWith("lep"))
                {
                    if (bemenet.Length == 9)
                    {
                        string részlet = bemenet.Substring(4, 5);

                        char honnanBetű = részlet[0];
                        int iHonnanBetű = -1;
                        char honnanSzám = részlet[1];
                        int iHonnanSzám = -1;

                        int i = 0;
                        while (i < 9 && (iHonnanBetű == -1 || iHonnanSzám == -1))
                        {
                            if (honnanBetű.ToString().ToUpper() == betűk[i].ToString())
                                iHonnanBetű = i;
                            if (honnanSzám == számok[i])
                                iHonnanSzám = i;
                            i++;
                        }

                        int[] honnan = new int[2];
                        honnan[0] = iHonnanBetű;
                        honnan[1] = iHonnanSzám;

                        char hovaBetű = részlet[3];
                        int iHovaBetű = -1;
                        char hovaSzám = részlet[4];
                        int iHovaSzám = -1;

                        i = 0;
                        while (i < 9 && (iHovaBetű == -1 || iHovaSzám == -1))
                        {
                            if (hovaBetű.ToString().ToUpper() == betűk[i].ToString())
                                iHovaBetű = i;
                            if (hovaSzám == számok[i])
                                iHovaSzám = i;
                            i++;
                        }

                        bool megfelelőAdatok = iHonnanBetű != -1 && iHonnanSzám != -1 && iHovaBetű != -1 && iHovaSzám != -1;
                        bool honnanHovaNemEgyezik = String.Concat(részlet[0], részlet[1]) != String.Concat(részlet[3], részlet[4]);
                        bool hovaÜresMező = false;
                        if (megfelelőAdatok)
                            hovaÜresMező = tábla.Mátrix[iHovaBetű, iHovaSzám] == '-';
                        bool nemLépneÁtlósan = iHonnanBetű == iHovaBetű || iHonnanSzám == iHovaSzám;

                        if (megfelelőAdatok && honnanHovaNemEgyezik
                            && hovaÜresMező && nemLépneÁtlósan)
                        {
                            int[] hova = new int[2];
                            hova[0] = iHovaBetű;
                            hova[1] = iHovaSzám;

                            try
                            {
                                Figura.Get(aktuálisJátékos, honnan).Lép(honnan, hova);
                                lépett = true;
                            }
                            catch (NullReferenceException)
                            {
                                hiba_bevitel = true;
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("A MEGADOTT HELYEN NEM TARTÓZKODIK FIGURA, VAGY NEM A TIÉD!");
                                Console.ResetColor();
                            }
                        }
                        else
                        {
                            hiba_bevitel = true;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("HIBÁS PARANCS!");
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        hiba_bevitel = true;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("HIBÁS PARANCS!");
                        Console.ResetColor();
                    }
                }
                else if (bemenet.ToLower() == "felad")
                    Feladás();
                else if (bemenet.ToLower() == "fájlok" || bemenet.ToLower() == "fajlok")
                    Fájlok();
                else if (bemenet.ToLower().StartsWith("ment "))
                    Mentés(bemenet.Remove(0, 5));
                else if (bemenet.ToLower().StartsWith("betölt ") || bemenet.ToLower().StartsWith("betolt"))
                    Betöltés(bemenet.Remove(0, 7));
                else if (bemenet.ToLower() == "segíts" || bemenet.ToLower() == "segits")
                    Segítség();
                else if (bemenet.ToLower() == "kilép" || bemenet.ToLower() == "kilep")
                    Environment.Exit(0);
                else
                {
                    hiba_bevitel = true;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ILYEN PARANCS NEM LÉTEZIK!");
                    Console.ResetColor();
                    Segítség();
                }

                if (hiba_mentés)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("MÁR LÉTEZIK ILYEN NEVŰ FÁJL!");
                    Console.ResetColor();
                }
                else if (hiba_betöltés)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("NEM LÉTEZIK ILYEN NEVŰ FÁJL, VAGY A BETÖLTENDŐ FÁJL SÉRÜLT!!");
                    Console.ResetColor();
                }

            } while (hiba_bevitel || hiba_mentés || hiba_betöltés);
        }

        static void Fájlok()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (Directory.Exists("mentések"))
            {
                if (Directory.GetFiles("mentések", "*.txt", SearchOption.TopDirectoryOnly).Length != 0)
                {
                    Console.WriteLine("[FÁJLOK]:");
                    foreach (string fájlnév in Directory.GetFiles("mentések", "*.txt", SearchOption.TopDirectoryOnly))
                        Console.WriteLine(fájlnév.Substring(9).Replace(".txt", ""));
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("A \"mentések\" MAPPÁBAN NEM TALÁLHATÓ EGY BETÖLTHETŐ FÁJL SEM!");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("A \"mentések\" MAPPA NEM LÉTEZIK!");
                Console.ResetColor();
            }
        }

        static void Mentés(string fájlnév)
        {
            if (!Directory.Exists("mentések"))
                Directory.CreateDirectory("mentések");

            if (!File.Exists("mentések\\" + fájlnév + ".txt"))
            {
                using (StreamWriter fájl = new StreamWriter("mentések\\" + fájlnév + ".txt"))
                {
                    for (int i = 0; i < 9; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            if (tábla.Mátrix[i, j] == '-')
                                fájl.Write('0');
                            else
                                fájl.Write(tábla.Mátrix[i, j]);
                        }
                        fájl.WriteLine();
                    }
                    fájl.WriteLine(játékosok[0].Pont + ":" + játékosok[1].Pont);
                    fájl.WriteLine(Convert.ToInt32(másikJátékos) + 1);
                }
            }
            else
                hiba_mentés = true;
        }

        static void Betöltés(string fájlnév)
        {
            if (File.Exists("mentések\\" + fájlnév + ".txt"))
            {
                betöltött = true;

                játékosok[0].Figurák = new Figura[9];
                játékosok[1].Figurák = new Figura[9];

                string sor;
                int sorszám = 0;
                int j1 = 0;
                int j2 = 0;

                using (StreamReader fájl = new StreamReader("mentések\\" + fájlnév + ".txt"))
                {
                    while ((sor = fájl.ReadLine()) != null)
                    {
                        if (sorszám < 9)
                        {
                            for (int i = 0; i < sor.Length; i++)
                            {
                                char c = sor[i];

                                if (c == '0')
                                    tábla.Mátrix[sorszám, i] = '-';
                                else if (c == '1')
                                {
                                    játékosok[0].Figurák[j1] = new Figura(1, sorszám, i);
                                    j1++;
                                }
                                else if (c == '2')
                                {
                                    játékosok[1].Figurák[j2] = new Figura(2, sorszám, i);
                                    j2++;
                                }
                                else
                                    hiba_betöltés = true;
                            }
                            sorszám++;
                        }
                        else if (sorszám == 10 - 1)
                        {
                            játékosok[0].Pont = int.Parse(sor[0].ToString());
                            játékosok[1].Pont = int.Parse(sor[2].ToString());
                            sorszám++;
                        }
                        else if (sorszám == 11 - 1)
                            aktuálisJátékos = játékosok[int.Parse(sor[0].ToString()) - 1];
                    }

                    tábla.Rajzol();
                }
            }
            else
                hiba_betöltés = true;
        }

        static void Feladás()
        {
            vége = true;
            feladta = true;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine((Convert.ToInt32(!másikJátékos) + 1) + ". JÁTÉKOS megnyerte a játékot.");
            Console.ResetColor();
        }

        static void Segítség()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[PARANCSOK]: lép <X1 Y2> | felad | fájlok | ment <fájlnév> | betölt <fájlnév> | segíts | kilép");
            Console.ResetColor();
        }
    }
}