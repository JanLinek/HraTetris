using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HraTetris
{
    internal class Logo
    {
        string[] LogoRadky =
        {
            ("███ ███ ███ ██  █  ██"),
            (" █  █    █  █ █ █ █"),        //Naplnění pomocného pole stringů
            (" █  ██   █  ██  █  █"),
            (" █  █    █  █ █ █   █"),
            (" █  ███  █  █ █ █ ██")
        };
        public Kosticka kosticka;//Objekt pro jakékoliv vykreslování "kostiček"
        public List<Kosticka> kostickyLogo;//Logo je Listem kostiček, každá s relativní pozicí vůči PH rohu...
        
        public Logo()
        {
            kostickyLogo= new List<Kosticka>();
            for (int y = 0; y < LogoRadky.Length;y++)
            {
                int x = 0;
                foreach (char znak in LogoRadky[y])             //Naplnění Listu loga
                {                                        //Ze stringu je použit jen znak "█", mezera se přeskočí
                    if (znak=='█') kostickyLogo.Add(new Kosticka(x, y));
                    x++;                                                
                }
            }
        }
        public void VykresliLogo(int poziceX,int poziceY,int barva) //při volání vykreslení je v parametru i barva
        {
            foreach (Kosticka kosticka in kostickyLogo) kosticka.VykresliKosticku(poziceX,poziceY,barva);
        }
    }
}
