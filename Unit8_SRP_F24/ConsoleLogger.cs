using System;
using System.Globalization;
using System.Xml.Linq;
using Unit8_SRP_F24.Contracts;

namespace Unit8_SRP_F24;

public class ConsoleLogger : ILogger
{
    private const string LogFilePath = "log.xml";
    private static readonly object fileLock = new object();

    public void LogMessage(string message, params object[] args)
    {
        // Format the message once so the console and XML file contain the same text.
        string formattedMessage = string.Format(CultureInfo.CurrentCulture, message, args);
        Console.WriteLine(formattedMessage);

        // Keep the entries inside one root element so log.xml remains valid XML.
        lock (fileLock)
        {
            XDocument document = File.Exists(LogFilePath)
                ? XDocument.Load(LogFilePath)
                : new XDocument(new XElement("logs"));

            document.Root!.Add(
                new XElement("log",
                    new XElement("timestamp", DateTimeOffset.Now.ToString("O", CultureInfo.InvariantCulture)),
                    new XElement("message", formattedMessage)));

            document.Save(LogFilePath);
        }
    }
}
