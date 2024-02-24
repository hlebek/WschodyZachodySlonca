using System.Xml;
using System.Xml.Linq;

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
                    double x = double.MaxValue, y = double.MaxValue;
                    Dictionary<double, double> wspolrzedne = new Dictionary<double, double>();
                    XmlNodeList childNodey = node.ChildNodes;
                    XmlAttributeCollection atrybut;

                    if (node.Attributes.Count != 0 && childNodey.Count != 0)
                    {
                        atrybut = node.Attributes;
                    }
                    else
                    {
                        continue;
                    }

                    foreach (XmlNode childNode in childNodey)
                    {
                        if (!string.IsNullOrEmpty(childNode.LocalName) && !string.IsNullOrWhiteSpace(childNode.LocalName) && !string.IsNullOrEmpty(childNode.InnerText) && !string.IsNullOrWhiteSpace(childNode.InnerText))
                        {
                            if (childNode.LocalName.ToLower() == "dlugoscgeo")
                            {
                                if (!double.TryParse(childNode.InnerText.Replace('.', ','), out x))
                                {
                                    // Dzieki ustawieniu na max value dalsze warunki nie beda sprawdzane, poniewaz nizej jest
                                    // warunek if, ktory pomija dalsze wykonywanie iteracji petli
                                    x = double.MaxValue;
                                }
                            }
                            else if (childNode.LocalName.ToLower() == "szerokoscgeo")
                            {
                                if (!double.TryParse(childNode.InnerText.Replace('.', ','), out y))
                                {
                                    y = double.MaxValue;
                                }
                            }
                        }
                    }

                    if (x == double.MaxValue || y == double.MaxValue)
                        continue;

                    wspolrzedne.Add((double)x, (double)y);

                    foreach (XmlAttribute attr in atrybut)
                    {
                        if (!string.IsNullOrEmpty(attr.InnerText) && !string.IsNullOrWhiteSpace(attr.InnerText))
                        {
                            if (attr.LocalName.ToLower() == "nazwa")
                            {
                                if (!listaMiast.ContainsKey(attr.InnerText) && x <= maxDluGeo && x >= minDluGeo && y <= maxSzGeo && y >= minSzGeo)
                                {
                                    listaMiast.Add(attr.InnerText, wspolrzedne);
                                }
                            } 
                                
                        }
                    }
                }
            }
            Console.Clear();
        }

        public static bool WyswietlListeMiast()
        {
            Console.Clear();
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
                return false;
            }
        }

        public static double[] WspolrzedneMiasta(string miasto)
        {
            KeyValuePair<double, double> placeHolder = listaMiast.Values[listaMiast.IndexOfKey(miasto)].FirstOrDefault();
            double[] wspolrzedne = { placeHolder.Key, placeHolder.Value };

            return wspolrzedne;
        }

        public static void ZapiszDoXml(SortedList<string, Dictionary<double, double>> lista, string nazwaPliku)
        {
            nazwaPliku = nazwaPliku + ".xml";
            XmlWriter writer = XmlWriter.Create(nazwaPliku);
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
            XDocument plik = XDocument.Load(nazwaPliku);
            plik.Save(nazwaPliku);
        }
    }
}