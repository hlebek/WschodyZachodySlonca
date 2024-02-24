using System.Xml;
using System.Xml.Schema;

public class WalidacjaXml
{
    private static readonly string schemaFileName = "LokalizacjeSchema.xsd";
    private static string? lokalizacjaXmlki = null;
    private static int fail = 0;
    public static bool WalidujXml(List<string> xmlsPaths)
    {
        XmlReaderSettings schema = new XmlReaderSettings();
        schema.Schemas.Add(null, schemaFileName);
        schema.ValidationType = ValidationType.Schema;
        schema.ValidationEventHandler += EventHandler;

        foreach (string item in xmlsPaths)
        {
            FileStream plik = File.Open(item, FileMode.Open);
            lokalizacjaXmlki = Path.GetFullPath(plik.Name);
            plik.Close();

            XmlReader daneXml = XmlReader.Create(item, schema);
            while (daneXml.Read()) { }
            lokalizacjaXmlki = null;

            daneXml.Close();
        }

        if (fail > 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nLiczba wykrytych bledow: {fail}");
            Console.WriteLine("\nWalidacja plikow xml nie powiodla sie!");
            Console.ResetColor();
            fail = 0;
            return false;
        }

        fail = 0;
        return true;
    }

    static void EventHandler(object sender, ValidationEventArgs e)
    {
        if (e.Severity == XmlSeverityType.Warning)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nWARNING: " + e.Message + "\nFile location: " + $"{lokalizacjaXmlki}\n");
            Console.ResetColor();
        }
        else if (e.Severity == XmlSeverityType.Error)
        {
            fail++;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nERROR: " + e.Message + "\nFile location: " + $"{lokalizacjaXmlki}");
            Console.ResetColor();
        }
    }
}