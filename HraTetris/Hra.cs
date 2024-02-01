using HraTetris;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HraTetris
{
    internal class Hra
    {
        static int Score;
        static int Level;
        public int Items=0;//Počitadlo vygenerovaných obrazců
        public int SirkaPlochy;//bude převzato v konstruktoru
        public int VyskaPlochy;//bude převzato v konstruktoru
        public int OdstupZleva;//bude převzato v konstruktoru
        public int MantinelX1;//Pomocné proměnné,
        public int MantinelX2;//odvíjejí se od 
        public int MantinelY;//rozměrů hrací plochy

        public int[,] kostickyPlocha; //2D pole int-ů reprezentující zaplnění hrací plochy (jednotlivé
                                       //int-y jsou vlastně barvy, které měl zaplňující obrazec
        public Kosticka kosticka; //Objekt pro jakékoliv vykreslování "kostiček"
        public Logo Logo; //Samostatná třída pro deklaraci a kreslení loga TETRIS
        public Obrazec obrazec;//každý nově vygenerovaný obrazec dostane instanci
        public Random r;
        public Hra(int odstupZleva, int sirkaPLochy, int vyskaPlochy)
        {
            OdstupZleva = odstupZleva;
            SirkaPlochy = sirkaPLochy;
            VyskaPlochy = vyskaPlochy;
            kostickyPlocha = new int[SirkaPlochy, VyskaPlochy];
            kosticka = new Kosticka(0, 0);//Každá kostička v této třídě má pozici (0,0)
            Logo = new Logo();
            MantinelX1 = OdstupZleva - 1;
            MantinelX2 = MantinelX1 + SirkaPlochy + 1;
            MantinelY = VyskaPlochy;
            r = new Random();
        }
        public void HraciCyklus()
        {
            Score = 0;//Vynulování skore
            Level = 1;
            int X = SirkaPlochy / 2;    //Toto budou souřadnice nově vygenerovaného obrazce
            int Y = 1;
            bool NeniKonec = true;  //Proměnná sledující, zda nenastala prohra
            int PristiVarianta = -1;int PristiBarva = -1;
            //Pro spuštění hry se vyčká na klávesu
            VykresliRamecek(MantinelX1 - 16, VyskaPlochy / 2, 5, 4, " Klávesu pro start... ");
            Console.ReadKey(true);
            VykresliRamecek(MantinelX1 - 16, VyskaPlochy / 2, 5, 0, "                      ");

            //Začátek smyčky, ze které je únik jen prohrou
            while (NeniKonec)
            {
                int obrazcu = 0; //pomocný čítač, po 50 obrazcích se zvyšuje Level
                while (obrazcu < 50)
                {
                    obrazcu++;
                    Items++;
                    Score++;
                    if (PristiVarianta<0) PristiVarianta = r.Next(7);
                    if (PristiBarva < 0) PristiBarva = r.Next(1, 6);
                    VypisScore();
                    VykresliPristiTvar(PristiVarianta,PristiBarva);
                    int Varianta = r.Next(7);   //Varianta generovaného obrazce
                    Hra hraProObrazec = new Hra(OdstupZleva, SirkaPlochy, VyskaPlochy); //pomocná instance Hry
                    hraProObrazec.kostickyPlocha = kostickyPlocha;                      //pro obrazec
                    Obrazec obrazec = new Obrazec(Varianta, X, Y, 3, hraProObrazec);    //vygenerování obrazce
                    obrazec.Barva = r.Next(1, 6);   //barva obrazce je náhodná...ani nevím, proč to nemám v konstruktoru
                    obrazec.VykresliObrazec();
                    bool Dopadl = false;    //Proměnná sledující, zda obrazec "nedosedl"
                    bool Spustit = false;   //Proměnná sledující, zda není aktivováno rychlé spuštění obrazce
                    while (!Dopadl)         //bude se opakovat až do dopadu
                    {
                        if (!Spustit)       //Pokud není aktivní rychlé spuštění, testuji klávesnici
                        {
                            for (int j = 0; j < 31 - Level; j++)    //Počet cyklů klesá s rostoucím Levelem
                            {
                                Thread.Sleep(1);
                                if (Console.KeyAvailable)
                                {
                                    ConsoleKeyInfo klavesa = Console.ReadKey();
                                    if (klavesa.Key == ConsoleKey.Spacebar) PauzaHry();
                                    if (klavesa.Key == ConsoleKey.RightArrow) obrazec.PosunObrazec(1, 0);
                                    if (klavesa.Key == ConsoleKey.LeftArrow) obrazec.PosunObrazec(-1, 0);
                                    if (klavesa.Key == ConsoleKey.UpArrow) obrazec.RotujObrazec();
                                    if (klavesa.Key == ConsoleKey.DownArrow) Spustit = true;
                                }
                            }
                        }
                        if (Spustit) Console.Beep(200, 20);
                        Dopadl = obrazec.PosunObrazec(0, 1); //Na obrazci otestuju, zda dopadl, předám proměnné Dopadl
                    }
                    VypisScore(); 
                    if (obrazec.PoziceY < 2)    //Pokud dopadl úplně nahoře, je prohra a vyskakuji z cyklu
                    {
                        NeniKonec = false;
                        break;
                    }
                    PristiVarianta = Varianta;
                    PristiBarva = obrazec.Barva;
                }
                if (Level < 31)
                {
                    Level++; //Zvyšuju Level až do 30
                    for (int Barva = 1; Barva < 7; Barva++)//Probliknutí rámečku Level
                    {
                        VykresliRamecek(MantinelX1 - 10 - Level.ToString().Length / 2, 
                            VyskaPlochy - 10, 3, Barva, " Level: " + Level + " ");
                        Thread.Sleep(10); Console.Beep(100 * Barva, 100);
                    }
                }               
            }
        }
        public bool KonecHry()
        {
            for (int i = 0; i < 10; i++) Console.Beep(500 - i * 20, 10);
            VykresliRamecek(MantinelX1 + SirkaPlochy / 2 - 3, VyskaPlochy / 2, 5, 4, " Konec hry! ");
            VykresliRamecek(MantinelX1 + SirkaPlochy / 2 - 3, VyskaPlochy / 2 + 6, 5, 10, " Chcete hrát znovu (A/N)?  ");
            char HratZnovu = Console.ReadKey().KeyChar;
            return (HratZnovu != 'n');
        }
        public void PauzaHry()  //Metoda pro obsloužení pauzy hry
        {
            Console.Beep(200, 200);
            VykresliRamecek(MantinelX1 + SirkaPlochy / 2 - 5,
                VyskaPlochy / 2, 3, 10, "Pauza hry, klávesu...");
            Console.ReadKey(); //Console.Clear();
            VykresliMantinel();
            VykresliPlochu();
            VypisScore();
        }
        public void VykresliMantinel()  //Vykreslí mantinel hrací plochy
        {
            Logo.VykresliLogo(2, 3, 10);
            Logo.VykresliLogo(1, 2, 6);
            for (int Y = 1; Y < MantinelY; Y++)
            {
                kosticka.VykresliKosticku(MantinelX1, Y, 6);
                kosticka.VykresliKosticku(MantinelX2, Y, 6);
            }
            for (int X = MantinelX1; X <= MantinelX2; X++)
                kosticka.VykresliKosticku(X, MantinelY, 6);
        }

        public void VykresliPlochu()
        {
            for (int Y = 0; Y < kostickyPlocha.GetLength(1); Y++)//Projde všechny kostičky hrací plochy
            {
                for (int X = 0; X < kostickyPlocha.GetLength(0); X++)
                    kosticka.VykresliKosticku((X + OdstupZleva), Y, kostickyPlocha[X, Y]);//a vykreslí je
            }
        }
        public void ZkontrolujRadky()
        {
            int ZnicenoRad = 0;
            for (int Y = 0; Y < kostickyPlocha.GetLength(1); Y++)//Projde všechny řádky hrací plochy
            {
                bool PlnaRada = false;
                for (int X = 0; X < kostickyPlocha.GetLength(0); X++)
                {
                    PlnaRada = false;
                    if (kostickyPlocha[X, Y] == 0) break;
                    else PlnaRada = true;//V každém řádku projde kostičky, jsou-li všechny "plné",
                }
                if (PlnaRada)
                {
                    ZrusRadu(Y); ZnicenoRad++;//řada bude zrušena. A sleduje se počet zrušených řad.
                }
            }
            switch (ZnicenoRad)
            {
                case 1:Score = Score + Level * 40 - 1; break;//Bonusové zvýšení skore dle počtu řad
                case 2: Score = Score + Level * 100 - 1; break;
                case 3:Score = Score + Level * 300 - 1; break;
                case 4:Score = Score + Level * 1200 - 1; break;
            }
        }
        public void ZrusRadu(int RadaY)
        {
            for (int Barva = 1; Barva < 7; Barva++)//Probliknutí rušené řady barvami 1-7
            {
                for (int X = 0; X < kostickyPlocha.GetLength(0); X++)
                {
                    kosticka.VykresliKosticku(X + OdstupZleva, RadaY, Barva);

                }
                Thread.Sleep(10); Console.Beep(100 * Barva, 100);
            }
            for (int Y = RadaY; Y >= 0; Y--)//Pole kostiček nutno až do dané řady posunout
            {
                for (int X = 0; X < kostickyPlocha.GetLength(0); X++)
                {
                    if (Y == 0) kostickyPlocha[X, Y] = 0;//vložení první řady nulových kostiček
                    else kostickyPlocha[X, Y] = kostickyPlocha[X, Y - 1];
                }
            }
            VykresliPlochu();
        }
        public void VypisScore() //Při vykreslování se pro pozici rámečku zohledňuje rostoucí délka textu...
        {
            VykresliRamecek(MantinelX1 - 10 - Level.ToString().Length / 2, VyskaPlochy - 10, 3, 3, " Level: " + Level + " ");
            VykresliRamecek(MantinelX1 - 10 - Items.ToString().Length / 2, VyskaPlochy - 6, 3, 2, " Items: " + Items + " ");
            VykresliRamecek(MantinelX1 - 10 - Score.ToString().Length / 2, VyskaPlochy - 2, 3, 5, " Score: " + Score + " ");
        }

        //Univerzální metoda pro výpis rámečku s textem, parametry: pozice PH rohu, výška, barva, text
        //Šířka rámečku se vypočítá dle délky textu:-)
        public void VykresliRamecek(int PHrohX, int PHrohY, int vyska, int barva, string text)
        {
            for (int y = 0; y < vyska; y++)
            {
                for (int x = 0; x < text.Length / 2 + 2 + text.Length % 2; x++)
                {
                    int barvaProVypis = 0;
                    if (y == 0 || y == vyska - 1 || x == 0 || x == text.Length / 2 + 1 + text.Length % 2) barvaProVypis = barva;
                    kosticka.VykresliKosticku(PHrohX + x, PHrohY + y, barvaProVypis);
                }
            }
            Console.SetCursorPosition(PHrohX * 2 + 2, PHrohY + vyska / 2 + 1);  //Nakonec vepíšu text
            Console.WriteLine(text);
        }
        public void VykresliPristiTvar(int PristiVarianta,int PristiBarva)
        {
            VykresliRamecek(MantinelX1 - 20, VyskaPlochy - 10, 3, 3, " Next shape: " + Level + " ");
        }
        public void VstupniObrazovka()
        {
            Console.Clear();
            VykresliRamecek(4, 1, 3, 3, "VÍTÁME VÁS VE HŘE TETRIS !!!");
            Console.WriteLine("\n\n\t------------------");
            Console.WriteLine("\t Instrukce ke hře:\n\t------------------\n");
            Console.WriteLine("\t Ovládání:");
            Console.WriteLine("\t Posun obrazce doleva/doprava.............LeftArrow/RightArrow");
            Console.WriteLine("\t Otočení obrazce..........................LeftArrow/RightArrow");
            Console.WriteLine("\t Spuštění obrazce.........................DownArrow");
            Console.WriteLine("\t Pauza....................................SpaceBar\n\n");
            Console.WriteLine("\t Systém bodování:\n\t Každý vygenerovaný obrazec je za 1 bod. Mnohem více jsou ohodnocena");
            Console.WriteLine("\t víceřádková doplnění, neboť je těžší jich docílit. ");
            Console.WriteLine("\t Počet bodů odpovídající počtu zničených řádek (1-4) a úrovni hry:\n");
            Console.WriteLine("\t Počet odbouraných řad\t\tBodů");
            Console.WriteLine("\t Jedna\t\t\t\t(Level * 40):");
            Console.WriteLine("\t Dvě\t\t\t\t(Level * 100):");
            Console.WriteLine("\t Tři\t\t\t\t(Level * 300):");
            Console.WriteLine("\t Čtyři (tetris)\t\t\t(Level * 1200):");
            VykresliRamecek(33, 23, 3, 4, "KLÁVESU...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}



