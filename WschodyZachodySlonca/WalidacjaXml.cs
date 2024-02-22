using System.Xml;
using System.Xml.Schema;

public class WalidacjaXml
{
    private static readonly string schemaFileName = "LokalizacjeSchema.xsd";
    private static string? lokalizacjaXmlki = null;
    private static int fail = 0;
    public static void WalidujXml(List<string> xmlsPaths)
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

            XmlReader lokalizacje = XmlReader.Create(item, schema);
            while (lokalizacje.Read()) { }
            lokalizacjaXmlki = null;
            lokalizacje.Close();
        }

        if (fail > 0)
        {
            fail = 0;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nWalidacja plikow xml nie powiodla sie.");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nWalidacja plikow xml powiodla sie.");
        }

        Console.ResetColor();
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
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nERROR: " + e.Message + "\nFile location: " + $"{lokalizacjaXmlki}");
            Console.ResetColor();
            fail++;
        }
    }
}