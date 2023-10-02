using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtageConverter.Table
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class TableFieldOrderAttribute : Attribute
    {
        public TableFieldOrderAttribute(int order)
        {
            Order = order;
        }

        public int Order { get; }
    }
}
