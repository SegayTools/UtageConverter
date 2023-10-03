using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UtageConverter;
using UtageConverter.Bases;
using UtageConverter.Table;
using UtageConverter.Table.Defines;

internal class Program
{
    private static void Main(string[] args)
    {
        var folderPath = @"D:\maimai FiNALE (SDEY 1.99.00)\maimai\data";
        var maiConverterExePath = @"F:\Source\MaiConverter\dist\maiconverter.exe";
        var outputFolder = @"F:\SDEZ_1.40\Package\option\H202";
        var buddiesPackagesFolder = @"F:\SDEZ_1.40\Package";
        var key = "0x4E2039D03C7AC1A36D10BAC801321D26";

        var tableLoader = new TableLoader(Path.Combine(folderPath, "tables"), maiConverterExePath, key);
        var dataLoader = new BuddiesDataLoader(buddiesPackagesFolder);
        var targetsMap = tableLoader.BuildGenerateTargets();

        //PrintTargets(targets);
        Directory.CreateDirectory(Path.Combine(outputFolder, "music"));
        Directory.CreateDirectory(Path.Combine(outputFolder, "SoundData"));
        Directory.CreateDirectory(Path.Combine(outputFolder, "AssetBundleImages", "jacket"));
        Directory.CreateDirectory(Path.Combine(outputFolder, "AssetBundleImages", "jacket_s"));

        var scoreFolderPath = Path.Combine(folderPath, "score");
        var soundFolderPath = Path.Combine(folderPath, "sound");
        var jacketFolderPath = Path.Combine(folderPath, "sprite", "movie_thumbnail");

        foreach (var target in targetsMap)
        {
            if (target.Title.Contains("ROCK"))
            {

            }
            Console.WriteLine($"-----------------------");
            Console.WriteLine($"Processing [{target.Id}-{target.DiffId}]{target.Artist} - {target.Title}");
            var genResult = BuddiesMa2Generator.GenerateMa2(target, scoreFolderPath, maiConverterExePath);
            if (genResult is null)
            {
                Console.WriteLine($"[x]\t\tSkip : No chart file.");
                continue;
            }
            if (!dataLoader.ContainMusic(target.Id))
            {
                var outputSoundFolder = BuddiesAudioGenerator.Generate(target, soundFolderPath);
                if (!Directory.Exists(outputSoundFolder))
                {
                    Console.WriteLine($"[x]\t\tSkip : No audio file generated.");
                    continue;
                }

                foreach (var filePath in Directory.GetFiles(outputSoundFolder))
                    File.Copy(filePath, Path.Combine(outputFolder, "SoundData", Path.GetFileName(filePath)), true);
            }
            var musicXmlContent = BuddiesUtageMusicXmlGenerator.Generate(target, genResult);

            var buddiesMusicId = target.Id + 100000 * (target.DiffId + 1);
            if (dataLoader.ContainMusicDiff(buddiesMusicId, target.DiffId))
            {
                Console.WriteLine($"[x]\t\tSkip : Utage fumen has been registered.");
                continue;
            }
            var outputMusicFolder = Path.Combine(outputFolder, "music", $"music{buddiesMusicId}");
            Directory.CreateDirectory(outputMusicFolder);
            File.WriteAllText(Path.Combine(outputMusicFolder, "Music.xml"), musicXmlContent);

            File.WriteAllText(Path.Combine(outputMusicFolder, $"{buddiesMusicId}_00.ma2"), genResult.Ma2Content);
            if (!dataLoader.ContainJacket(target.Id))
            {
                Console.WriteLine($"[?]\t\tWarn : No jacket file,try to create new...");
                //generate jacket
                (var jacketFilePath, var jacketSmallFilePath) = BuddiesJacketGenerator.Generate(target, jacketFolderPath);
                if (File.Exists(jacketFilePath))
                {
                    var fileName = Path.GetFileName(jacketFilePath);
                    File.Copy(jacketFilePath, Path.Combine(outputFolder, "AssetBundleImages", "jacket", fileName), true);
                    Console.WriteLine($"Generated jacketFilePath.");
                }
                if (File.Exists(jacketSmallFilePath))
                {
                    var fileName = Path.GetFileName(jacketSmallFilePath);
                    File.Copy(jacketSmallFilePath, Path.Combine(outputFolder, "AssetBundleImages", "jacket_s", fileName), true);
                    Console.WriteLine($"Generated jacketSmallFilePath.");
                }
            }
            Console.WriteLine($"[o]\tGenerated! BuddiesMusicId: {buddiesMusicId}");
        }

        Console.WriteLine($"-----------------------");
        Console.WriteLine($"DONE.");
    }

    private static void PrintTargets(GenerateTarget[] targets)
    {
        var props = typeof(GenerateTarget).GetProperties();

        Console.WriteLine(string.Join(",\t", props.Select(x => x.Name)));
        foreach (var target in targets)
            Console.WriteLine(string.Join(",\t", props.Select(x => x.GetValue(target))));
    }
}