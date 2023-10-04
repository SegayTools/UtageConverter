using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace UtageConverter.Table
{
    public static class TableSerialize
    {
        public static List<T> TrySerializeTableItems<T>(Stream stream) where T : new()
        {
            var streamReader = new StreamReader(stream);
            var type = typeof(T);
            var result = new List<T>();

            if (type.GetCustomAttribute<TableNameAttribute>() is not TableNameAttribute tableNameAttr)
                throw new Exception($"type {type.Name} not contain TableNameAttribute");

            var setters = type.GetProperties().Select(x =>
            {
                if (x.GetCustomAttribute<TableFieldOrderAttribute>() is not TableFieldOrderAttribute order)
                    return default;
                return new
                {
                    order.Order,
                    Setter = new Action<string, T>((str, obj) =>
                    {
                        if (TypeDescriptor.GetConverter(x.PropertyType) is TypeConverter converter)
                        {
                            var val = converter.ConvertFrom(str);
                            if (val is string s && (s.StartsWith("\"") || s.StartsWith("L\"")) && s.EndsWith("\"") && s.Length >= 2)
                            {
                                var start = s.StartsWith("\"") ? 1 : 2;
                                val = s.Substring(start, s.Length - (1 + start));
                            }
                            x.SetValue(obj, val);
                        }
                    })
                };
            }).Where(x => x != null).ToDictionary(x => x.Order, x => x.Setter);

            var fieldName = tableNameAttr.TableName;
            if (!string.IsNullOrWhiteSpace(fieldName))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine().Trim();
                    if (!line.StartsWith(fieldName, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    var stack = 0;

                    var subString = "";
                    if (line.Contains("ARM＋夕野ヨシミ"))
                    {

                    }

                    for (int i = 0; i < line.Length; i++)
                    {
                        var ch = line[i];
                        if (ch == '(')
                        {
                            if (stack > 0)
                                subString += ch;
                            stack++;
                        }
                        else if (ch == ')')
                        {
                            stack--;
                            if (stack == 0)
                                break;
                            else
                                subString += ch;
                        }
                        else if (stack > 0)
                            subString += ch;

                    }

                    var split = subString.Trim().Split(',').Select(x => x.Trim()).ToArray();

                    var obj = new T();

                    foreach (var pair in setters)
                    {
                        var valStr = split.ElementAtOrDefault(pair.Key);

                        pair.Value(valStr, obj);
                    }

                    result.Add(obj);
                }
            }

            return result;
        }
    }
}
