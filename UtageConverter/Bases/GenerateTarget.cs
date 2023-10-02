using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtageConverter.Table.Defines;

namespace UtageConverter.Bases
{
    public class GenerateTarget
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public float Level { get; set; }
        public float Bpm { get; set; }
        public int UtageTypeId { get; set; }

        public string UtageTypeString => UtageTypeId switch
        {
            1 => "宴",
            2 => "狂",
            3 => "蔵",
            4 => "耐",
            5 => "蛸",
            6 => "光",
            7 => "星",
            8 => "傾",
            11 => "覚",
            12 => "協",
            13 => "逆",
            15 => "即",
            16 => "撫",
            _ => string.Empty
        };

        public MirrorOptions FixedMirrorOptions { get; set; }
        public JudgeOptions FixedJudgeOptions { get; set; }

        public SkipBorderOptions FixedSkipBorderOptions { get; set; }
        public DisplayOptions FixedDisplayOptions { get; set; }
        public string Designer { get; set; }
    }
}
