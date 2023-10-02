using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtageConverter
{
    internal static class Utils
    {
        private static Random rand = new Random();
        private static string char36 = "abcdefghijklnmopqrstuvwxyz0123456789";
        public static string GetTempFolderPath()
        {
            return Path.Combine(Path.GetTempPath(), string.Concat(Enumerable.Range(0, 10).Select(x => char36[rand.Next(char36.Length)])));
        }
    }
}
