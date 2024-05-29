using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Tavstal.TLibrary.Utils
{
    /// <summary>
    /// Logger helper used to log messages to console and log file.
    /// </summary>
    public static class LoggerHelper
    {
        private static readonly string _pluginName = "Uconomy";

        /// <summary>
        /// Logs a rich message to the console and the log file
        /// </summary>
        /// <param name="message"></param>
        /// <param name="prefix"></param>
        public static void LogRich(object message, string prefix = "&a[INFO] >&f")
        {
            string text = string.Format("&b[{0}] {1} {2}", _pluginName, prefix, message.ToString());
            try
            {
                ConsoleColor oldColor = Console.ForegroundColor;
                using (StreamWriter streamWriter = File.AppendText(Path.Combine(Rocket.Core.Environment.LogsDirectory, Rocket.Core.Environment.LogFile)))
                {
                    streamWriter.WriteLine(string.Concat("[", DateTime.Now, "] ", FormatHelper.ClearFormaters(text)));
                    streamWriter.Close();
                }
                FormatHelper.SendFormatedConsole(text);
                Console.ForegroundColor = oldColor;
            }
            catch
            {
                Rocket.Core.Logging.Logger.Log(text);
            }
        }

        /// <summary>
        /// Logs a rich message to the console as warning
        /// </summary>
        /// <param name="message"></param>
        /// <param name="prefix"></param>
        public static void LogRichWarning(object message, string prefix = "&e[WARNING] >&f")
        {
            LogRich(message, prefix);
        }

        /// <summary>
        /// Logs a rich message to the console as exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="prefix"></param>
        public static void LogRichException(object message, string prefix = "&6[EXCEPTION] >&f")
        {
            LogRich(message, prefix);
        }

        /// <summary>
        /// Logs a rich message to the console as error
        /// </summary>
        /// <param name="message"></param>
        /// <param name="prefix"></param>
        public static void LogRichError(object message, string prefix = "&c[ERROR] >&f")
        {
            LogRich(message, prefix);
        }

        /// <summary>
        /// Logs a rich message to the console as command response
        /// </summary>
        /// <param name="message"></param>
        /// <param name="prefix"></param>
        public static void LogRichCommand(object message, string prefix = "&9[COMMAND] >&f")
        {
            LogRich(message, prefix);
        }

        /// <summary>
        /// Logs a message to the console and log file
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        /// <param name="prefix"></param>
        public static void Log(object message, ConsoleColor color = ConsoleColor.Green, string prefix = "[INFO] >")
        {

            string text = string.Format("[{0}] {1} {2}", _pluginName, prefix, message.ToString());
            try
            {
                ConsoleColor oldColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                using (StreamWriter streamWriter = File.AppendText(Path.Combine(Rocket.Core.Environment.LogsDirectory, Rocket.Core.Environment.LogFile)))
                {
                    streamWriter.WriteLine(string.Concat("[", DateTime.Now, "] ", text));
                    streamWriter.Close();
                }
                Console.WriteLine(text);
                Console.ForegroundColor = oldColor;
            }
            catch
            {
                Rocket.Core.Logging.Logger.Log(text.Replace($"[{_pluginName}] ", ""), color);
            }
        }

        /// <summary>
        /// Logs a message to the console as warning
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        /// <param name="prefix"></param>
        public static void LogWarning(object message, ConsoleColor color = ConsoleColor.Yellow, string prefix = "[WARNING] >")
        {
            Log(message, color, prefix);
        }

        /// <summary>
        /// Logs a message to the console as exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        /// <param name="prefix"></param>
        public static void LogException(object message, ConsoleColor color = ConsoleColor.DarkYellow, string prefix = "[EXCEPTION] >")
        {
            Log(message, color, prefix);
        }

        /// <summary>
        /// Logs a message to the console as error
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        /// <param name="prefix"></param>
        public static void LogError(object message, ConsoleColor color = ConsoleColor.Red, string prefix = "[ERROR] >")
        {
            Log(message, color, prefix);
        }

        /// <summary>
        /// Logs the late init message to the console and log file
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        /// <param name="prefix"></param>
        public static void LogCommand(object message, ConsoleColor color = ConsoleColor.Blue, string prefix = "[Command] >")
        {
            string msg = message.ToString().Replace("((", "{").Replace("))", "}").Replace("[TShop]", "");
            int amount = msg.Split('{').Length;
            for (int i = 0; i < amount; i++)
            {
                Regex regex = new Regex(string.Format("{0}(.*?){1}", Regex.Escape("{"), Regex.Escape("}")), RegexOptions.RightToLeft);
                msg = regex.Replace(msg, "{" + "}");
            }

            Log(msg.Replace("{", "").Replace("}", ""), color, prefix);
        }
    }
}
