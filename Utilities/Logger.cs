using System;
using System.IO;
using System.Text;

namespace Utilities
{
    public enum MessageType
    {
        Error,
        Information,
        Warning
    }

    public interface ILogger
    {
        void UpdateConfig(LoggerConfig config);
        bool WriteLog(string logMessage, MessageType messageType);
    }

    public class LoggerConfig
    {
        public string FolderPath { get; set; } = "Logs/";
        public string BaseFileName { get; set; } = "log";
    }

    public class Logger : ILogger
    {
        private LoggerConfig Config;
        private string SeparatorText = "------------------------------------------------------------------------------------------";

        public Logger() { }

        public Logger(LoggerConfig config)
        {
            Config = config;
        }

        public void UpdateConfig(LoggerConfig config)
        {
            Config = config;
        }

        public bool WriteLog(string logMessage, MessageType messageType = MessageType.Information)
        {
            try
            {
                using (TextWriter writer = new StreamWriter(GetFilePath(), true))
                {
                    writer.Write($"{GenerateFormattedLogMessage(logMessage, messageType)}{writer.NewLine}");
                }
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        private string GenerateFormattedLogMessage(string logMessage, MessageType messageType)
        {
            StringBuilder logMessageBuilder = new StringBuilder();
            logMessageBuilder.AppendLine(SeparatorText);
            logMessageBuilder.AppendLine($"Timestamp: {GetTimeStamp()}");
            logMessageBuilder.AppendLine($"Message Type: {GetMessageTypeValue(messageType)}");
            logMessageBuilder.AppendLine($"Message: {logMessage}");
            logMessageBuilder.AppendLine(SeparatorText);
            return logMessageBuilder.ToString();
        }

        private string GetMessageTypeValue(MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.Error: return "Error";
                case MessageType.Information: return "Information";
                case MessageType.Warning: return "Warning";
                default: return "Unknown";
            }
        }

        private string GetTimeStamp()
        {
            DateTime time = new DateTime();
            return time.ToString();
        }

        private string GetFilePath()
        {
            return $"{Path.Combine(Environment.CurrentDirectory, Config.BaseFileName, Config.BaseFileName)}_${GetDateString()}.txt";
        }

        private string GetDateString()
        {
            var date = new DateTime();
            return date.ToShortDateString();
        }
    }
}