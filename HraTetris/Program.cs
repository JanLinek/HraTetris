using HraTetris;

Hra Hra;
int SirkaPlochy = 12;//Šířka hrací plochy 
int VyskaPlochy = 24;//Výška hrací plochy 
int OdstupZleva = 26;//odstup hrací plochy od pravého kraje konzole
bool HratZnovu = true;

do
{
    Hra = new Hra(OdstupZleva, SirkaPlochy, VyskaPlochy);//Založení nové instance hry
    Hra.VstupniObrazovka();
    Hra.VykresliMantinel();
    Hra.VykresliPlochu();
    Hra.HraciCyklus();
    HratZnovu = Hra.KonecHry();
}
while (HratZnovu);  //Dokud hráč chce, hraje se

