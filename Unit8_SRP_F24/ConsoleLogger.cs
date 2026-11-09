using System;
using System.Xml.Linq;
using Unit8_SRP_F24.Contracts;

namespace Unit8_SRP_F24;

public class ConsoleLogger : ILogger
{
    private static readonly object logLock = new object();

    public void LogMessage(string message, params object[] args)
    {
        string formattedMessage = string.Format(message, args);
        string type = "INFO";
        string logMessage = formattedMessage;

        if (formattedMessage.StartsWith("WARN: "))
        {
            type = "WARN";
            logMessage = formattedMessage.Substring(6);
        }
        else if (formattedMessage.StartsWith("ERROR: "))
        {
            type = "ERROR";
            logMessage = formattedMessage.Substring(7);
        }
        else if (formattedMessage.StartsWith("INFO: "))
        {
            logMessage = formattedMessage.Substring(6);
        }
        else if (formattedMessage.StartsWith("Error ", StringComparison.OrdinalIgnoreCase) ||
                 formattedMessage.StartsWith("Invalid ", StringComparison.OrdinalIgnoreCase))
        {
            type = "ERROR";
        }

        lock (logLock)
        {
            Console.WriteLine(formattedMessage);

            using (StreamWriter logfile = File.AppendText("log.xml"))
            {
                var entry = new XElement("log", new XElement("type", type), new XElement("message", logMessage));
                logfile.WriteLine(entry.ToString(SaveOptions.DisableFormatting));
            }
        }
    }
}
