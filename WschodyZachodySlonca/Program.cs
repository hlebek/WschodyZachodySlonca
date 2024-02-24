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
                    "\n5. Dodaj lokalizacje do listy" +
                    "\n6. Zapisz lokalizacje do pliku" +
                    "\n7. Wczytaj lokalizacje z pliku" +
                    "\n8. Edytuj wybrana lokalizacje" +
                    "\n9. Wyswietl liste lokalizacji" +
                    "\n10. Usun lokalizacje" +
                    "\n11. Usun wszytkie lokalizacje" +
                    "\n12. Zwaliduj xml-ke" +
                    "\n13. Oblicz wschod i zachod slonca dla danego dnia" +
                    "\n14. Oblicz wschod i zachod slonca dla calego miesiaca" +
                    "\n15. Oblicz wschod i zachod slonca dla calego roku" +
                    "\n16. Zmien ustawienie korekcji bledu zalamania swiatla" +
                    "\n17. Info o programie\n" +
                    "\n18. Wyjdz z programu.\n";


        // TODO
        // Dodac opcje, ze przy podawaniu nazwy xmlki akceptuje format z .xml w nazwie i bez. Tak jak dla pkt. 12 - walidacja xml
        // (ifem sprawdzac czy jest to rozszerzenie podane i jak tak to nic nie robic, a jak nie to dodac albo cos w ten desen)

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
                Console.WriteLine("\nNiepoprawne lokalizacje nie zostana uwzglednione!");
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

                if ((!int.TryParse(Console.ReadLine(), out wyborMenu) || wyborMenu < minOpcji || wyborMenu > maxOpcji) && wyborMenu != 2137)
                {
                    continue;
                }

                // TODO
                // Potworzyc osobne metody dla kazdego case'a i do tego zeliminowac duplikujacy sie kod (np. edycja, usuwanie lokalizacji albo liczenie wschodow i zachodow)
                switch (wyborMenu)
                {
                    case 1: // Sprawdz aktualnie ustawiona date i wspolrzedne geograficzne
                        // TODO
                        // Mozna zrobic, zeby defaultowo bylo stale wyswietlane w menu
                        Obliczenia.SprawdzDane(obl);
                        break;
                    case 2: // Zmien date
                        obl.UstawDate();
                        break;
                    case 3: // Zmien wspolrzedne geograficzne
                        obl.UstawWspolrzedneGeo();
                        break;
                    case 4: // Wybierz lokalizacje
                        if (!Lokalizacje.WyswietlListeMiast())
                            continue;
                        obl.Kontynuuj();

                        Console.WriteLine("\nPodaj nazwe lokalizacji:");
                        string miasto = Console.ReadLine();
                        double[] wspolrzedne;

                        if (!string.IsNullOrEmpty(miasto) && !string.IsNullOrWhiteSpace(miasto))
                        {
                            miasto = miasto.ToLower().Insert(1, miasto[0].ToString().ToUpper()).Remove(0, 1);
                        }
                        else
                        {
                            Console.WriteLine("Niepoprawna nazwa lokalizacji.");
                            obl.Kontynuuj();
                            continue;
                        }

                        if (Lokalizacje.listaMiast.ContainsKey(miasto))
                        {
                            wspolrzedne = Lokalizacje.WspolrzedneMiasta(miasto);
                        }
                        else
                        {
                            Console.WriteLine("\nPodanego miasta nie ma na liscie.");
                            obl.Kontynuuj();
                            continue;
                        }

                        obl.UstawWspolrzedneGeo(wspolrzedne[0], wspolrzedne[1]);
                        break;
                    
                        // TODO
                        // Zrobic, ze jak ktos podaje nazwe, ktora juz jest to informuje o tym
                    case 5: // Dodaj lokalizacje
                        Console.Clear();
                        Console.WriteLine("Podaj nazwe nowej lokalizacji:");
                        string nazwaLok;
                        double szGeo = double.MaxValue, dluGeo = double.MaxValue;
                        nazwaLok = Console.ReadLine();

                        while (!nazwaLok.All(char.IsLetter) || string.IsNullOrEmpty(nazwaLok) || string.IsNullOrWhiteSpace(nazwaLok))
                        {
                            Console.Clear();
                            Console.WriteLine("Podaj poprawna nazwe, skladajaca sie z samych liter i cyfr:");
                            nazwaLok = Console.ReadLine();
                        }

                        nazwaLok = nazwaLok.ToLower().Insert(1, nazwaLok[0].ToString().ToUpper()).Remove(0, 1);
                        Console.Clear();
                        Console.WriteLine("Podaj szerokosc geograficzna °N:");

                        // TODO
                        // magic numbery
                        while (!double.TryParse(Console.ReadLine(), out szGeo) || szGeo > 90 || szGeo < -90)
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
                    case 6: // Zapisz lokalizacje
                        Console.Clear();
                        Console.WriteLine("Podaj nazwe nowego pliku bez rozszerzenia:");
                        string nazwaPliku = Console.ReadLine();

                        while (string.IsNullOrEmpty(nazwaPliku) || string.IsNullOrWhiteSpace(nazwaPliku) || File.Exists(nazwaPliku + ".xml") || nazwaPliku.Contains(" "))
                        {
                            Console.Clear();
                            Console.WriteLine("Nazwa pliku jest niepoprawna lub plik juz istnieje (nazwa pliku nie moze zawierac spacji). Podaj inna nazwe:");
                            nazwaPliku = Console.ReadLine();
                        }

                        Lokalizacje.ZapiszDoXml(Lokalizacje.listaMiast, nazwaPliku);
                        Console.WriteLine("Zapisano plik: " + Path.GetFullPath(nazwaPliku) + ".xml");
                        obl.Kontynuuj();
                        break;
                    case 7: // Wczytaj lokalizacje
                        Console.Clear();
                        Console.WriteLine("Podaj nazwe pliku xml bez rozszerzenia do wczytania:");
                        nazwaPliku = Console.ReadLine();
                        nazwaPliku = nazwaPliku + ".xml";

                        if (string.IsNullOrEmpty(nazwaPliku) || string.IsNullOrWhiteSpace(nazwaPliku) || !File.Exists(nazwaPliku) || nazwaPliku.Contains(" "))
                        {
                            Console.Clear();
                            Console.WriteLine("Nazwa pliku jest niepoprawna lub plik nie istnieje. (nazwa pliku nie moze zawierac spacji)");
                            obl.Kontynuuj();
                            continue;
                        }


                        List<string> xml = new List<string>();
                        xml.Add(nazwaPliku);

                        if (!WalidacjaXml.WalidujXml(xml))
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine("\nNiepoprawne lokalizacje nie zostana uwzglednione!");
                            Console.ResetColor();
                            obl.Kontynuuj();
                            Console.Clear();
                        }

                        Lokalizacje.SortowanieListyMiast(xml);
                        break;
                    case 8: // Edytuj lokalizacje
                        if (!Lokalizacje.WyswietlListeMiast())
                            continue;

                        Console.WriteLine("\nPodaj nazwe lokalizacji, ktora chcesz edytowac:");
                        miasto = Console.ReadLine();

                        if (!string.IsNullOrEmpty(miasto) && !string.IsNullOrWhiteSpace(miasto))
                        {
                            miasto = miasto.ToLower().Insert(1, miasto[0].ToString().ToUpper()).Remove(0, 1);
                        }
                        else
                        {
                            Console.WriteLine("Niepoprawna nazwa lokalizacji.");
                            obl.Kontynuuj();
                            continue;
                        }

                        if (Lokalizacje.listaMiast.ContainsKey(miasto))
                        {
                            wspolrzedne = Lokalizacje.WspolrzedneMiasta(miasto);
                        }
                        else
                        {
                            Console.WriteLine("\nPodanego miasta nie ma na liscie.");
                            obl.Kontynuuj();
                            continue;
                        }

                        Console.Clear();
                        Console.WriteLine($"Wybrana lokalizacja: {miasto}\nSzerokosc geograficzna: {wspolrzedne[0]}°N\nDlugosc geograficzna: {wspolrzedne[1]}°E\n");
                        Console.WriteLine("Podaj nowa nazwe lokalizacji (wcisnij Enter, jesli nie chcesz jej zmieniac): ");
                        string nowaNazwa = Console.ReadLine();

                        if (!string.IsNullOrEmpty(nowaNazwa) && !string.IsNullOrWhiteSpace(nowaNazwa) && nowaNazwa.ToLower() != miasto.ToLower())
                        {
                            nowaNazwa = nowaNazwa.ToLower().Insert(1, nowaNazwa[0].ToString().ToUpper()).Remove(0, 1);
                            while (Lokalizacje.listaMiast.ContainsKey(nowaNazwa) || nowaNazwa.Contains(" "))
                            {
                                Console.WriteLine("\nPodana lokalizacja juz istnieje lub jest niepoprawna (nie moze zawierac spacji). Podaj inna nazwe:");
                                nowaNazwa = Console.ReadLine();
                            }
                        }
                        else
                        {
                            nowaNazwa = miasto;
                        }

                        Console.WriteLine("\nPodaj nowa wartosc szerokosci geograficznej [°N] (wcisnij Enter, jesli nie chcesz jej zmieniac): ");
                        double nowaWartoscGeo;
                        string nowaWartosc;
                        nowaWartosc = Console.ReadLine();

                        if (string.IsNullOrEmpty(nowaWartosc) || string.IsNullOrWhiteSpace(nowaWartosc))
                        {
                            nowaWartoscGeo = wspolrzedne[0];
                        }
                        else
                        {
                            // TODO
                            // Magic numbery
                            while (!double.TryParse(nowaWartosc, out nowaWartoscGeo) || nowaWartoscGeo < -90 || nowaWartoscGeo > 90)
                            {
                                Console.WriteLine("\nPodana wartosc szerokosci geograficznej jest niepoprawna. Poprawny wartosci mieszcza sie w przedziale <-90;90>." +
                                    "\nPodaj poprawna wartosc:");
                                nowaWartosc = Console.ReadLine();
                            }
                        }

                        wspolrzedne[0] = nowaWartoscGeo;

                        Console.WriteLine("\nPodaj nowa wartosc dlugosci geograficznej [°E] (wcisnij Enter, jesli nie chcesz jej zmieniac): ");
                        nowaWartosc = Console.ReadLine();

                        if (string.IsNullOrEmpty(nowaWartosc) || string.IsNullOrWhiteSpace(nowaWartosc))
                        {
                            nowaWartoscGeo = wspolrzedne[1];
                        }
                        else
                        {
                            // TODO
                            // Magic numbery
                            while (!double.TryParse(nowaWartosc, out nowaWartoscGeo) || nowaWartoscGeo < -180 || nowaWartoscGeo > 180)
                            {
                                Console.WriteLine("Podana wartosc dlugosci geograficznej jest niepoprawna. Poprawny wartosci mieszcza sie w przedziale <-180;180>." +
                                    "\nPodaj poprawna wartosc:");
                                nowaWartosc = Console.ReadLine();
                            }
                        }

                        wspolrzedne[1] = nowaWartoscGeo;

                        Lokalizacje.listaMiast.Remove(miasto);
                        Lokalizacje.listaMiast.Add(nowaNazwa, new Dictionary<double, double>() { { wspolrzedne[0], wspolrzedne[1] } });

                        Console.WriteLine($"\nZedytowana lokalizacje {miasto} pomyslnie.");
                        Console.WriteLine($"\nAktualne dane o lokalizacji - Nazwa: {nowaNazwa}, Wspolrzedne: {wspolrzedne[0]}°N {wspolrzedne[1]}°E");

                        // TODO
                        // Wyswietlanie lokalizacji po edycji.
                        obl.Kontynuuj();
                        break;
                    case 9: // Wyswietl lokalizacje
                        if (!Lokalizacje.WyswietlListeMiast())
                        {
                            obl.Kontynuuj();
                            break;
                        }
                        obl.Kontynuuj();
                        break;
                    case 10: // Usun lokalizacje
                        Console.Clear();
                        if (!Lokalizacje.WyswietlListeMiast())
                            continue;

                        Console.WriteLine("\nPodaj nazwe lokalizacji, ktora chcesz usunac:");
                        miasto = Console.ReadLine();

                        if (!string.IsNullOrEmpty(miasto) && !string.IsNullOrWhiteSpace(miasto))
                        {
                            miasto = miasto.ToLower().Insert(1, miasto[0].ToString().ToUpper()).Remove(0, 1);
                        }
                        else
                        {
                            Console.WriteLine("Niepoprawna nazwa lokalizacji.");
                            obl.Kontynuuj();
                            continue;
                        }

                        if (Lokalizacje.listaMiast.ContainsKey(miasto))
                        {
                            wspolrzedne = Lokalizacje.WspolrzedneMiasta(miasto);
                        }
                        else
                        {
                            Console.WriteLine("\nPodanego miasta nie ma na liscie.");
                            obl.Kontynuuj();
                            continue;
                        }

                        Lokalizacje.listaMiast.Remove(miasto);
                        Console.WriteLine($"\nUsunieto lokalizacje: {miasto}");
                        obl.Kontynuuj();
                        break;
                    case 11: // Wyczysc wszytkie lokalizacje
                        Console.Clear();
                        Console.WriteLine("Jestes pewny, ze chcesz usunac wszystkie lokalizacje? Wpisz [Y], aby potwierdzic.");
                        if (Console.ReadLine() == "Y")
                            Lokalizacje.listaMiast.Clear();
                        Console.WriteLine("\nLista lokalizacji zostala wyczyszczona.");
                        obl.Kontynuuj();
                        break;
                    case 12: // Zwaliduj poprawnosc xmlki
                        Console.Clear();
                        Console.WriteLine("Podaj nazwe/sciezke do xml-ki (bez spacji):");
                        string zwalidujXmlSciezka = Console.ReadLine();

                        if (!string.IsNullOrEmpty(zwalidujXmlSciezka) && !string.IsNullOrWhiteSpace(zwalidujXmlSciezka))
                        {
                            if (zwalidujXmlSciezka.Length >= 5)
                            {
                                if (zwalidujXmlSciezka.Remove(0, zwalidujXmlSciezka.Length - 4).Contains(".xml")) { }
                                else
                                {
                                    zwalidujXmlSciezka = zwalidujXmlSciezka + ".xml";
                                }
                            }
                            else
                            {
                                zwalidujXmlSciezka = zwalidujXmlSciezka + ".xml";
                            }

                            if (File.Exists(zwalidujXmlSciezka))
                            {
                                List<string> zwalidujXmlSciezkaList = new List<string>();
                                zwalidujXmlSciezkaList.Add(zwalidujXmlSciezka);
                                if (WalidacjaXml.WalidujXml(zwalidujXmlSciezkaList))
                                {
                                    Console.Clear();
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("Walidacja powiodla sie. Brak wykrytych bledow w pliku.");
                                    Console.ResetColor();
                                    obl.Kontynuuj();
                                    break;
                                }
                                else
                                {
                                    obl.Kontynuuj();
                                    break;
                                }    
                            }
                            else
                            {
                                Console.WriteLine("Nie znaleziono pliku.");
                                obl.Kontynuuj();
                                break;
                            }
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Niepoprawna nazwa.");
                            obl.Kontynuuj();
                            break;
                        }
                    case 13:
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
                    case 14:
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
                    case 15:
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
                    case 16:
                        Obliczenia.korekcjaBledu = !Obliczenia.korekcjaBledu;
                        break;
                    case 17:
                        Info();
                        break;
                    case 18:
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