using System;

namespace UtageConverter.Table
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class TableNameAttribute : Attribute
    {
        public TableNameAttribute(string tableName)
        {
            TableName = tableName;
        }

        public string TableName { get; }
    }
}
