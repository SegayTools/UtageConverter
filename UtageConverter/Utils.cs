using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtageConverter
{
    internal static class Utils
    {
        private static Random rand = new Random();
        private static string chars = "ABCDEFGHIJKLNMOPQRSTUVWXYZabcdefghijklnmopqrstuvwxyz0123456789";

        public static string GetTempFolderPath()
        {
            var dir = Path.Combine(Path.GetTempPath(), string.Concat(Enumerable.Range(0, 10).Select(x => chars[rand.Next(chars.Length)])));
            Directory.CreateDirectory(dir);
            return dir;
        }

        public static string GetTempFilePath(string ext = ".utageConverter.dat")
        {
            var filePath = Path.Combine(Path.GetTempPath(), string.Concat(Enumerable.Range(0, 10).Select(x => chars[rand.Next(chars.Length)])) + ext);
            if (File.Exists(filePath))
                return GetTempFilePath(ext);
            return filePath;
        }

        public static void SimpleExec(string argLine, string exeFilePath, string currentDirectory = default)
        {
            Log($"EXEC:\t{Path.GetFileName(exeFilePath)} {argLine.Trim()}", ConsoleColor.DarkGray);
            var startInfo = new ProcessStartInfo(exeFilePath)
            {
                CreateNoWindow = true,
                Arguments = argLine,
            };
            if (Directory.Exists(currentDirectory))
                startInfo.WorkingDirectory = currentDirectory;
            var process = Process.Start(startInfo);
            process.WaitForExit();
        }

        public static void Log(string msg, ConsoleColor color = ConsoleColor.White)
        {
            var backup = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ForegroundColor = backup;
        }
    }
}
