using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UtageConverter.Bases;
using UtageConverter.Table;
using UtageConverter.Table.Defines;

namespace UtageConverter
{
    public class TableLoader
    {
        public List<FesListItem> FesList { get; }
        public List<MusicItem> MusicList { get; }
        public List<ScoreItem> ScoreList { get; }
        public Dictionary<string, string> TextoutMap { get; }

        private static Regex regex = new Regex(@"eScore_(\d+)");

        public TableLoader(string tableFolderPath, string maiConverterPath, string key)
        {
            List<T> GetTableItemList<T>(string fileName) where T : new()
            {
                var outputTempFolder = Utils.GetTempFolderPath();
                var decryptCMD = $"decrypt \"{Path.Combine(tableFolderPath, fileName)}\" -o \"{outputTempFolder}\" -k \"{key}\"";
                Utils.SimpleExec(decryptCMD, maiConverterPath);

                var decryptedFilePath = Directory.GetFiles(outputTempFolder).FirstOrDefault();

                Utils.Log($"decrypted {Path.GetFileName(tableFolderPath)} file: {decryptedFilePath}");
                if (string.IsNullOrWhiteSpace(decryptedFilePath) || !File.Exists(decryptedFilePath))
                    return new List<T>();

                using var fs = File.OpenRead(decryptedFilePath);
                return TableSerialize.TrySerializeTableItems<T>(fs);
            }

            FesList = GetTableItemList<FesListItem>("mmFesList.bin");
            MusicList = GetTableItemList<MusicItem>("mmMusic.bin");
            ScoreList = GetTableItemList<ScoreItem>("mmScore.bin");
            TextoutMap = GetTableItemList<TextOutItem>("mmtextout_jp.bin").ToDictionary(x => x.Name, x => x.Content);
        }

        public IEnumerable<GenerateTarget> BuildGenerateTargets()
        {
            string GetString(string key)
            {
                if (TextoutMap.TryGetValue(key, out var str))
                    return str;
                return string.Empty;
            }

            foreach (var fesItem in FesList)
            {
                var target = new GenerateTarget();

                if (ScoreList.FirstOrDefault(x => x.ID == fesItem.ScoreID) is not ScoreItem scoreItem)
                    continue;
                if (!(regex.Match(scoreItem.Name) is Match match && match.Success))
                    continue;
                var musicId = int.Parse(match.Groups[1].Value);
                if (MusicList.FirstOrDefault(x => x.ID == musicId) is not MusicItem musicItem)
                    continue;

                target.Id = musicItem.ID;

                target.Comment = GetString(fesItem.RstCommentID);
                target.Artist = GetString(musicItem.アーティスト);
                target.Title = GetString(musicItem.タイトル);
                target.Bpm = musicItem.BPM;
                target.Level = scoreItem.LV;
                target.ChartFileName = scoreItem.safename;
                target.DiffId = int.TryParse(target.ChartFileName.LastOrDefault().ToString(), out var d) ? d : 0;

                var designerId = $"RST_SCORECREATOR_{scoreItem.譜面作者ID.ToString().PadLeft(4, '0')}";
                target.Designer = GetString(designerId);

                target.UtageTypeId = fesItem.Dif;
                target.FixedJudgeOptions = (JudgeOptions)fesItem.Judge;
                target.FixedDisplayOptions = (DisplayOptions)fesItem.Disp;
                target.FixedMirrorOptions = (MirrorOptions)fesItem.Mirror;
                target.FixedSkipBorderOptions = (SkipBorderOptions)fesItem.Skip;

                yield return target;
            }
        }
    }
}
