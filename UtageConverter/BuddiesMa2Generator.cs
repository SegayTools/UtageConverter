using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtageConverter.Bases;

namespace UtageConverter
{
    public static class BuddiesMa2Generator
    {
        private const string KEY = "0x4E2039D03C7AC1A36D10BAC801321D26";

        public static string GenerateMa2(GenerateTarget target, string scoreFolderPath, string maiConverterFilePath)
        {
            var outputTempFolder = Utils.GetTempFolderPath();
            var encryptedFilePath = "";
            var decryptCMD = $"decrypt \"{sdtFilePath}\" -o \"{outputTempFolder}\" -k \"{KEY}\"";

            var process = Process.Start(new ProcessStartInfo(maiConverterFilePath)
            {
                CreateNoWindow = true,
                Arguments = decryptCMD,
            });
            process.WaitForExit();

            var sdtFilePath = Directory.GetFiles(outputTempFolder).FirstOrDefault();

            if（string.IsNullOrWhiteSpace(sdtFilePath) || !File.Exists(sdtFilePath))
                return default;

            outputTempFolder = Utils.GetTempFolderPath();
            var convertCMD = $"sdttoma2 \"{sdtFilePath}\" -o \"{outputTempFolder}\" -b {target.Bpm}";

            process = Process.Start(new ProcessStartInfo(maiConverterFilePath)
            {
                CreateNoWindow = true,
                Arguments = decryptCMD,
            });
            process.WaitForExit();

            var ma2FilePath = Directory.GetFiles(outputTempFolder).FirstOrDefault();

            if（string.IsNullOrWhiteSpace(ma2FilePath) || !File.Exists(ma2FilePath))
                return default;

            var ma2Content = File.ReadAllText(ma2FilePath);

            return ma2Content;
        }
    }
}
