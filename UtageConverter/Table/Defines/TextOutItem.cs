using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtageConverter.Table.Defines
{
    [TableName("MMTEXTOUT")]
    public class TextOutItem
    {
        [TableFieldOrder(0)]
        public string Name { get; set; }
        [TableFieldOrder(1)]
        public string Content { get; set; }
    }
}
