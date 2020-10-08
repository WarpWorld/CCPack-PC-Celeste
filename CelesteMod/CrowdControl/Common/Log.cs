using System;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;

namespace CrowdControl.Common
{
    public static class Log
    {
        [Conditional("DEBUG")]
        public static void Debug([CanBeNull] object message) => Write(Console.Out, message);

        public static void Message([CanBeNull] object message, bool omitTimestamp = false) => Write(Console.Out, message, omitTimestamp);

        public static void Error([CanBeNull] object message) => Write(Console.Error, message);

        public static void Error([NotNull] Exception ex, [NotNull] string message) =>
            Write(Console.Error, message + Environment.NewLine + ex);

        [Conditional("DEBUG")]
        public static void DebugFormat([NotNull] string message, params object[] args) =>
            WriteFormat(Console.Out, message, args);

        public static void MessageFormat([NotNull] string message, params object[] args) =>
            WriteFormat(Console.Out, message, args);

        public static void ErrorFormat([NotNull] string message, params object[] args) =>
            WriteFormat(Console.Error, message, args);

        private static void WriteFormat([NotNull] TextWriter writer, [NotNull] string format, [NotNull] object[] args) =>
            Write(writer, string.Format(format, args));

        private static void Write([NotNull] TextWriter writer, [CanBeNull] object message, bool omitTimestamp = false)
        {
            string m = (omitTimestamp ? string.Empty : $"[{DateTimeOffset.UtcNow}] ") + (message?.ToString() ?? "(null)");
            writer.Write("> ");
            writer.WriteLine(m);
            OnMessage?.Invoke(m);
        }

        public static void InvokeOnMessage([CanBeNull] object message, bool omitTimestamp = false)
            => OnMessage?.Invoke((omitTimestamp ? string.Empty : $"[{DateTimeOffset.UtcNow}] ") + (message?.ToString() ?? "(null)"));

        public static event Action<string> OnMessage;
    }
}
