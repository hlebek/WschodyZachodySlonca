namespace WschodyZachodySlonca
{
    internal class Obliczenia
    {
        //TODO pozbyc sie magic numberow
        #region stale

        const int maxSzerokoscGeo = 90;

        const int maxDlugoscGeo = 180;

        const int maxDniDlugiMsc = 31;

        const int maxDniKrotkiMsc = 30;

        const int maxDniLuty = 28;

        const int maxDniLutyPrzestepny = 29;

        const int pierwszyDzien = 1;

        const int ileSekWMin = 60;

        #endregion

        #region zmienne

        public static bool korekcjaBledu = true;

        int oldSeconds = 0, seconds;

        #endregion

        #region wlasciwosci

        public DateTime data { get; set; }

        public double szerokoscGeo { get; set; }

        public double dlugoscGeo { get; set; }

        #endregion

        #region konstruktory

        public Obliczenia()
        {
            data = DateTime.Now;
            szerokoscGeo = 54.72;
            dlugoscGeo = 18.41;
        }

        #endregion

        #region metody

        /// <summary>
        /// Ustawia wspolrzedne geograficzne obiektu odczytujac wejscie z klawiatury.
        /// </summary>
        public void UstawWspolrzedneGeo(double? dlGeo = null, double? szGeo = null)
        {
            Console.Clear();

            if (dlGeo != null && szGeo != null)
            {
                dlugoscGeo = (double)dlGeo;
                szerokoscGeo = (double)szGeo;
            }
            else
            {
                double input;

                Console.WriteLine("Uwaga! Liczby niecałkowite podajemy z przecinkiem, a nie kropką!" +
                    "\nWprowadz szerokosc geograficzna w stopniach z minutami po przecinku." +
                    $"\nDla N wartosci dodatnie, dla S wartosci ujemne (-{maxSzerokoscGeo};{maxSzerokoscGeo}):");

                while (!double.TryParse(Console.ReadLine(), out input) || input < -maxSzerokoscGeo || input > maxSzerokoscGeo)
                    continue;
                szerokoscGeo = input;

                Console.WriteLine("\nWprowadz dlugosc geograficzna w stopniach z minutami po przecinku." +
                    $"\nDla E wartosci dodatnie, dla W wartosci ujemne (-{maxDlugoscGeo};{maxDlugoscGeo}):");

                while (!double.TryParse(Console.ReadLine(), out input) || input < -maxDlugoscGeo || input > maxDlugoscGeo)
                    continue;
                dlugoscGeo = input;
                Console.WriteLine("");
            }

            Console.WriteLine($"Szerokość geograficzna to: {szerokoscGeo}°N.\nDługość geograficzna to: {dlugoscGeo}°E.");
            Kontynuuj();
        }

        /// <summary>
        /// Zwraca false jesli dzien nie istnieje w kalendarzu. W innym przypadku zwraca true.
        /// </summary>
        /// <param name="dzien">Dzien miesiaca</param>
        /// <param name="miesiac">Miesiac</param>
        /// <param name="rok">Rok</param>
        /// <returns>Zwraca true, jesli dzien istnieje. W innym wypadku false.</returns>
        public bool SprawdzCzyDzienIstnieje(int dzien, int miesiac, int rok)
        {
            if (dzien < pierwszyDzien)
                return false;

            switch (miesiac)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    if (dzien <= maxDniDlugiMsc)
                        return true;
                    else
                        return false;
                case 2:
                    if (dzien <= maxDniLuty || (DateTime.IsLeapYear(rok) && dzien == maxDniLutyPrzestepny))
                        return true;
                    else
                        return false;
                default:
                    if (dzien <= maxDniKrotkiMsc)
                        return true;
                    else
                        return false;
            }
        }

        /// <summary>
        /// Oblicza wschod badz zachod slonca i wyswietla jego wartosc.
        /// </summary>
        /// <param name="dzien">Dzien miesiaca</param>
        /// <param name="miesiac">Miesiac</param>
        /// <param name="rok">Rok</param>
        /// <param name="korekcja">Czy korekcja bledu jest wlaczona?</param>
        /// <param name="wschod">Czy obliczany jest wschod? Jesli nie liczymy zachod.</param>
        /// <param name="czyDzienPoprzedni">Czy liczymy czas dla dnia poprzedniego?</param>
        public void ObliczWschodZachod(DateTime data, bool wschod = false, int czyDzienPoprzedni = 0)
        {
            double N1 = Math.Floor(275d * data.Month / 9);
            double N2 = Math.Floor((data.Month + 9d) / 12);
            double N3 = 1 + Math.Floor((data.Year - (4 * Math.Floor(data.Year / 4d)) + 2) / 3);
            double N = N1 - (N2 * N3) + data.Day - 30;
            double zenith = 90.83333333333333;
            double D2R = Math.PI / 180d;
            double R2D = 180d / Math.PI;
            double lnHour = dlugoscGeo / 15;
            double t;

            if (wschod)
            {
                // wschod
                t = N + ((6 - lnHour) / 24);
            } else {
                // zachod
                t = N + ((18 - lnHour) / 24);
            };

            double M = (0.9856 * t) - 3.289;
            double L = M + (1.916 * Math.Sin(M * D2R)) + (0.02 * Math.Sin(2 * M * D2R)) + 282.634;

            if (L > 360)
            {
                L = L - 360;
            }
            else if (L < 0)
            {
                L = L + 360;
            };

            double RA = R2D * Math.Atan(0.91764 * Math.Tan(L * D2R));

            if (RA > 360)
            {
                RA = RA - 360;
            }
            else if (RA < 0)
            {
                RA = RA + 360;
            };

            double Lquadrant = (Math.Floor(L / (90))) * 90;
            double RAquadrant = (Math.Floor(RA / 90)) * 90;
            RA = RA + (Lquadrant - RAquadrant);
            RA = RA / 15;
            double sinDec = 0.39782 * Math.Sin(L * D2R);
            double cosDec = Math.Cos(Math.Asin(sinDec));
            double cosH = (Math.Cos(zenith * D2R) - (sinDec * Math.Sin(szerokoscGeo * D2R))) / (cosDec * Math.Cos(szerokoscGeo * D2R));
            double H;

            if (wschod) {
                //wschod
                H = 360 - R2D * Math.Acos(cosH);
            } else {
                //zachod
                H = R2D * Math.Acos(cosH);
            };

            H = H / 15;
            double T = H + RA - (0.06571 * t) - 6.622;
            double UT = T - lnHour;

            if (UT > 24)
            {
                UT = UT - 24;
            }
            else if (UT < 0)
            {
                UT = UT + 24;
            }

            int offset = (int)(dlugoscGeo / 15);
            double localT = UT + offset;
            seconds = (int)(localT * 3600);

            if (korekcjaBledu)
                seconds += KorekcjaBledu(wschod, data.Month);

            double sec = seconds % 60;
            double minutes = seconds % 3600 / 60;
            double hours = seconds % 86400 / 3600;
            if (TimeZoneInfo.Local.IsDaylightSavingTime(new DateTime(data.Year, data.Month, data.Day, 12, 0, 0)))
                hours += 1;

            if (czyDzienPoprzedni == 1)
            {
                oldSeconds = seconds;
                return;
            }

            int sekundy = (seconds - oldSeconds) % 60;
            double minuty;
            if (seconds - oldSeconds < 0)
                minuty = Math.Ceiling((seconds - oldSeconds) % 3600d / 60);
            else
                minuty = Math.Floor((seconds - oldSeconds) % 3600d / 60);

            if (wschod)
            {
                Console.WriteLine($"Wschod o: {hours}h {minutes}min. {sec}sec.\nWschod szybciej o: {-minuty}min. i {-sekundy}sec.");
            }
            else
            {
                Console.WriteLine($"Zachod o: {hours}h {minutes}min. {sec}sec.\nZachod pozniej o: {minuty}min. i {sekundy}sec.\n");
            }
        }

        /// <summary>
        /// Zwraca wartosc korekcji bledu dla danego miesiaca.
        /// </summary>
        /// <param name="wschod">Czy korekcja jest dla wschodu slonca? Jesli nie to korekcja dla zachodu.</param>
        /// <param name="miesiac">Dla ktorego miesiaca korekcja?</param>
        /// <returns>Zwraca wartosc korekcji czasu w sekundach.</returns>
        public int KorekcjaBledu(bool wschod, int miesiac)
        {
            if (wschod)
            {
                switch (miesiac)
                {
                    case 1:
                        return 2 * ileSekWMin;
                    case 2:
                        return (int)(0.5 * ileSekWMin);
                    case 3:
                        return (int)(-0.5 * ileSekWMin);
                    case 4:
                        return -1 * ileSekWMin;
                    case 5:
                        return -2 * ileSekWMin;
                    case 6:
                        return -1 * ileSekWMin;
                    case 7:
                        return 0 * ileSekWMin;
                    case 8:
                        return 1 * ileSekWMin;
                    case 9:
                        return 2 * ileSekWMin;
                    case 10:
                        return (int)(2.5 * ileSekWMin);
                    case 11:
                        return 3 * ileSekWMin;
                    case 12:
                        return (int)(3.5 * ileSekWMin);
                    default:
                        break;
                }
            }
            else
            {
                switch (miesiac)
                {
                    case 1:
                        return -1 * ileSekWMin;
                    case 2:
                        return 0 * ileSekWMin;
                    case 3:
                        return 1 * ileSekWMin;
                    case 4:
                        return 2 * ileSekWMin;
                    case 5:
                        return 3 * ileSekWMin;
                    case 6:
                        return 4 * ileSekWMin;
                    case 7:
                        return (int)(3.5 * ileSekWMin);
                    case 8:
                        return (int)(2.5 * ileSekWMin);
                    case 9:
                        return 1 * ileSekWMin;
                    case 10:
                        return 0 * ileSekWMin;
                    case 11:
                        return (int)(-0.5 * ileSekWMin);
                    case 12:
                        return -1 * ileSekWMin;
                    default:
                        break;
                }
            }
            return 0;
        }

        /// <summary>
        /// Stopuje wiersz wywolania polecen.
        /// </summary>
        public void Kontynuuj()
        {
            Console.WriteLine("\nNacisnij Enter, aby kontynuowac...");
            Console.ReadLine();
        }

        /// <summary>
        /// Pozwala zmienic rok i miesiac obecnej daty.
        /// </summary>
        public void UstawDate()
        {
            Console.Clear();

            while (true)
            {
                int rok = 0;
                while (rok < 1 || rok >= 10000)
                {
                    Console.WriteLine("Podaj rok:");
                    int.TryParse(Console.ReadLine(), out rok);
                }

                int miesiac = 0;
                while (miesiac < 1 || miesiac > 12)
                {
                    Console.WriteLine("Podaj miesiac:");
                    int.TryParse(Console.ReadLine(), out miesiac);
                }

                data = data.Date.AddYears(rok - data.Year).AddMonths(miesiac - data.Month);

                Console.WriteLine($"Nowa data to: {data.Year}.{data.Month}");
                Kontynuuj();
                break;
            }
        }

        /// <summary>
        /// Sprawdza ustawiona date i wspolrzedne geograficzne danego obiektu.
        /// </summary>
        /// <param name="obl">Obiekt, dla ktorego chcemy sprawdzic ustawiona date oraz wspolrzedne geograficzne.</param>
        public static void SprawdzDane(Obliczenia obl)
        {
            Console.Clear();
            Console.WriteLine($"Aktualna data to: {obl.data.Year}Y {obl.data.Month}M\n" +
                $"Aktualne wspolrzedne geograficzne to (szerokosc, dlugosc): {obl.szerokoscGeo}°N, {obl.dlugoscGeo}°E");
            obl.Kontynuuj();
        }

        #endregion
    }
}