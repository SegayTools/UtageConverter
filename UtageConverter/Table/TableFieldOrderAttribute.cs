using System;

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
