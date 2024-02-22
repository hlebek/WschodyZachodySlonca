using System.Xml;

namespace WschodyZachodySlonca
{
    public static class Sortowanie
    {
        public static void sortowanieTemp()
        {
            Dictionary<double, double> wspolrzedne = new Dictionary<double, double>();
            SortedList<string, Dictionary<double, double>> lista = new SortedList<string, Dictionary<double, double>>();

            XmlDocument xml = new XmlDocument();

            // TODO
            // Tutaj zmienić "\\Lokalizacje.xml" na zmienna, zeby uzytkownik mogl pobierac swoje xmlki
            string xmlPath = string.Concat(Directory.GetCurrentDirectory(), "\\Lokalizacje.xml");
            xml.Load("Lokalizacje.xml");
            XmlNodeList nodey = xml.SelectNodes("/Lokalizacje/Miejscowosc");

            foreach (XmlNode node in nodey)
            {
                XmlNodeList childNodey = node.ChildNodes;
                XmlAttributeCollection atrybut = node.Attributes;

                double x, y;
                double.TryParse(childNodey.Item(0).InnerText, out x);
                double.TryParse(childNodey.Item(1).InnerText, out y);

                wspolrzedne.Add(x, y);

                lista.Add(atrybut.Item(0).InnerText, wspolrzedne);

                wspolrzedne.Clear();
            }

            foreach (var item in lista)
            {
                Console.WriteLine(item.Key);
            }


            Console.ReadLine();
        }
    }
}
