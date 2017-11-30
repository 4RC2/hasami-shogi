using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace HasamiShogi
{
    class Program // A program alapvető működését kezelő osztály
    {
        // Tábla és adatmezői;
        public static Tábla tábla = new Tábla();
        static char[] betűk = tábla.Betűk;
        static char[] számok = tábla.Számok;

        // Játékosok
        public static Játékos[] játékosok = new Játékos[2];
        static Játékos aktuálisJátékos;
        static bool másikJátékos = Convert.ToBoolean(0);

        // Játék állapotát jellemző logikai változók
        static bool lépett = false;
        static bool feladta = false;
        static bool vége = false;
        public static bool betöltött = false;

        // Hibalehetőségek
        static bool hiba_bevitel = false;
        static bool hiba_mentés = false;
        static bool hiba_betöltés = false;

        static void Main(string[] args)
        {
            // Tábla rajzolása, üzenetek kiírása
            Console.Title = "Hasami shogi";
            tábla.Rajzol();
            Segítség();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Nyomj meg egy billentyűt a kezdéshez! ");
            Console.ResetColor();
            Console.ReadKey();

            játékosok[0] = new Játékos(1);
            játékosok[1] = new Játékos(2);

            while (!vége) // Fő játékciklus: Addig fut, amíg valamelyik játékos meg nem nyeri, vagy fel nem adja a játékot
            {
                aktuálisJátékos = játékosok[Convert.ToInt32(másikJátékos)]; // Aktuális játékos cseréje
                ParancsBevitel();
                if (!hiba_bevitel && (lépett || betöltött)) // Játékos cseréjéhez szükséges feltételek vizsgálata
                    másikJátékos = !másikJátékos;
                if (játékosok[0].Pont >= 8 || játékosok[1].Pont >= 8) // Annak vizsgálata, hogy nyert-e valamelyik Játékos
                    vége = true;
            }

            if (!feladta) // Győzelmi üzenet kiírása
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

        static void ParancsBevitel() // Felhasználó által bevitt szöveges parancsokat kiértékelő metódus
        {
            do
            {
                // Logikai változók alaphelyzetbe állítása
                hiba_bevitel = false;
                hiba_mentés = false;
                hiba_betöltés = false;
                lépett = false;
                betöltött = false;

                // Felhasználói bevitel fogadása, Tábla rajzolása
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

                if (bemenet.ToLower().StartsWith("lép") || bemenet.ToLower().StartsWith("lep")) // "lép" parancs argumentumainak kiértékelése
                {
                    if (bemenet.Length == 9)
                    {
                        string részlet = bemenet.Substring(4, 5);

                        char honnanBetű = részlet[0];
                        int iHonnanBetű = -1;
                        char honnanSzám = részlet[1];
                        int iHonnanSzám = -1;

                        int i = 0;
                        while (i < 9 && (iHonnanBetű == -1 || iHonnanSzám == -1)) // Megfelelő indexek hozzárendelése a "honnan" argumentum betűjéhez, illetve számához 
                        {
                            if (honnanBetű.ToString().ToUpper() == betűk[i].ToString())
                                iHonnanBetű = i;
                            if (honnanSzám == számok[i])
                                iHonnanSzám = i;
                            i++;
                        }

                        // "honnan" pozíció meghatározása (forráspozíció)
                        int[] honnan = new int[2];
                        honnan[0] = iHonnanBetű;
                        honnan[1] = iHonnanSzám;

                        char hovaBetű = részlet[3];
                        int iHovaBetű = -1;
                        char hovaSzám = részlet[4];
                        int iHovaSzám = -1;

                        i = 0;
                        while (i < 9 && (iHovaBetű == -1 || iHovaSzám == -1)) // Megfelelő indexek hozzárendelése a "hova" argumentum betűjéhez, illetve számához 
                        {
                            if (hovaBetű.ToString().ToUpper() == betűk[i].ToString())
                                iHovaBetű = i;
                            if (hovaSzám == számok[i])
                                iHovaSzám = i;
                            i++;
                        }

                        // Lépés végrehajtásához szükséges feltételek
                        bool megfelelőAdatok = iHonnanBetű != -1 && iHonnanSzám != -1 && iHovaBetű != -1 && iHovaSzám != -1;
                        bool honnanHovaNemEgyezik = String.Concat(részlet[0], részlet[1]) != String.Concat(részlet[3], részlet[4]);
                        bool hovaÜresMező = false;
                        if (megfelelőAdatok)
                            hovaÜresMező = tábla.Mátrix[iHovaBetű, iHovaSzám] == '-';
                        bool csakEgyetLépne = Math.Abs(iHonnanBetű - iHovaBetű) <= 1 && Math.Abs(iHonnanSzám - iHovaSzám) <= 1;
                        bool ugranaEgyet = false;

                        for (int j = 0; j < 2; j++) // Ugrás szándékának vizsgálata
                        {
                            ugranaEgyet =
                                (Math.Abs(iHonnanBetű - iHovaBetű) == 2 || Math.Abs(iHonnanSzám - iHovaSzám) == 2)
                                &&
                                // Célpozíció körüli mezők valamelyikén tartózkodik Figura?
                                (Figura.Get(játékosok[j], new int[] { iHovaBetű, iHovaSzám - 1 }) != null
                                || Figura.Get(játékosok[j], new int[] { iHovaBetű, iHovaSzám + 1 }) != null
                                || Figura.Get(játékosok[j], new int[] { iHovaBetű - 1, iHovaSzám }) != null
                                || Figura.Get(játékosok[j], new int[] { iHovaBetű + 1, iHovaSzám }) != null)
                                &&
                                // Üres a célpozíció?
                                Figura.Get(játékosok[j], new int[] { iHovaBetű, iHovaSzám }) == null;
                        }
                        
                        bool nemLépneÁtlósan = iHonnanBetű == iHovaBetű || iHonnanSzám == iHovaSzám;

                        if (megfelelőAdatok && honnanHovaNemEgyezik
                            && nemLépneÁtlósan && ((csakEgyetLépne && hovaÜresMező) || ugranaEgyet)) // Lépés végrehajtásához szükséges feltételek vizsgálata
                        {
                            // "hova" pozíció meghatározása (célpozíció)
                            int[] hova = new int[2];
                            hova[0] = iHovaBetű;
                            hova[1] = iHovaSzám;

                            try
                            {
                                Figura.Get(aktuálisJátékos, honnan).Lép(honnan, hova); // Lépés végrehajtása: Játékos mozgatása forráspozícióból célpozícióba
                                lépett = true;
                            }
                            catch (NullReferenceException) // Annak kezelése, ha nem tartózkodik (saját) Figura a forráspozíción
                            {
                                hiba_bevitel = true;
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("A MEGADOTT HELYEN NEM TARTÓZKODIK FIGURA, VAGY NEM A TIÉD!");
                                Console.ResetColor();
                            }
                        }
                        else // Parancs szintaktikai hibáinak kezelése
                        {
                            hiba_bevitel = true;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("HIBÁS PARANCS!");
                            Console.ResetColor();
                        }
                    }
                    else // Parancs szintaktikai hibáinak kezelése
                    {
                        hiba_bevitel = true;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("HIBÁS PARANCS!");
                        Console.ResetColor();
                    }
                }
                else if (bemenet.ToLower() == "felad") // "felad" parancs kezelése
                    Feladás();
                else if (bemenet.ToLower() == "fájlok" || bemenet.ToLower() == "fajlok") // "fájlok" parancs kezelése
                    Fájlok();
                else if (bemenet.ToLower().StartsWith("ment ")) // "ment" parancs kezelése
                    Mentés(bemenet.Remove(0, 5));
                else if (bemenet.ToLower().StartsWith("betölt ") || bemenet.ToLower().StartsWith("betolt")) // "betölt" parancs kezelése
                    Betöltés(bemenet.Remove(0, 7));
                else if (bemenet.ToLower() == "segíts" || bemenet.ToLower() == "segits") // "segíts" parancs kezelése
                    Segítség();
                else if (bemenet.ToLower() == "kilép" || bemenet.ToLower() == "kilep") // "kilép" parancs kezelése
                    Environment.Exit(0);
                else // Parancs szintaktikai hibáinak kezelése
                {
                    hiba_bevitel = true;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ILYEN PARANCS NEM LÉTEZIK!");
                    Console.ResetColor();
                    Segítség();
                }

                // Fájlkezelési hibák kezelése
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

        static void Fájlok() // "fájlok" parancs által hívott metódus
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (Directory.Exists("mentések")) // "mentések" mappa létezésének vizsgálata
            {
                if (Directory.GetFiles("mentések", "*.txt", SearchOption.TopDirectoryOnly).Length != 0)
                {
                    Console.WriteLine("[FÁJLOK]:"); // "mentések" mappában található szöveges fájlok kilistázása
                    foreach (string fájlnév in Directory.GetFiles("mentések", "*.txt", SearchOption.TopDirectoryOnly))
                        Console.WriteLine(fájlnév.Substring(9).Replace(".txt", ""));
                    Console.ResetColor();
                }
                else // Fájlkezelési hiba kezelése
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("A \"mentések\" MAPPÁBAN NEM TALÁLHATÓ EGY BETÖLTHETŐ FÁJL SEM!");
                    Console.ResetColor();
                }
            }
            else // Fájlkezelési hiba kezelése
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("A \"mentések\" MAPPA NEM LÉTEZIK!");
                Console.ResetColor();
            }
        }

        static void Mentés(string fájlnév) // "ment" parancs által hívott metódus
        {
            if (!Directory.Exists("mentések"))
                Directory.CreateDirectory("mentések"); // "mentések" mappa létrehozása, amennyiben még nem létezik

            if (!File.Exists("mentések\\" + fájlnév + ".txt"))
            {
                using (StreamWriter fájl = new StreamWriter("mentések\\" + fájlnév + ".txt")) // Adatok írása a megadott nevű fájlba
                {
                    for (int i = 0; i < 9; i++) // Tábla elrendezésének írása
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
                    fájl.WriteLine(játékosok[0].Pont + ":" + játékosok[1].Pont); // Játékosok pontszámainak írása
                    fájl.WriteLine(Convert.ToInt32(másikJátékos) + 1); // Betöltés után soron következő Játékos számának írása
                }
            }
            else
                hiba_mentés = true;
        }

        static void Betöltés(string fájlnév) // "betölt" parancs által hívott metódus
        {
            if (File.Exists("mentések\\" + fájlnév + ".txt")) // Betölteni kívánt fájl létezésének vizsgálata
            {
                betöltött = true;

                játékosok[0].Figurák = new Figura[9];
                játékosok[1].Figurák = new Figura[9];

                string sor;
                int sorszám = 0;
                int j1 = 0;
                int j2 = 0;

                using (StreamReader fájl = new StreamReader("mentések\\" + fájlnév + ".txt")) // Adatok olvasása a megadott nevű fájlból
                {
                    while ((sor = fájl.ReadLine()) != null)
                    {
                        if (sorszám < 9) // Tábla elrendezésének olvasása
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
                        else if (sorszám == 10 - 1) // Játékosok pontszámainak olvasása
                        {
                            játékosok[0].Pont = int.Parse(sor[0].ToString());
                            játékosok[1].Pont = int.Parse(sor[2].ToString());
                            sorszám++;
                        }
                        else if (sorszám == 11 - 1) // Aktuális játékos választása a fájl alapján
                            aktuálisJátékos = játékosok[int.Parse(sor[0].ToString()) - 1];
                    }

                    tábla.Rajzol();
                }
            }
            else
                hiba_betöltés = true;
        }

        static void Feladás() // "felad" parancs által hívott metódus
        {
            vége = true;
            feladta = true;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine((Convert.ToInt32(!másikJátékos) + 1) + ". JÁTÉKOS megnyerte a játékot.");
            Console.ResetColor();
        }

        static void Segítség() // "segíts" parancs által hívott metódus
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[PARANCSOK]: lép <X1 Y2> | felad | fájlok | ment <fájlnév> | betölt <fájlnév> | segíts | kilép");
            Console.ResetColor();
        }
    }
}