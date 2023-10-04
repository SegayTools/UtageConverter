using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtageConverter.Bases;

namespace UtageConverter
{
    public static class BuddiesMovieGenerator
    {
        public static string Generate(GenerateTarget target, string movieFolderPath, string jacketFolderPath, string ffmpegFolderPath, string wannaCriPythonFilePath, string key)
        {
            var inputFile = string.Empty;
            var isJacketInput = false;

            {
                var movieFilePath = Directory.GetFiles(movieFolderPath, $"{target.Id.ToString().PadLeft(3, '0')}_*.wmv").FirstOrDefault();
                if (File.Exists(movieFilePath))
                    inputFile = movieFilePath;
                else
                {
                    var jacketFilePath = Directory.GetFiles(jacketFolderPath, $"{target.Id.ToString().PadLeft(3, '0')}_*.dds").FirstOrDefault();
                    if (File.Exists(jacketFilePath))
                    {
                        inputFile = BuddiesJacketGenerator.GeneratePng(jacketFilePath);
                        isJacketInput = true;
                    }
                }
            }

            if (!File.Exists(inputFile))
            {
                Utils.Log($"No movie file.", ConsoleColor.Red);
                return default;
            }

            var outputVideoFile = Utils.GetTempFilePath();

            var ffmpegCmd = $"-i \"{inputFile}\" -c:v h264 -b:v 0 -vf \"fps=60\" -s 1080x1080 -threads 16 -f h264 -an {outputVideoFile}";
            if (isJacketInput)
                ffmpegCmd += $"-loop 1 -t 10";

            var ffmpegExePath = Path.Combine(ffmpegFolderPath, "ffmpeg.exe");
            if (!File.Exists(ffmpegExePath))
            {
                Utils.Log($"ffmpeg.exe not found: {ffmpegExePath}", ConsoleColor.Red);
                return default;
            }

            Utils.SimpleExec(ffmpegCmd, ffmpegExePath);

            if (!File.Exists(outputVideoFile))
            {
                Utils.Log($"ffmpeg not generate video file.", ConsoleColor.Red);
                return default;
            }

            var wannacriCmd = $"-m wannacri createusm \"{outputVideoFile}\" --key {key}";
            Utils.SimpleExec(wannacriCmd, wannaCriPythonFilePath, ffmpegFolderPath);
            var outputUsmFilePath = Path.ChangeExtension(outputVideoFile, ".usm");
            if (!File.Exists(outputUsmFilePath))
            {
                Utils.Log($"wannaCri not generate .usm file.", ConsoleColor.Red);
                return default;
            }

            return outputUsmFilePath;
        }
    }
}
