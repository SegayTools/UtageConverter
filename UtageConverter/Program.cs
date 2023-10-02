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
        var folderPath = @"C:\Users\MikiraSora\Desktop\output";

        List<T> GetTableItemList<T>(string fileName) where T : new()
        {
            using var fs = File.OpenRead(Path.Combine(folderPath, fileName));
            return TableSerialize.TrySerializeTableItems<T>(fs);
        }

        var fesList = GetTableItemList<FesListItem>("mmFesList.tin");
        var musicList = GetTableItemList<MusicItem>("mmMusic.tin");
        var scoreList = GetTableItemList<ScoreItem>("mmScore.tin");
        var textoutMap = GetTableItemList<TextOutItem>("mmtextout_jp.tin").ToDictionary(x => x.Name, x => x.Content);

        var regex = new Regex(@"eScore_(\d+)");

        string GetString(string key)
        {
            if (textoutMap.TryGetValue(key, out var str))
                return str;
            return string.Empty;
        }

        IEnumerable<GenerateTarget> BuildGenerateTargets()
        {
            foreach (var fesItem in fesList)
            {
                var target = new GenerateTarget();

                if (scoreList.FirstOrDefault(x => x.ID == fesItem.ScoreID) is not ScoreItem scoreItem)
                    continue;
                if (!(regex.Match(scoreItem.Name) is Match match && match.Success))
                    continue;
                var musicId = int.Parse(match.Groups[1].Value);
                if (musicList.FirstOrDefault(x => x.ID == musicId) is not MusicItem musicItem)
                    continue;

                target.Id = musicItem.ID;

                target.Comment = GetString(fesItem.RstCommentID);
                target.Artist = GetString(musicItem.アーティスト);
                target.Title = GetString(musicItem.タイトル);
                target.Bpm = musicItem.BPM;
                target.Level = scoreItem.LV;

                var designerId = $"RST_SCORECREATOR_{scoreItem.譜面作者ID.ToString().PadLeft(4,'0')}";
                target.Designer = GetString(designerId);

                target.UtageTypeId = fesItem.Dif;
                target.FixedJudgeOptions = (JudgeOptions)fesItem.Judge;
                target.FixedDisplayOptions = (DisplayOptions)fesItem.Disp;
                target.FixedMirrorOptions = (MirrorOptions)fesItem.Mirror;
                target.FixedSkipBorderOptions = (SkipBorderOptions)fesItem.Skip;

                yield return target;
            }
        }

        var targets = BuildGenerateTargets().ToArray();

        PrintTargets(targets);

        var content = BuddiesUtageMusicXmlGenerator.Generate(targets.First());
    }

    private static void PrintTargets(GenerateTarget[] targets)
    {
        var props = typeof(GenerateTarget).GetProperties();

        Console.WriteLine(string.Join(",\t", props.Select(x => x.Name)));
        foreach (var target in targets)
            Console.WriteLine(string.Join(",\t", props.Select(x => x.GetValue(target))));
    }
}