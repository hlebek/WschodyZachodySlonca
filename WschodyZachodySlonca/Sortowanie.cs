using System.Xml;

namespace WschodyZachodySlonca
{
    public static class Sortowanie
    {
        public static SortedList<string, Dictionary<double, double>> listaMiast = new SortedList<string, Dictionary<double, double>>();
        public static void SortowanieListyMiast(List<string> xmlsPaths)
        {
            Dictionary<double, double> wspolrzedne = new Dictionary<double, double>();

            XmlDocument xml = new XmlDocument();

            foreach (string item in xmlsPaths)
            {
                xml.Load(item);
                XmlNodeList nodey = xml.SelectNodes("/Lokalizacje/Miejscowosc");

                foreach (XmlNode node in nodey)
                {
                    XmlNodeList childNodey = node.ChildNodes;
                    XmlAttributeCollection atrybut = node.Attributes;

                    double x, y;
                    double.TryParse(childNodey.Item(0).InnerText, out x);
                    double.TryParse(childNodey.Item(1).InnerText, out y);

                    wspolrzedne.Add(x, y);

                    if(!listaMiast.ContainsKey(atrybut.Item(0).InnerText))
                        listaMiast.Add(atrybut.Item(0).InnerText, wspolrzedne);

                    wspolrzedne.Clear();
                }
            }
        }
    }
}