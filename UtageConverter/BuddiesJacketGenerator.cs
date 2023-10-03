using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UtageConverter.Bases;
using System.Security.Cryptography;

namespace UtageConverter
{
    public class BuddiesJacketGenerator
    {
        public static (string jacketFilePath, string jacketSmallFilePath) Generate(GenerateTarget target, string jacketFolderPath)
        {
            var ddsFilePath = Directory.GetFiles(jacketFolderPath, $"{target.Id.ToString().PadLeft(3, '0')}_*.dds").FirstOrDefault();
            if (!File.Exists(ddsFilePath))
            {
                Console.WriteLine($"No .dds file: {ddsFilePath}");
                return default;
            }

            var outputPngFilePath = Utils.GetTempFilePath();
            using var image = new MagickImage(ddsFilePath);
            image.Format = MagickFormat.Png;
            image.Write(outputPngFilePath);

            if (!File.Exists(outputPngFilePath))
            {
                Console.WriteLine($"can't generate .png file: {ddsFilePath}");
                return default;
            }

            var programFolderPath = Path.GetDirectoryName(typeof(BuddiesAudioGenerator).Assembly.Location);
            var generatorStreamExePath = Path.Combine(programFolderPath, "jacketGenerator", "JacketGenerator.exe");
            if (!File.Exists(generatorStreamExePath))
            {
                Console.WriteLine($"JacketGenerator.exe file not found: {generatorStreamExePath}");
                return default;
            }

            return (Generate(generatorStreamExePath, outputPngFilePath, target.Id, false, 512, 512), Generate(generatorStreamExePath, outputPngFilePath, target.Id, true, 220, 220));
        }

        private static string Generate(string exePath, string pngFilePath, int musicId, bool isSmall, int width, int height)
        {
            var fileName = $"UI_Jacket_{musicId.ToString().PadLeft(6, '0')}{(isSmall ? "_S" : string.Empty)}.png";
            var outputTempFolder = Utils.GetTempFolderPath();
            var copyPngFilePath = Path.Combine(outputTempFolder, fileName);
            File.Copy(pngFilePath, copyPngFilePath, true);

            var cmd = $"--mode GenerateJacket --inputFiles \"{copyPngFilePath}\" --outputFolder \"{outputTempFolder}\" --gameType SDEZ ";
            cmd += $"--addWatermark --watermarkProof UtageConvert --noPause --watermarkPassword LoveFromDP";

            Utils.SimpleExec(cmd, exePath);

            var abFile = Directory.GetFiles(outputTempFolder, "*.ab").FirstOrDefault();
            if (!File.Exists(abFile))
            {
                Console.WriteLine($"Can't generate .ab file, cmd: {cmd}");
                return exePath;
            }

            return abFile;
        }
    }
}
