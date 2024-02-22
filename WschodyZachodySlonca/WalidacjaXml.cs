using System.Xml;
using System.Xml.Schema;

public class WalidacjaXml
{
    public static void WalidujXml()
    {
        XmlReaderSettings schema = new XmlReaderSettings();
        schema.Schemas.Add(null, "LokalizacjeSchema.xsd");
        schema.ValidationType = ValidationType.Schema;
        schema.ValidationEventHandler += EventHandler;

        // TODO
        // Lokalizacje.xml trzeba zmienic na zmienna, zeby uzytkownik mogl uzywac dowolnych xmlek
        XmlReader lokalizacje = XmlReader.Create("Lokalizacje.xml", schema);

        while (lokalizacje.Read()) { }
    }

    static void EventHandler(object sender, ValidationEventArgs e)
    {
        if (e.Severity == XmlSeverityType.Warning)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("WARNING: " + e.Message + "\nFile location: " + Path.GetFullPath("Lokalizacje.xml"));

            Console.ResetColor();
        }
        else if (e.Severity == XmlSeverityType.Error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: " + e.Message + "\nFile location: " + Path.GetFullPath("Lokalizacje.xml"));
            Console.ResetColor();
        }
    }
}