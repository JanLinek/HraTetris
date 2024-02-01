using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HraTetris
{
    internal class Kosticka
    {
        public int X;//Souřadnice X a Y jsou používány jen jako relativní,
        public int Y;//viz VykresliKosticku
        public Kosticka(int x, int y)
        {
            X = x;
            Y = y;
        }
        //Vykreslení kosticky, parametry metody je poloha bodu, od něhož
        //se vykresluje, a barva (vyjádřená int stupnicí)
        public void VykresliKosticku(int vstupX, int vstupY, int barva)
        {
            int VykreslitX = (vstupX + X) * 2;//Při vykreslení je výsledná X souřadnice dvojnásobná
            int VykreslitY = vstupY + Y + 1;
            if (VykreslitY > 0)
            {
                if (barva> 6) VykreslitX--;
                Console.SetCursorPosition(VykreslitX, VykreslitY);
                switch (barva)
                {
                    case 0: Console.BackgroundColor = ConsoleColor.Black; break;
                    case 1: Console.BackgroundColor = ConsoleColor.Blue; break;
                    case 2: Console.BackgroundColor = ConsoleColor.Green; break;
                    case 3: Console.BackgroundColor = ConsoleColor.DarkYellow; break;
                    case 4: Console.BackgroundColor = ConsoleColor.DarkRed; break;
                    case 5: Console.BackgroundColor = ConsoleColor.Magenta; break;
                    case 6: Console.BackgroundColor = ConsoleColor.White; break;
                    case 10: Console.BackgroundColor = ConsoleColor.DarkBlue; break;
                    case 11: Console.BackgroundColor = ConsoleColor.DarkGray; break;
                    case 12: Console.BackgroundColor = ConsoleColor.Black; break;
                }
                Console.WriteLine("  ");
                Console.BackgroundColor = ConsoleColor.Black;
            }
        }

        //Rotace kosticky vůči virtuálnímu středu o 90° (prohození souřadnic X a Y a jejich znamének)
        public void RotujKosticku()
        {
            int x = X;
            int y = Y;
            if (x * y == 0)
            {
                if (y == 0) x = -x;
                X = y; Y = x;
            }
            else
            {
                Y = Math.Abs(x); X = Math.Abs(y);
                if (x > 0 && y > 0) Y = -Y;
                if (x > 0 && y < 0)
                {
                    Y = -Y; X = -X;
                }
                if (x < 0 && y < 0) X = -X;
            }
        }
    }
}

