using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UtageConverter.Bases;

namespace UtageConverter
{
    public static class BuddiesMa2Generator
    {
        private const string KEY = "0x4E2039D03C7AC1A36D10BAC801321D26";
        private static Regex noteCountRegex = new Regex("@\"T_REC_ALL\\s*(\\d+)\"");

        public static Ma2GenerateResult GenerateMa2(GenerateTarget target, string scoreFolderPath, string maiConverterFilePath)
        {
            if (string.IsNullOrWhiteSpace(maiConverterFilePath) || !File.Exists(maiConverterFilePath))
                return default;
            if (string.IsNullOrWhiteSpace(scoreFolderPath) || !Directory.Exists(scoreFolderPath))
                return default;

            var outputTempFolder = Utils.GetTempFolderPath();
            var encryptedFilePath = Directory.GetFiles(scoreFolderPath, $"{target.ChartFileName}.*", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(encryptedFilePath) || !File.Exists(encryptedFilePath))
                return default;

            var decryptCMD = $"decrypt \"{encryptedFilePath}\" -o \"{outputTempFolder}\" -k \"{KEY}\"";

            Utils.SimpleExec(decryptCMD, maiConverterFilePath);

            var sdtFilePath = Directory.GetFiles(outputTempFolder).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(sdtFilePath) || !File.Exists(sdtFilePath))
                return default;

            outputTempFolder = Utils.GetTempFolderPath();
            var convertCMD = $"sdttoma2 \"{sdtFilePath}\" -o \"{outputTempFolder}\" -b {target.Bpm}";

            Utils.SimpleExec(convertCMD, maiConverterFilePath);

            var ma2FilePath = Directory.GetFiles(outputTempFolder).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(ma2FilePath) || !File.Exists(ma2FilePath))
                return default;

            var ma2Content = File.ReadAllText(ma2FilePath);
            var result = new Ma2GenerateResult();
            result.Ma2Content = ma2Content;

            var match = noteCountRegex.Match(result.Ma2Content);
            if (match.Success)
                result.NoteCount = int.Parse(match.Groups[1].Value);

            return result;
        }
    }
}
