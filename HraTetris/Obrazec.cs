using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HraTetris
{
    internal class Obrazec
    {
        public int Varianta;
        public int PoziceX;
        public int PoziceY; //Každý nový obrazec bude mít aktuální pozici ("středu") a barvu
        public int Barva;   //Barva je reprezentována int škálou 1-6
        Kosticka[] kosticky = new Kosticka[4];  //pole kostiček obrazce
        Kosticka[] kostickyNove = new Kosticka[4];  //pomocné pole kostiček pro test, zda je možný posun/rotace
        Hra Hra;                                    //a také pro efektnější překreslování
        string[] SouradniceZdroj =
        {
            ("0,0 1,0 0,1 1,1"), 
            ("0,0 0,1 1,1 2,1"), 
            ("0,1 1,1 2,1 2,0"), 
            ("0,3 0,1 0,2 0,0"),        //pomocné pole stringů pro sestavu obrazce
            ("0,0 1,0 1,1 2,1"),        
            ("1,0 2,0 0,1 1,1"),        //každý rádek je 1 varianta obrazce (7 variant)
            ("0,0 1,0 2,0 1,1"),        //každá dvojice x,y představuje 1 ze 4 kostiček obrazce
        };

        public Obrazec(int varianta, int poziceX, int poziceY, int barva, Hra hra)
        {
            Hra = hra;
            PoziceX = poziceX;
            PoziceY = poziceY;
            Barva = barva;
            Varianta = varianta;
            string[] SouradniceKosticky = SouradniceZdroj[Varianta].Split(" ");
            for (int i = 0; i < kosticky.Length; i++)
            {
                kosticky[i] = new Kosticka(0, 0);
                kostickyNove[i] = new Kosticka(0, 0);   
                int X = Convert.ToInt32((SouradniceKosticky[i].Split(","))[0]) - 1; //Vyextrahování souřadnic
                int Y = Convert.ToInt32((SouradniceKosticky[i].Split(","))[1]) - 1; //z pole stringů, mínus jedna
                kosticky[i].X = X;                                                  //je tam kvůli posunu původních
                kosticky[i].Y = Y;                                                  //souřadnic na lepší "střed"
                kostickyNove[i].X = X;  //na začátku je pomocné pole stejné
                kostickyNove[i].Y = Y;
            }
        }
        public void VykresliObrazec()
        {
            foreach (Kosticka kosticka in kosticky)
            {
                kosticka.VykresliKosticku(PoziceX + Hra.MantinelX1, PoziceY, Barva);
            }
        }
        public bool PosunObrazec(int posunX, int posunY)
        {
            for (int i = 0; i < kosticky.Length; i++)
            {
                kostickyNove[i].X = (kosticky[i].X) + posunX;   //nové kostičky budou někam posunuty
                kostickyNove[i].Y = kosticky[i].Y + posunY;
            }

            if (!JeKolize())  //Otestuj, zda nové kostičky nebudou v kolici (mimo plochu nebo už je tam plno)
            {
                PrekresliObrazec(); 
                PoziceX += posunX;  //Pokud nebudou, jen přesuň pozici obrazce
                PoziceY += posunY;

            }
            else
            {
                if (posunY == 1)    //Pokud budou a jde o posun dolů, obrazec NARAZIL
                {
                    foreach (Kosticka kosticka in kosticky) //a jeho kostičky budou převzaty do hrací plochy
                        Hra.kostickyPlocha[(PoziceX + kosticka.X) - 1, PoziceY + kosticka.Y] = Barva;
                    Hra.ZkontrolujRadky();  //a také nutno zkontrolovat, zda někde nedošlo k naplnění řady
                    return true;            //a ohlaš náraz
                }
            }
            VratNoveKosticky(); //Pokud budou v kolizi a nebyl posun dolů, vrať nové kostičky na původní stav
            return false;       //Posun nebude, ale nebyl to náraz
        }
        public void PrekresliObrazec()
        {
            foreach (Kosticka kosticka in kosticky)  //projdi aktuální sadu kostiček
            {
                int x = kosticka.X;
                int y = kosticka.Y;
                bool KryjouSe = false;
                foreach (Kosticka kostickaNova in kostickyNove) //a porovnej se sadou nových kostiček
                {
                    if (kostickaNova.X == x && kostickaNova.Y == y)
                    {
                        KryjouSe = true;  //zjisti, které se kryjou
                    }                   //a u těch, co se nekryjou, je odmaž (vykresli černě)
                }
                if (!KryjouSe) kosticka.VykresliKosticku(PoziceX + Hra.MantinelX1, PoziceY, 0);
            }
            foreach (Kosticka kosticka in kostickyNove) //Nové kostičky vykresli barvou obrazce
                kosticka.VykresliKosticku(PoziceX + Hra.MantinelX1, PoziceY, Barva);

        }
        public void RotujObrazec()
        {
            foreach (Kosticka kosticka in kostickyNove) kosticka.RotujKosticku();   //Zrotuj kostičky
            if (!JeKolize())
            {
                PrekresliObrazec();
                foreach (Kosticka kosticka in kosticky) kosticka.RotujKosticku();
            }
            else VratNoveKosticky();    //Pokud je kolize, zrotuj je zpátky
        }
        public bool JeKolize()  //Testuji, zda při transformaci sady kostiček nějaká nevyleze mimo hrací plochu
        {                       
            foreach (Kosticka kosticka in kostickyNove)
            {
                int indexX = kosticka.X + PoziceX - 1;
                int indexY = kosticka.Y + PoziceY;
                if (indexX < 0 || indexX >= Hra.SirkaPlochy) return true;
                if (indexY < 0 || indexY >= Hra.VyskaPlochy) return true;
                if (Hra.kostickyPlocha[indexX, indexY] > 0) return true;    //nebo se nedostane na zaplněnou pozici
            }                                                               //hrací plochy
            return false;
        }
        public void VratNoveKosticky()  //Jen vrátím modifikované nové kostičky na původní stav
        {
            for (int i = 0; i < kosticky.Length; i++)
            {
                kostickyNove[i].X = kosticky[i].X;
                kostickyNove[i].Y = kosticky[i].Y;
            }
        }
    }
}
