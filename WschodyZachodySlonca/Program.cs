using System.Collections.Generic;
using System.Reflection;

namespace WschodyZachodySlonca
{
    internal class Program
    {
        static string opcjeMenu = "\n1. Sprawdz aktualnie ustawiona date i wspolrzedne geograficzne" +
                    "\n2. Zmien date" +
                    "\n3. Zmien wspolrzedne geograficzne" +
                    "\n4. Oblicz wschod i zachod slonca dla danego dnia" +
                    "\n5. Oblicz wschod i zachod slonca dla calego miesiaca" +
                    "\n6. Oblicz wschod i zachod slonca dla calego roku" +
                    "\n7. Zmien ustawienie korekcji bledu zalamania swiatla" +
                    "\n999. Test" +
                    "\n8. Info o programie\n" +
                    "\n9. Wyjdz z programu.\n";

        const int minOpcji = 1;
        static int maxOpcji = opcjeMenu.Count(c => c.Equals('\n')) - 2;

        static void Main(string[] args)
        {
            Sortowanie.sortowanieTemp();
            Obliczenia obl = new Obliczenia();

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Czesc\nCo chcesz zrobic?\n\nKorekcja bledu wlaczona? [{Obliczenia.korekcjaBledu}]\n" + Program.opcjeMenu);

                int wyborMenu;
                // TODO
                // warunek na 999 tymczasowy
                if ((!int.TryParse(Console.ReadLine(), out wyborMenu) || wyborMenu < minOpcji || wyborMenu > maxOpcji) && !(wyborMenu == 999 || wyborMenu == 2137))
                {
                    continue;
                }

                switch (wyborMenu)
                {
                    case 1:
                        Obliczenia.SprawdzDane(obl);
                        break;
                    case 2:
                        obl.UstawDate();
                        break;
                    case 3:
                        obl.UstawWspolrzedneGeo();
                        break;
                    case 4:
                        Console.Clear();
                        Console.WriteLine($"Podaj dzien z {obl.data.Month} miesiaca:");
                        int input;
                        while (true)
                        {
                            if (int.TryParse(Console.ReadLine(), out input))
                            {
                                if (!obl.SprawdzCzyDzienIstnieje(input, obl.data.Month, obl.data.Year))
                                {
                                    Console.WriteLine("\nDzien nie istnieje!\nNacisnij Enter, aby kontynuowac.");
                                    Console.ReadLine();
                                    break;
                                }
                                Console.WriteLine($"Dla {input}.{obl.data.Month}.{obl.data.Year} ({obl.szerokoscGeo}N {obl.dlugoscGeo}E):");

                                // TODO
                                // da się to zrobić lepiej i ładniej - wystarczą dwa fory zagniezdzone.
                                // Oba fory liczą od 1 do 0 i wywołują metodę ponizej.
                                // Zadne skomplikowane konwersje nie sa potrzebne...
                                // xx = k
                                // 11 = 3
                                // 10 = 2
                                // 01 = 1
                                // 00 = 0
                                for (int k = 3; k >= 0; k--)
                                {
                                    string str = Convert.ToString(k, 2).PadLeft(2, '0');
                                    char[] charArr = str.ToCharArray();

                                    obl.ObliczWschodZachod(obl.data.AddDays(-(int)char.GetNumericValue(charArr[1])),
                                        Convert.ToBoolean((int)char.GetNumericValue(charArr[0])), (int)char.GetNumericValue(charArr[1]));
                                }
                                //obl.ObliczWschodZachod(obl.data.AddDays(-1), Convert.ToBoolean(1), 1);
                                //obl.ObliczWschodZachod(obl.data, Convert.ToBoolean(1), 0);
                                //obl.ObliczWschodZachod(obl.data.AddDays(-1), Convert.ToBoolean(0), 1);
                                //obl.ObliczWschodZachod(obl.data, Convert.ToBoolean(0), 0);
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
                            Console.WriteLine($"Dla {i}.{obl.data.Month}.{obl.data.Year} ({obl.szerokoscGeo}N {obl.dlugoscGeo}E):");

                            for (int k = 3; k >= 0; k--)
                            {
                                string str = Convert.ToString(k, 2).PadLeft(2, '0');
                                char[] charArr = str.ToCharArray();

                                obl.ObliczWschodZachod(obl.data.AddDays(-(int)char.GetNumericValue(charArr[1]) - obl.data.Day + i),
                                    Convert.ToBoolean((int)char.GetNumericValue(charArr[0])), (int)char.GetNumericValue(charArr[1]));
                            }

                            //obl.ObliczWschodZachod(obl.data.AddDays(-1 - obl.data.Day + i), Convert.ToBoolean(1), 1);
                            //obl.ObliczWschodZachod(obl.data.AddDays(-obl.data.Day + i), Convert.ToBoolean(1), 0);
                            //obl.ObliczWschodZachod(obl.data.AddDays(-1 - obl.data.Day + i), Convert.ToBoolean(0), 1);
                            //obl.ObliczWschodZachod(obl.data.AddDays(-obl.data.Day + i), Convert.ToBoolean(0), 0);
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
                                Console.WriteLine($"Dla {j}.{i}.{obl.data.Year} ({obl.szerokoscGeo}N {obl.dlugoscGeo}E):");

                                for (int k = 3; k >= 0; k--)
                                {
                                    string str = Convert.ToString(k, 2).PadLeft(2, '0');
                                    char[] charArr = str.ToCharArray();

                                    obl.ObliczWschodZachod(obl.data.AddDays(-(int)char.GetNumericValue(charArr[1]) - obl.data.Day + j).AddMonths(-obl.data.Month + i),
                                        Convert.ToBoolean((int)char.GetNumericValue(charArr[0])), (int)char.GetNumericValue(charArr[1]));
                                }

                                //obl.ObliczWschodZachod(obl.data.AddDays(-1 - obl.data.Day + j).AddMonths(-obl.data.Month + i), Convert.ToBoolean(1), 1);
                                //obl.ObliczWschodZachod(obl.data.AddDays(-obl.data.Day + j).AddMonths(-obl.data.Month + i), Convert.ToBoolean(1), 0);
                                //obl.ObliczWschodZachod(obl.data.AddDays(-1 - obl.data.Day + j).AddMonths(-obl.data.Month + i), Convert.ToBoolean(0), 1);
                                //obl.ObliczWschodZachod(obl.data.AddDays(-obl.data.Day + j).AddMonths(-obl.data.Month + i), Convert.ToBoolean(0), 0);
                            }
                        }
                        obl.Kontynuuj();
                        break;
                    case 7:
                        Obliczenia.korekcjaBledu = !Obliczenia.korekcjaBledu;
                        break;
                    case 8:
                        Info();
                        break;
                    case 9:
                        return;
                    case 999:
                        WalidacjaXml.WalidujXml();
                        Console.ReadLine();
                        break;
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
                "Amen.");
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
    }
}