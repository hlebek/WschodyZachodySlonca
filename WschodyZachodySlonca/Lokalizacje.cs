using System.Xml;

namespace WschodyZachodySlonca
{
    public static class Lokalizacje
    {
        public static SortedList<string, Dictionary<double, double>> listaMiast = new SortedList<string, Dictionary<double, double>>();
        private static readonly int maxDluGeo = 180;
        private static readonly int minDluGeo = -180;
        private static readonly int maxSzGeo = 90;
        private static readonly int minSzGeo = -90;

        public static void SortowanieListyMiast(List<string> xmlsPaths)
        {
            XmlDocument xml = new XmlDocument();

            foreach (string item in xmlsPaths)
            {
                xml.Load(item);
                XmlNodeList nodey = xml.SelectNodes("/Lokalizacje/Miejscowosc");

                foreach (XmlNode node in nodey)
                {
                    Dictionary<double, double> wspolrzedne = new Dictionary<double, double>();
                    XmlNodeList childNodey = node.ChildNodes;
                    XmlAttributeCollection atrybut;

                    if (node.Attributes.Count != 0)
                    {
                        atrybut = node.Attributes;
                    }
                    else
                    {
                        continue;
                    }


                    double x, y;
                    x = double.Parse(childNodey.Item(0).InnerText.Replace('.', ','));
                    y = double.Parse(childNodey.Item(1).InnerText.Replace('.', ','));

                    wspolrzedne.Add(x, y);

                    Console.WriteLine($"Atrybut: " + atrybut.Item(0).InnerText);

                    // TODO
                    // Bierze tutaj zerowy atrybut, a u gory zerowa i pierwszy node przez co jak ktos powiesza w xmlce to pomimo, ze nazwy nodeow i atrybutow beda niepoprawne to i tak wezmie z nich wartosci - do poprawy
                    if (!listaMiast.ContainsKey(atrybut.Item(0).InnerText) && x <= maxDluGeo && x >= minDluGeo && y <= maxSzGeo && y >= minSzGeo)
                        listaMiast.Add(atrybut.Item(0).InnerText, wspolrzedne);
                }
            }
            Console.Clear();
        }

        public static bool WyswietlListeMiast()
        {
            if (listaMiast.Count > 0)
            {
                foreach (var item in listaMiast)
                {
                    foreach (var ite in item.Value)
                    {
                        Console.WriteLine(item.Key + " - " + ite.Key + "°N " + ite.Value + "°E");
                    }
                }
                return true;
            }
            else
            {
                Console.WriteLine("Brak predefiniowanych lokalizacji.");
                Console.WriteLine("Nacisnij Enter, aby kontynuowac...");
                Console.ReadLine();
                return false;
            }
        }

        public static double[] WybierzMiasto(string miasto)
        {
            KeyValuePair<double, double> placeHolder = listaMiast.Values[listaMiast.IndexOfKey(miasto)].FirstOrDefault();
            double[] wspolrzedne = { placeHolder.Key, placeHolder.Value };

            return wspolrzedne;
        }

        public static void ZapiszDoXml(SortedList<string, Dictionary<double, double>> lista, string nazwaPliku)
        {
            XmlWriter writer = XmlWriter.Create(nazwaPliku + ".xml");
            writer.WriteStartElement("Lokalizacje");

            for (int i = 0; i < lista.Count; i++)
            {
                writer.WriteStartElement("Miejscowosc", null);
                writer.WriteAttributeString("Nazwa", lista.Keys[i].ToString());
                writer.WriteElementString("DlugoscGeo", lista.Values[i].Keys.ElementAt(0).ToString().Replace(',', '.'));
                writer.WriteElementString("SzerokoscGeo", lista.Values[i].Values.ElementAt(0).ToString().Replace(',', '.'));
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }
    }
}