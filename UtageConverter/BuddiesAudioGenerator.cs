using System.IO;
using UtageConverter.Bases;

namespace UtageConverter
{
    public class BuddiesAudioGenerator
    {
        public static string Generate(GenerateTarget target, string soundFolderPath)
        {
            var xwbFilePath = Path.Combine(soundFolderPath, $"{target.Id.ToString().PadLeft(3, '0')}.xwb");
            var outputWavFilePath = Utils.GetTempFilePath();

            if (!File.Exists(xwbFilePath))
            {
                Utils.Log($"No .xwb file: {xwbFilePath}");
                return default;
            }

            var programFolderPath = Path.GetDirectoryName(typeof(BuddiesAudioGenerator).Assembly.Location);
            var vgmStreamExePath = Path.Combine(programFolderPath, "vgmstream", "vgmstream-cli.exe");

            if (!File.Exists(vgmStreamExePath))
            {
                Utils.Log($"No vgmstream-cli.exe: {vgmStreamExePath}");
                return default;
            }

            Utils.SimpleExec($"-o \"{outputWavFilePath}\" -i \"{xwbFilePath}\"", vgmStreamExePath);

            if (!File.Exists(outputWavFilePath))
            {
                Utils.Log($"vgmstream-cli.exe can't decode .xwb file: {xwbFilePath}");
                return default;
            }

            var outputAcbFolderPath = Utils.GetTempFolderPath();
            var outputAcbFileName = $"music{target.Id.ToString().PadLeft(6, '0')}";

            if ((!AcbGeneratorFuck.Generator.Generate(outputWavFilePath, outputAcbFileName, outputAcbFolderPath, false, new VGAudio.Cli.Options()
            {
                Bitrate = 192 * 1024,
                KeyCode = 9170825592834449000,
            }, 0, 0, true)) || Directory.GetFiles(outputAcbFolderPath).Length < 2)
            {
                Utils.Log($"generate .acb/.awb file failed");
                return default;
            }

            return outputAcbFolderPath;
        }
    }
}
