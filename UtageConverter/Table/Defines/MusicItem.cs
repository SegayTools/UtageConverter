namespace UtageConverter.Table.Defines
{
    [TableName("MMMUSIC")]
    public class MusicItem
    {
        [TableFieldOrder(0)]
        public int ID { get; set; }
        [TableFieldOrder(1)]
        public string Name { get; set; }
        [TableFieldOrder(2)]
        public int Ver { get; set; }
        [TableFieldOrder(3)]
        public int SubCate { get; set; }
        [TableFieldOrder(4)]
        public float BPM { get; set; }
        [TableFieldOrder(5)]
        public int SortID { get; set; }
        [TableFieldOrder(6)]
        public int ドレス { get; set; }
        [TableFieldOrder(7)]
        public int 暗黒 { get; set; }
        [TableFieldOrder(8)]
        public int mile { get; set; }
        [TableFieldOrder(9)]
        public int VL { get; set; }
        [TableFieldOrder(10)]
        public int Event { get; set; }
        [TableFieldOrder(11)]
        public int Rec { get; set; }
        [TableFieldOrder(12)]
        public float PVStart { get; set; }
        [TableFieldOrder(13)]
        public float PVEnd { get; set; }
        [TableFieldOrder(14)]
        public int 曲長さ { get; set; }
        [TableFieldOrder(15)]
        public int オフRanking { get; set; }
        [TableFieldOrder(16)]
        public int ADDef { get; set; }
        [TableFieldOrder(17)]
        public int ReMaster { get; set; }
        [TableFieldOrder(18)]
        public int 特殊PV { get; set; }
        [TableFieldOrder(19)]
        public int チャレンジトラック { get; set; }
        [TableFieldOrder(20)]
        public int ボーナス { get; set; }
        [TableFieldOrder(21)]
        public int GenreID { get; set; }
        [TableFieldOrder(22)]
        public string タイトル { get; set; }
        [TableFieldOrder(23)]
        public string アーティスト { get; set; }
        [TableFieldOrder(24)]
        public int sort_jp_index { get; set; }
        [TableFieldOrder(25)]
        public int sort_ex_index { get; set; }
        [TableFieldOrder(26)]
        public string filename { get; set; }
    }
}
