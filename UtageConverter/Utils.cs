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

        public static void SimpleExec(string argLine, string exeFilePath)
        {
            var process = Process.Start(new ProcessStartInfo(exeFilePath)
            {
                CreateNoWindow = true,
                Arguments = argLine,
            });
            process.WaitForExit();
        }
    }
}
