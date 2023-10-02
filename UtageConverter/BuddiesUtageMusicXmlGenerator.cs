using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using UtageConverter.Bases;

namespace UtageConverter
{
    public static class BuddiesUtageMusicXmlGenerator
    {
        public static string Generate(GenerateTarget target)
        {
            using var fs = typeof(BuddiesUtageMusicXmlGenerator).Assembly.GetManifestResourceStream($"UtageConverter.Resources.MusicTemplate.xml");
            var xmlContent = new StreamReader(fs).ReadToEnd();

            void replace(string name, object value)
            {
                var replaceKey = $"${{{name}}}";
                xmlContent = xmlContent.Replace(replaceKey, value.ToString());
            }

            var buddiesMusicId = 100000 + target.Id;
            replace("buddiesMusicId", buddiesMusicId);
            replace("musicId", target.Id);

            var title = SecurityElement.Escape($"[{target.UtageTypeString}]{target.Title}");
            replace("buddiesTitle", title);
            replace("title", SecurityElement.Escape(target.Title));

            replace("sortName", SecurityElement.Escape(target.Title));
            replace("artist", SecurityElement.Escape(target.Artist));
            replace("Bpm", target.Bpm);

            replace("utageTypeString", target.UtageTypeString);
            replace("utageComment", target.Comment);
            replace("utageTypeString", target.UtageTypeString);
            replace("designer", target.Designer);

            replace("level", (int)target.Level);
            replace("levelDecimal", ((int)(target.Level * 100)) % 100);

            replace("utageFixedMirrorName", target.FixedMirrorOptions switch
            {
                MirrorOptions.None => "None",
                _ => "Mirror"
            });
            replace("utageFixedMirrorValue", target.FixedMirrorOptions switch
            {
                MirrorOptions.Normal => "Normal",
                MirrorOptions.Mirror => "LR",
                MirrorOptions.Filp => "UD",
                MirrorOptions.Rotate => "LRUD",
                _ => "Off"
            });

            replace("utageFixedDisplayName", target.FixedDisplayOptions switch
            {
                DisplayOptions.AchievementDown or
                DisplayOptions.BorderS or
                DisplayOptions.BorderSS or
                DisplayOptions.BorderSSS => "DispCenter",
                _ => "None"
            });
            replace("utageFixedDisplayValue", target.FixedDisplayOptions switch
            {
                DisplayOptions.AchievementDown => "AchiveMinus1",
                DisplayOptions.BorderS => "BoarderS",
                DisplayOptions.BorderSS => "BoarderSS",
                DisplayOptions.BorderSSS => "BoarderSSS",
                _ => "Off"
            });

            replace("utageFixedJudgeName", "None");
            replace("utageFixedJudgeValue", "Off");

            replace("utageFixedSkipBorderName", target.FixedSkipBorderOptions switch
            {
                SkipBorderOptions.S or
                SkipBorderOptions.SS or
                SkipBorderOptions.SSS or
                SkipBorderOptions.OFF => "TrackSkip",
                _ => "None"
            });
            replace("utageFixedSkipBorderValue", target.FixedSkipBorderOptions switch
            {
                SkipBorderOptions.S => "AutoS",
                SkipBorderOptions.SS => "AutoSS",
                SkipBorderOptions.SSS => "AutoSSS",
                SkipBorderOptions.OFF or _ => "Off"
            });

            return xmlContent;
        }
    }
}
