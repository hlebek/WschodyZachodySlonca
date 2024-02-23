using System.Collections.Generic;
using System.Reflection;

namespace WschodyZachodySlonca
{
    internal class Program
    {
        static string opcjeMenu = "\n1. Sprawdz aktualnie ustawiona date i wspolrzedne geograficzne" +
                    "\n2. Zmien date" +
                    "\n3. Zmien wspolrzedne geograficzne" +
                    "\n4. Wybierz lokalizacje" +
                    "\n5. Dodaj lokalizacje" +
                    "\n6. Zapisz lokalizacje" +
                    "\n-. Wczytaj lokalizacje" +
                    "\n-. Edytuj lokalizacje" +
                    "\n-. Wyswietl lokalizacje" +
                    "\n-. Wyczysc lokalizacje" +
                    "\n11. Oblicz wschod i zachod slonca dla danego dnia" +
                    "\n12. Oblicz wschod i zachod slonca dla calego miesiaca" +
                    "\n13. Oblicz wschod i zachod slonca dla calego roku" +
                    "\n14. Zmien ustawienie korekcji bledu zalamania swiatla" +
                    "\n999. Test" +
                    "\n15. Info o programie\n" +
                    "\n16. Wyjdz z programu.\n";

        const int minOpcji = 1;
        static int maxOpcji = opcjeMenu.Count(c => c.Equals('\n')) - 2;

        static void Main(string[] args)
        {
            List<string> xmlkiSciezki = new List<string>();
            Obliczenia obl = new Obliczenia();

            foreach (string arg in args)
            {
                if(arg.Contains("-x ") && !arg.Remove(0, 3).Contains(" ") && arg.Remove(0, arg.Length - 4).Contains(".xml"))
                {
                    xmlkiSciezki.Add(arg.Remove(0,3));
                }
            }

            if(!WalidacjaXml.WalidujXml(xmlkiSciezki))
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("\nNiepoprawne lokalizacje nie zostana uwzglednione!\n");
                Console.ResetColor();
                obl.Kontynuuj();
                Console.Clear();
            }

            Lokalizacje.SortowanieListyMiast(xmlkiSciezki);

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
                        if (!Lokalizacje.WyswietlListeMiast())
                            continue;
                        Console.WriteLine("\nPodaj nazwe lokalizacji:");
                        string miasto = Console.ReadLine();
                        double[] wspolrzedne;

                        if (!string.IsNullOrEmpty(miasto))
                        {
                            miasto = miasto.ToLower().Insert(1, miasto[0].ToString().ToUpper()).Remove(0, 1);
                        }
                        else
                        {
                            continue;
                        }

                        if(Lokalizacje.listaMiast.ContainsKey(miasto))
                        {
                            wspolrzedne = Lokalizacje.WybierzMiasto(miasto);
                        }
                        else
                        {
                            Console.WriteLine("\nPodanego miasta nie ma na liscie.");
                            obl.Kontynuuj();
                            continue;
                        }

                        obl.UstawWspolrzedneGeo(wspolrzedne[0], wspolrzedne[1]);
                        break;
                    case 5:
                        Console.Clear();
                        Console.WriteLine("Podaj nazwe nowej lokalizacji:");
                        string nazwaLok;
                        double szGeo = double.MaxValue, dluGeo = double.MaxValue;
                        nazwaLok = Console.ReadLine();

                        while (!nazwaLok.All(char.IsLetter) || string.IsNullOrEmpty(nazwaLok) || string.IsNullOrWhiteSpace(nazwaLok))
                        {
                            Console.Clear();
                            Console.WriteLine("Podaj poprawna nazwe, skladajaca sie z samych liter:");
                            nazwaLok = Console.ReadLine();
                        }

                        nazwaLok = nazwaLok.ToLower().Insert(1, nazwaLok[0].ToString().ToUpper()).Remove(0, 1);
                        Console.Clear();
                        Console.WriteLine("Podaj szerokosc geograficzna °N:");

                        // TODO
                        // magic numbery
                        while (!double.TryParse(Console.ReadLine(), out szGeo) || szGeo > 90 || szGeo <-90)
                        {
                            Console.Clear();
                            Console.WriteLine("Podaj poprawna wartosc z przedzialu <-90;90>°N:");
                        }

