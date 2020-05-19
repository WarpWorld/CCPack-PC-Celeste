using System;
using System.IO;

namespace Celeste.Mod.CrowdControl
{
    internal static class Log
    {
        static Log() => File.Delete("CrowdControl.log");
        public static void Message(string message) => File.AppendAllText("CrowdControl.log", $"[{DateTime.Now}] {message}{Environment.NewLine}");
        public static void Error(Exception ex) => Message(ex.ToString());
    }
}
