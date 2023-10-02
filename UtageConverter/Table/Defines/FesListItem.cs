using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtageConverter.Table.Defines
{
    [TableName("MMFESLIST")]
    public class FesListItem
    {
        [TableFieldOrder(0)]
        public string Name { get; set; }
        [TableFieldOrder(1)]
        public int ID { get; set; }
        [TableFieldOrder(2)]
        public int EVENT { get; set; }
        [TableFieldOrder(3)]
        public int SordID { get; set; }
        [TableFieldOrder(4)]
        public int ScoreID { get; set; }
        [TableFieldOrder(5)]
        public int Dif { get; set; }
        [TableFieldOrder(6)]
        public int Creator { get; set; }
        [TableFieldOrder(7)]
        public int Mirror { get; set; }
        [TableFieldOrder(8)]
        public int Disp { get; set; }
        [TableFieldOrder(9)]
        public int Skip { get; set; }
        [TableFieldOrder(10)]
        public int Judge { get; set; }
        [TableFieldOrder(11)]
        public string RstCommentID { get; set; }
    };
}
