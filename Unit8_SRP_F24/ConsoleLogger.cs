using System;
using System.Xml.Linq;
using Unit8_SRP_F24.Contracts;

namespace Unit8_SRP_F24;

public class ConsoleLogger : ILogger
{
    private const string LogFilePath = "trade-log.xml";

    public void LogMessage(string message, params object[] args)
    {
        string formattedMessage = string.Format(message, args);
        Console.WriteLine(formattedMessage);

        int separator = formattedMessage.IndexOf(':');
        string prefix = separator > 0
            ? formattedMessage[..separator].Trim().ToUpperInvariant()
            : string.Empty;
        bool hasSeverity = prefix is "INFO" or "WARN" or "ERROR";
        string type = hasSeverity ? prefix : "INFO";
        string logMessage = hasSeverity
            ? formattedMessage[(separator + 1)..].Trim()
            : formattedMessage;

        XDocument document = File.Exists(LogFilePath)
            ? XDocument.Load(LogFilePath)
            : new XDocument(new XElement("logs"));

        document.Root!.Add(
            new XElement("log",
                new XElement("type", type),
                new XElement("message", logMessage)));
        document.Save(LogFilePath);
    }
}
