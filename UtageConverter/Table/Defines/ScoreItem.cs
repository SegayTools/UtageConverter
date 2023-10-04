namespace UtageConverter.Table.Defines
{
    [TableName("MMSCORE")]
    public class ScoreItem
    {
        [TableFieldOrder(0)]
        public int ID { get; set; }
        [TableFieldOrder(1)]
        public string Name { get; set; }
        [TableFieldOrder(2)]
        public float LV { get; set; }
        [TableFieldOrder(3)]
        public int 譜面作者ID { get; set; }
        [TableFieldOrder(4)]
        public int 計算対象 { get; set; }
        [TableFieldOrder(5)]
        public string safename { get; set; }
    }
}
