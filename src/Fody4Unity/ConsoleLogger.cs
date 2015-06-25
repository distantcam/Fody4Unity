using Microsoft.Build.Framework;
using static System.Console;

namespace Fody4Unity
{
    class ConsoleLogger : ILogger
    {
        private const string MESSAGE_FORMAT = "[{0}] ({1}): {2}";
        private const string LONG_MESSAGE_FORMAT = "[{0}] ({1}) {3}({4}:{5}-{6}:{7}): {2}";
        private string weaverName;

        public void ClearWeaverName()
        {
            weaverName = "";
        }

        public void LogDebug(string message)
        {
            WriteLine(MESSAGE_FORMAT, "DEBUG", weaverName, message);
        }

        public void LogError(string message)
        {
            WriteLine(MESSAGE_FORMAT, "ERROR", weaverName, message);
        }

        public void LogError(string message, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber)
        {
            WriteLine(MESSAGE_FORMAT, "ERROR", weaverName, message, file, lineNumber, columnNumber, endLineNumber, endColumnNumber);
        }

        public void LogInfo(string message)
        {
            WriteLine(MESSAGE_FORMAT, "INFO", weaverName, message);
        }

        public void LogMessage(string message, MessageImportance level)
        {
            switch (level)
            {
                case MessageImportance.High:
                    WriteLine(MESSAGE_FORMAT, "HIGH", weaverName, message);
                    break;

                case MessageImportance.Normal:
                    WriteLine(MESSAGE_FORMAT, "NORMAL", weaverName, message);
                    break;

                case MessageImportance.Low:
                    WriteLine(MESSAGE_FORMAT, "LOW", weaverName, message);
                    break;
            }
        }

        public void LogWarning(string message)
        {
            WriteLine(MESSAGE_FORMAT, "WARNING", weaverName, message);
        }

        public void LogWarning(string message, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber)
        {
            WriteLine(MESSAGE_FORMAT, "WARNING", weaverName, message, file, lineNumber, columnNumber, endLineNumber, endColumnNumber);
        }

        public void SetCurrentWeaverName(string weaverName)
        {
            this.weaverName = weaverName;
        }
    }
}