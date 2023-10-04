using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UtageConverter;
using UtageConverter.Bases;

internal class Program
{
    private static void Main(string[] args)
    {
        var folderPath = @"D:\maimai FiNALE (SDEY 1.99.00)\maimai\data";
        var maiConverterExePath = @"F:\Source\MaiConverter\dist\maiconverter.exe";
        var outputFolder = @"F:\option\H202";
        var buddiesPackagesFolder = @"F:\A000";
        var key = "0x4E2039D03C7AC1A36D10BAC801321D26";
        var dxkey = "0x7F4551499DF55E68";
        var ffmpegFolderPath = @"D:\usmTest\Publish\FFmpeg";
        var wannaCriFolderPath = @"D:\usmTest\Publish\Python\python.exe";

        var tableLoader = new TableLoader(Path.Combine(folderPath, "tables"), maiConverterExePath, key);
        var dataLoader = new BuddiesDataLoader(buddiesPackagesFolder);
        var targetsMap = tableLoader.BuildGenerateTargets();

        //PrintTargets(targets);
        Directory.CreateDirectory(Path.Combine(outputFolder, "music"));
        Directory.CreateDirectory(Path.Combine(outputFolder, "SoundData"));
        Directory.CreateDirectory(Path.Combine(outputFolder, "MovieData"));
        Directory.CreateDirectory(Path.Combine(outputFolder, "AssetBundleImages", "jacket"));
        Directory.CreateDirectory(Path.Combine(outputFolder, "AssetBundleImages", "jacket_s"));

        var scoreFolderPath = Path.Combine(folderPath, "score");
        var soundFolderPath = Path.Combine(folderPath, "sound");
        var movieFolderPath = Path.Combine(folderPath, "movie");
        var jacketFolderPath = Path.Combine(folderPath, "sprite", "movie_thumbnail");

        var genSounds = new HashSet<int>();
        var genJackets = new HashSet<int>();
        var genMovies = new HashSet<int>();

        foreach (var target in targetsMap)
        {
            Utils.Log($"-----------------------");
            Utils.Log($"Processing [{target.Id}-{target.DiffId}]{target.Artist} - {target.Title}");
            var buddiesMusicId = target.Id + 100000 * (target.DiffId + 1);
            if (dataLoader.ContainMusicDiff(buddiesMusicId, target.DiffId))
            {
                Utils.Log($"[x]\t\tSkip : Utage fumen has been registered.", ConsoleColor.Red);
                continue;
            }

            var genResult = BuddiesMa2Generator.GenerateMa2(target, scoreFolderPath, maiConverterExePath);
            if (genResult is null)
            {
                Utils.Log($"[x]\t\tSkip : No chart file.", ConsoleColor.Red);
                continue;
            }
            if ((!dataLoader.ContainMusic(target.Id)) && !genSounds.Contains(target.Id))
            {
                var outputSoundFolder = BuddiesAudioGenerator.Generate(target, soundFolderPath);
                if (!Directory.Exists(outputSoundFolder))
                {
                    Utils.Log($"[x]\t\tSkip : No audio file generated.", ConsoleColor.Red);
                    continue;
                }

                foreach (var filePath in Directory.GetFiles(outputSoundFolder))
                    File.Copy(filePath, Path.Combine(outputFolder, "SoundData", Path.GetFileName(filePath)), true);

                genSounds.Add(target.Id);
            }
            var musicXmlContent = BuddiesUtageMusicXmlGenerator.Generate(target, genResult);

            var outputMusicFolder = Path.Combine(outputFolder, "music", $"music{buddiesMusicId}");
            Directory.CreateDirectory(outputMusicFolder);
            File.WriteAllText(Path.Combine(outputMusicFolder, "Music.xml"), musicXmlContent);

            File.WriteAllText(Path.Combine(outputMusicFolder, $"{buddiesMusicId}_00.ma2"), genResult.Ma2Content);
            if ((!dataLoader.ContainJacket(target.Id)) && !genJackets.Contains(target.Id))
            {
                Utils.Log($"[?]\t\tWarn : No jacket file,try to create new...");
                //generate jacket
                (var jacketFilePath, var jacketSmallFilePath) = BuddiesJacketGenerator.Generate(target, jacketFolderPath);
                if (File.Exists(jacketFilePath))
                {
                    var fileName = Path.GetFileName(jacketFilePath);
                    File.Copy(jacketFilePath, Path.Combine(outputFolder, "AssetBundleImages", "jacket", fileName), true);
                    Utils.Log($"Generated jacketFilePath.");
                }
                if (File.Exists(jacketSmallFilePath))
                {
                    var fileName = Path.GetFileName(jacketSmallFilePath);
                    File.Copy(jacketSmallFilePath, Path.Combine(outputFolder, "AssetBundleImages", "jacket_s", fileName), true);
                    Utils.Log($"Generated jacketSmallFilePath.");
                }

                genJackets.Add(target.Id);
            }
            if ((!dataLoader.ContainMovie(target.Id)) && !genMovies.Contains(target.Id))
            {
                Utils.Log($"[?]\t\tWarn : No movie file,try to create new...");
                //generate movie
                var outputUsmFilePath = BuddiesMovieGenerator.Generate(target, movieFolderPath, jacketFolderPath, ffmpegFolderPath, wannaCriFolderPath, dxkey);
                if (File.Exists(outputUsmFilePath))
                {
                    File.Copy(outputUsmFilePath, Path.Combine(outputFolder, "MovieData", $"{target.Id.ToString().PadLeft(6, '0')}.dat"), true);
                    Utils.Log($"Generated outputUsmFilePath.");
                }
                else
                {
                    Utils.Log($"No movie file generated.", ConsoleColor.Yellow);
                }

                genMovies.Add(target.Id);
            }
            Utils.Log($"[o]\tGenerated! BuddiesMusicId: {buddiesMusicId}", ConsoleColor.Green);
        }

        Utils.Log($"-----------------------");
        Utils.Log($"DONE.");
    }

    private static void PrintTargets(GenerateTarget[] targets)
    {
        var props = typeof(GenerateTarget).GetProperties();

        Utils.Log(string.Join(",\t", props.Select(x => x.Name)));
        foreach (var target in targets)
            Utils.Log(string.Join(",\t", props.Select(x => x.GetValue(target))));
    }
}
