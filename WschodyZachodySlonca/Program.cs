namespace WschodyZachodySlonca
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Obliczenia obl = new Obliczenia();
            bool korekcja = true;

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Czesc\nCo chcesz zrobic?\n\nKorekcja bledu wlaczona? [{korekcja}]\n" +
                    "\n1. Sprawdz aktualnie ustawiona date i wspolrzedne geograficzne" +
                    "\n2. Zmien date" +
                    "\n3. Zmien wspolrzedne geograficzne" +
                    "\n4. Oblicz wschod i zachod slonca dla danego dnia" +
                    "\n5. Oblicz wschod i zachod slonca dla calego miesiaca" +
                    "\n6. Oblicz wschod i zachod slonca dla calego roku" +
                    "\n7. Zmien ustawienie korekcji bledu zalamania swiatla" +
                    "\n8. Info o programie\n" +
                    "\n9. Wyjdz z programu.\n");

                //Console.WriteLine($"Sin(90): {Math.Sin(90 * Math.PI / 180)}" +
                //    $"\nSin(180): {Math.Sin(180 * Math.PI / 180)}");

                int wyborMenu;
                if (!int.TryParse(Console.ReadLine(), out wyborMenu) || wyborMenu < 1 || wyborMenu > 7)
                {
                    Console.WriteLine("Niepoprawna wartosc!");
                }

                switch (wyborMenu)
                {
                    case 1:
                        SprawdzDane(obl);
                        break;
                    case 2:
                        obl.UstawDate();
                        break;
                    case 3:
                        Obliczenia.UstawWspolrzedneGeo();
                        break;
                    case 4:
                        Console.Clear();
                        Console.WriteLine($"Podaj dzien z {obl.data.Month} miesiaca:");
                        int help;
                        while (true)
                        {
                            if (int.TryParse(Console.ReadLine(), out help))
                            {
                                if (!obl.SprawdzCzyDzienIstnieje(help, obl.data.Month, obl.data.Year))
                                {
                                    break;
                                }
                                DateTime dzienPrzed = new DateTime(obl.data.Year, obl.data.Month, help).AddDays(-1);
                                Console.WriteLine($"Dla {help}.{obl.data.Month}.{obl.data.Year} ({Obliczenia.szerokoscGeo}N {Obliczenia.dlugoscGeo}E):");
                                obl.ObliczWschodZachod(dzienPrzed.Day, dzienPrzed.Month, dzienPrzed.Year, korekcja, true, 2);
                                obl.ObliczWschodZachod(help, obl.data.Month, obl.data.Year, korekcja, true);
                                obl.ObliczWschodZachod(dzienPrzed.Day, dzienPrzed.Month, dzienPrzed.Year, korekcja, false, 2);
                                obl.ObliczWschodZachod(help, obl.data.Month, obl.data.Year, korekcja, false);
                                obl.Kontynuuj();
                                break;
                            }
                        }
                        break;
                    case 5:
                        Console.Clear();
                        for (int i = 1; i <= 31; i++)
                        {
                            if (!obl.SprawdzCzyDzienIstnieje(i, obl.data.Month, obl.data.Year))
                            {
                                break;
                            }
                            DateTime dzienPrzed = new DateTime(obl.data.Year, obl.data.Month, i).AddDays(-1);
                            Console.WriteLine($"Dla {i}.{obl.data.Month}.{obl.data.Year} ({Obliczenia.szerokoscGeo}N {Obliczenia.dlugoscGeo}E):");
                            obl.ObliczWschodZachod(dzienPrzed.Day, dzienPrzed.Month, dzienPrzed.Year, korekcja, true, 2);
                            obl.ObliczWschodZachod(i, obl.data.Month, obl.data.Year, korekcja, true, 1);
                            obl.ObliczWschodZachod(dzienPrzed.Day, dzienPrzed.Month, dzienPrzed.Year, korekcja, false, 2);
                            obl.ObliczWschodZachod(i, obl.data.Month, obl.data.Year, korekcja, false, 1);
                        }
                        obl.Kontynuuj();
                        break;
                    case 6:
                        Console.Clear();
                        for (int i = 1; i <= 12; i++)
                        {
                            for (int j = 1; j <= 31; j++)
                            {
                                if (!obl.SprawdzCzyDzienIstnieje(j, i, obl.data.Year))
                                {
                                    break;
                                }
                                DateTime dzienPrzed = new DateTime(obl.data.Year, i, j).AddDays(-1);
                                Console.WriteLine($"Dla {j}.{i}.{obl.data.Year} ({Obliczenia.szerokoscGeo}N {Obliczenia.dlugoscGeo}E):");
                                obl.ObliczWschodZachod(dzienPrzed.Day, dzienPrzed.Month, dzienPrzed.Year, korekcja, true, 2);
                                obl.ObliczWschodZachod(j, i, obl.data.Year, korekcja, true, 1);
                                obl.ObliczWschodZachod(dzienPrzed.Day, dzienPrzed.Month, dzienPrzed.Year, korekcja, false, 2);
                                obl.ObliczWschodZachod(j, i, obl.data.Year, korekcja, false, 1);
                            }
                        }
                        obl.Kontynuuj();
                        break;
                    case 7:
                        korekcja = !korekcja;
                        break;
                    case 8:
                        Info();
                        break;
                    case 9:
                        return;
                    case 2137:
                        Papaj();
                        break;
                    default:
                        break;
                }
            }
        }

        private static void Papaj()
        {
            Console.Clear();
            Console.WriteLine("                                                                                 \n" +
                "                                  .,,.....                                      \n" +
                "                            ,,,,,,,,*/(((///*****//***,,.                       \n" +
                "                        ,,,,/#%########(((((///*/(//*******/**                  \n" +
                "                     ,*((/////((((((((/(/((///*//***,**,*,.,,,,*,,              \n" +
                "                   ##(//////*/***********/**/**,,,**,*,,,,,,,,,,,,,,*           \n" +
                "                *####(///***************,*******,,,,,,,....,,,,*****,,,         \n" +
                "              ((((((//***********,*,,,,,,,,,,,,*,**,,,,,,,,,,,********,**.      \n" +
                "            //*/////************,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,***,******   \n" +
                "           (((/////*************,,,,,,......,,,,,,,,,,,,....,,,,,,,,,*,**,****  \n" +
                "          #(((/(///*************,,,,,........,,,,,,,,,,,.....,,.....,,,***,***. \n" +
                "         ((//(////**************,,,,,,,.....,,,,,,,,,,,,.............,,,***,**, \n" +
                "        ,/*//////******************,,,,.,,,,,,,,,,,,,,,,,,,,,,.......,,,,*,,***,\n" +
                "        ///((/////******/**********,,,,,,,,,,,,,,,,,,,,,,,,,,,,,....,,,,*******.\n" +
                "        /(((//////**//*************,,,,,...,,,,,,,,,,,,,,,,,,,,,,,,,,,,********.\n" +
                "   ,,***(((//**//******/************,,,..,,,,,,,,,,,,******,**,,,,,,,********** \n" +
                " ,/***/,*/////*************/********,,,,,.,,,,,,,.,,,,,,,,,,,,,,,,,,,*******/** \n" +
                " **,,**/*****/************///*******,,,.,,,,,,,,,,,,,,,,,,,,,,,,,,,********//// \n" +
                " /*,,,,,***/*/*//********/**/********,,,,.,,,,,,,,,,,,,,,,,,,,,,,,,,******/*/// \n" +
                " **,*/(#/***//////**********///(//(((##(/**,**,,,,,,,,,,,,,,,,**,,,,,,*****/*/, \n" +
                "  *,***,,,/*/////***********//////(/(/((###(///**************,,,,,,,,****///*   \n" +
                "  **,***,,*///////*****,*,**/////%%&%%(#%%%##((/*****///(((###(/*/((///*////,   \n" +
                "   ,,*,*,*,///////****,,,,,,,,**,,,***///(((*/*/****/(#%%%&&&&&%%#((//*///*/    \n" +
                "   ,,,,****///////*****,,,,,,,,,*****/////*,*,**,,,,,/#%%##(##%(##(/(****/*     \n" +
                "    ,,,,,,*////////****,,,,,,,,,,,,,,,,,,,,,,,**,,,.,,,,*/******,,**,,****      \n" +
                "      ,,***(/////////***,,,,,,,,,,,,,,,,,,,,,***,,,.,,,...,,,,,,,,,,,,**/,      \n" +
                "           //////////****,,,,,,,,,,,,,****,,,***,,,.,,,,.,........,,,,***       \n" +
                "           *///////////**************//(*******,,,,.,****,,,.....,,,***/        \n" +
                "           (////////////*/*******///((*/**,,***,....,,,*(/**,,,,,,*****         \n" +
                "           ((/////////////**/////(#/*,,,,/*/****,,,,,**/#(///****//***          \n" +
                "          *%((//////////****///(/*****,,,,,,*(///////***(##((//////**           \n" +
                "          /%((((////////******/*******,*,*,****/(((/*****/(((///***,            \n" +
                "           (///((/////***********//*/(*************//***(##(/*****,             \n" +
                "           ,//////(/////****************,****/((((/******/(/*****               \n" +
                "             ****//(///////***********,*******************//*/**                \n" +
                "                /*//(////////*****,,,***,,,***////**********//,                 \n" +
                "                   ,*//////////*********,,,,,,,,,,*********/*                   \n" +
                "                        *///////***********,,,,,,,,,*******                     \n" +
                "                           */**///**********************                        \n" +
                "                                 *((***////////*///*,                           \n\n" +
                "Zostales zgwalcony przez papaja zbrodniarza szmate pedofila. Amen.");
            Console.ReadLine();
        }

        private static void Info()
        {
            Console.Clear();
            Console.WriteLine("Program do obliczania przyblizonego czasu wschodu i zachodu slonca\n" +
                "na danej wysokosci i szerokosci geograficznej danego dnia w roku.\n\n" +
                "Podczas korzystania z programu nalezy miec na uwadze, " +
                "ze podane wartosci sa wartosciami przyblizonymi.\n" +
                "W celu otrzymania dokladniejszych wynikow zostala udostepniona funkcja korekcji\n" +
                "bledu. Wciaz jednak podawane z jej pomoca wyniki sa tylko przyblizonymi wartosciami\n" +
                "i moga cechowac sie wiekszymi odstepstwami w roznicy dlugosci dnia w przypadkach, gdy\n" +
                "liczymy roznice wschodu badz zachodu slonca na granicy dwoch miesiecy.\n\n" +
                "Metoda bez uzycia korekcji jest bardziej ciagla, lecz charakteryzuje sie\n" +
                "w ogolnym przypadku wiekszymi bledami obliczeniowymi.\n\n" +
                "Wszelkie wyniki dla wschodu i zachodu slonca nie przekraczaja rzeczywistej\n" +
                "wartosci o 4 minuty w przod badz 2 minuty w tyl w przypadku braku korekcji,\n" +
                "natomiast z korekcja bledy nie przekraczaja 1,5 minuty w przod i 1 minuty w tyl.\n\n");

            Console.WriteLine("Aby kontynuowac nacisnij Enter...");
            Console.ReadLine();
        }

        private static void SprawdzDane(Obliczenia obl)
        {
            Console.Clear();
            Console.WriteLine($"Aktualna data to: {obl.data.Year}.{obl.data.Month}.{obl.data.Day}\n" +
                $"Aktualne wspolrzedne geograficzne to (szerokosc, dlugosc): {Obliczenia.szerokoscGeo}N {Obliczenia.dlugoscGeo}E" +
                $"\nNacisnij Enter aby kontynuowac...");
            Console.ReadLine();
        }
    }
}