using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HasamiShogi
{
    class Játékos // A játékban részt vevő játékosokat reprezentáló, azok pontszámát és figuráit tartalmazó osztály
    {
        int csapat;
        int pont;
        public int Pont
        {
            get { return pont; }
            set { pont = value; }
        }
        Figura[] figurák;
        public Figura[] Figurák
        {
            get { return figurák; }
            set { figurák = value; }
        }

        public Játékos(int csapat)
        {
            this.csapat = csapat;
            pont = 0;
            figurák = new Figura[9];

            if (csapat == 1) // Játékos számára 1-es csapathoz tartozó Figurák létrehozása
            {
                int j = 1;
                for (int i = 0; i < 9; i++)
                {
                    figurák[i] = new Figura(1, 9, j);
                    j++;
                }
            }
            else if (csapat == 2) // Játékos számára 2-es csapathoz tartozó Figurák létrehozása
            {
                int j = 1;
                for (int i = 0; i < 9; i++)
                {
                    figurák[i] = new Figura(2, 1, j);
                    j++;
                }
            }
        }
    }
}