                        Console.Clear();
                        Console.WriteLine("Podaj dlugosc geograficzna °E:");

                        // TODO
                        // magic numbery
                        while (!double.TryParse(Console.ReadLine(), out dluGeo) || dluGeo > 180 || dluGeo < -180)
                        {
                            Console.Clear();
                            Console.WriteLine("Podaj poprawna wartosc z przedzialu <-180;180>°E:");
                        }

                        Console.Clear();
                        Console.WriteLine($"Podana lokalizacja to: {nazwaLok} - {szGeo}°N, {dluGeo}°E");
                        if (!Lokalizacje.listaMiast.ContainsKey(nazwaLok))
                        {
                            Lokalizacje.listaMiast.Add(nazwaLok, new Dictionary<double, double>() { { szGeo, dluGeo } });
                        }
                        break;
                    case 6:
                        Console.Clear();
                        Console.WriteLine("Podaj nazwe nowego pliku:");
                        string nazwaPliku = Console.ReadLine();

                        while (string.IsNullOrEmpty(nazwaPliku) || string.IsNullOrWhiteSpace(nazwaPliku) || File.Exists(nazwaPliku + ".xml"))
                        {
                            Console.Clear();
                            Console.WriteLine("Nazwa pliku jest niepoprawna lub plik juz istnieje. Podaj inna nazwe:");
                            nazwaPliku = Console.ReadLine();
                        }

                        Lokalizacje.ZapiszDoXml(Lokalizacje.listaMiast, nazwaPliku);
                        Console.WriteLine("Zapisano plik: " + Path.GetFullPath(nazwaPliku));
                        break;
                    case 7:
                        Console.Clear();

                        break;
                    case 8:
                        Console.Clear();

                        break;
                    case 9:
                        Console.Clear();

                        break;
                    case 10:
                        Console.Clear();

                        break;
                    case 11:
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

                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.WriteLine($"Dla {input}.{obl.data.Month}.{obl.data.Year} ({obl.szerokoscGeo}°N {obl.dlugoscGeo}°E):");
                                Console.ResetColor();

                                for (int i = 1; i >= 0; i--)
                                {
                                    for (int j = 1; j >= 0; j--)
                                    {
                                        obl.ObliczWschodZachod(obl.data.AddDays(-j), Convert.ToBoolean(i), j);
                                    }
                                }

                                obl.Kontynuuj();
                                break;
                            }
                        }
                        break;
                    case 12:
                        Console.Clear();
                        for (int i = 1; i <= 31; i++)
                        {
                            if (!obl.SprawdzCzyDzienIstnieje(i, obl.data.Month, obl.data.Year))
                            {
                                break;
                            }

                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine($"Dla {i}.{obl.data.Month}.{obl.data.Year} ({obl.szerokoscGeo}°N {obl.dlugoscGeo}°E):");
                            Console.ResetColor();

                            for (int x = 1; x >= 0; x--)
                            {
                                for (int y = 1; y >= 0; y--)
                                {
                                    obl.ObliczWschodZachod(obl.data.AddDays(- y - obl.data.Day + i), Convert.ToBoolean(x), y);
                                }
                            }
                        }
                        obl.Kontynuuj();
                        break;
                    case 13:
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
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.WriteLine($"Dla {j}.{i}.{obl.data.Year} ({obl.szerokoscGeo}°N {obl.dlugoscGeo}°E):");
                                Console.ResetColor();

                                for (int x = 1; x >= 0; x--)
                                {
                                    for (int y = 1; y >= 0; y--)
                                    {
                                        obl.ObliczWschodZachod(obl.data.AddDays(- y - obl.data.Day + j).AddMonths(- obl.data.Month + i), Convert.ToBoolean(x), y);
                                    }
                                }
                            }
                        }
                        obl.Kontynuuj();
                        break;
                    case 14:
                        Obliczenia.korekcjaBledu = !Obliczenia.korekcjaBledu;
                        break;
                    case 15:
                        Info();
                        break;
                    case 16:
                        return;
                    case 999:
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
            Console.ForegroundColor = ConsoleColor.Yellow;
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
            Console.ResetColor();
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