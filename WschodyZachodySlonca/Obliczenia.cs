using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WschodyZachodySlonca
{
    internal class Obliczenia
    {
        #region stale

        const int mnozRok = 367;

        const int mnozNowyRok = 7;

        const int dalejNizLuty = 1;

        const int dzielNowyRok = 4;

        const int mnozMiesiac = 275;

        const int dzielMiesiac = 9;

        const double stalaA = 730531.5;

        const int stalaB = 36525;

        const double Wys = -0.833;

        #endregion

        #region zmienne

        double a, b, c, d, f, g, h, rad, wiek, Wschod, Zachod;

        int oldSeconds = 0, seconds;

        #endregion

        #region wlasciwosci

        public DateTime data { get; set; }

        public static double szerokoscGeo { get; set; }

        public static double dlugoscGeo { get; set; }

        #endregion

        #region konstruktory

        public Obliczenia()
        {
            data = DateTime.Now;
            szerokoscGeo = 54.35;
            dlugoscGeo = 18.67;
        }

        #endregion

        public static void UstawWspolrzedneGeo()
        {
            Console.Clear();

            double help = double.MaxValue;

            Console.WriteLine("Uwaga! Liczby niecałkowite podajemy z przecinkiem, a nie kropką!" +
                "\nWprowadz szerokosc geograficzna w stopniach z minutami po przecinku." +
                "\nDla N wartosci dodatnie, dla S wartosci ujemne (-90;90):");

            while (!double.TryParse(Console.ReadLine(), out help) || help < -90 || help > 90)
                continue;
            szerokoscGeo = help;

            Console.WriteLine("\nWprowadz dlugosc geograficzna w stopniach z minutami po przecinku." +
                "\nDla E wartosci dodatnie, dla W wartosci ujemne (-180;180):");


            while (!double.TryParse(Console.ReadLine(), out help) || help < -180 || help > 180)
                continue;
            dlugoscGeo = help;

            Console.WriteLine($"\nSzerokość geograficzna to: {szerokoscGeo}.\nDługość geograficzna to: {dlugoscGeo}." +
                $"\n\nNacisnij Enter, aby kontynuowac...");
            Console.ReadLine();
        }

        public bool SprawdzCzyDzienIstnieje(int dzien, int miesiac, int rok)
        {
            if (dzien < 1)
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
                    if (dzien > 31)
                        return false;
                    else
                        return true;
                case 2:
                    if (dzien > 28 || DateTime.IsLeapYear(rok) && dzien == 29)
                        return false;
                    else
                        return true;
                default:
                    if (dzien > 30)
                        return false;
                    else
                        return true;
            }
        }

        public void ObliczWschodZachod(int dzien, int miesiac, int rok, bool korekcja = true, bool wschod = false, int mode = 0)
        {
            double N1 = Math.Floor(275d * miesiac / 9);
            double N2 = Math.Floor((miesiac + 9d) / 12);
            double N3 = 1 + Math.Floor((rok - (4 * Math.Floor(rok / 4d)) + 2) / 3);
            double N = N1 - (N2 * N3) + dzien - 30;
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

            if (korekcja)
                seconds += KorekcjaBledu(wschod, miesiac);

            double sec = seconds % 60;
            double minutes = seconds % 3600 / 60;
            double hours = seconds % 86400 / 3600;
            if (TimeZoneInfo.Local.IsDaylightSavingTime(new DateTime(rok, miesiac, dzien, 12, 0, 0)))
                hours += 1;
            //hours = hours % 12;

            //Tutaj zrobic, ze jesli wybieramy wylistowanie calego miesiaca albo roku, to niech wylicza roznica miedzy aktualnym,
            //a poprzednim dniem.
            //Potem jeszcze trzeba dodac cos takiego dla wywolania obliczenia pojedynczego dnia.
            if (mode == 2)
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

            //Console.WriteLine($"Dla {dzien}.{miesiac}.{data.Year} ({szerokoscGeo}N {dlugoscGeo}E):\nWschod slonca o: ?h. ?min. ?sec.\nZachod slonca o: {hours}h. {minutes}min. {sec}sec." +
            //    $"\nDlugosc dnia: ?h ?min. ?sec.\n");

            //Console.WriteLine($"Dla {dzien}.{miesiac}.{data.Year} ({szerokoscGeo}N {dlugoscGeo}E):\nWschod slonca o: {Math.Floor(Wschod)}h. {Math.Round(Math.Truncate(Wschod)*100/60, 2)}min.\nZachod slonca o: {Math.Floor(Zachod)}h. {Math.Round(Math.Truncate(Zachod)*100/60, 2)}min." +
            //    $"\nDlugosc dnia: {Math.Floor(Zachod-Wschod)}h {Math.Round((Math.Truncate(Zachod-Wschod))*100/60, 2)}min.\n");

            //if (mode == 0 && wschod == false)
            //{
            //    Kontynuuj();
            //}
        }

        public int KorekcjaBledu(bool wschod, int miesiac)
        {
            if (wschod)
            {
                switch (miesiac)
                {
                    case 1:
                        return 2 * 60;
                    case 2:
                        return (int)(0.5 * 60);
                    case 3:
                        return (int)(-0.5 * 60);
                    case 4:
                        return -1 * 60;
                    case 5:
                        return -2 * 60;
                    case 6:
                        return -1 * 60;
                    case 7:
                        return 0 * 60;
                    case 8:
                        return 1 * 60;
                    case 9:
                        return 2 * 60;
                    case 10:
                        return (int)(2.5 * 60);
                    case 11:
                        return 3 * 60;
                    case 12:
                        return (int)(3.5 * 60);
                    default:
                        break;
                }
            }
            else
            {
                switch (miesiac)
                {
                    case 1:
                        return -1 * 60;
                    case 2:
                        return 0 * 60;
                    case 3:
                        return 1 * 60;
                    case 4:
                        return 2 * 60;
                    case 5:
                        return 3 * 60;
                    case 6:
                        return 4 * 60;
                    case 7:
                        return (int)(3.5 * 60);
                    case 8:
                        return (int)(2.5 * 60);
                    case 9:
                        return 1 * 60;
                    case 10:
                        return 0 * 60;
                    case 11:
                        return (int)(-0.5 * 60);
                    case 12:
                        return -1 * 60;
                    default:
                        break;
                }
            }
            return 0;
        }

        public void Kontynuuj()
        {
            Console.WriteLine("Nacisnij Enter, aby kontynuowac...");
            Console.ReadLine();
        }

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

                Console.WriteLine($"Nowa data to: {data.Year}.{data.Month}\n");
                Console.WriteLine($"Nacisnij Enter, aby kontynuowac...");
                Console.ReadLine();
                break;
            }
        }
    }
}
