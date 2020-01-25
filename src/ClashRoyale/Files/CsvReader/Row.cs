using System;
using System.Collections.Generic;
using System.Reflection;
using ClashRoyale.Files.CsvHelpers;

namespace ClashRoyale.Files.CsvReader
{
    public class Row
    {
        private readonly Table _table;
        public readonly int RowStart;

        public Row(Table table)
        {
            _table = table;
            RowStart = _table.GetColumnRowCount();

            _table.AddRow(this);
        }

        public int Offset => RowStart;

        public void LoadData(Data data)
        {
            foreach (var property in data.GetType().GetProperties(BindingFlags.Instance |
                                                                  BindingFlags.NonPublic |
                                                                  BindingFlags.Public))
                if (property.CanRead && property.CanWrite)
                {
                    if (property.PropertyType.IsArray)
                    {
                        var elementType = property.PropertyType.GetElementType();

                        if (elementType == typeof(byte)) property.SetValue(data, LoadBoolArray(property.Name));
                        else if (elementType == typeof(int)) property.SetValue(data, LoadIntArray(property.Name));
                        else if (elementType == typeof(string)) property.SetValue(data, LoadStringArray(property.Name));
                    }
                    else if (property.PropertyType.IsGenericType)
                    {
                        if (property.PropertyType == typeof(List<>))
                        {
                            var listType = typeof(List<>);
                            var generic = property.PropertyType.GetGenericArguments();
                            var concreteType = listType.MakeGenericType(generic);
                            var newList = Activator.CreateInstance(concreteType);
                            var add = concreteType.GetMethod("Add");
                            var indexerName =
                                ((DefaultMemberAttribute)newList.GetType()
                                    .GetCustomAttributes(typeof(DefaultMemberAttribute), true)[0]).MemberName;
                            var indexProperty = newList.GetType().GetProperty(indexerName);

                            for (var i = Offset; i < Offset + GetArraySize(property.Name); i++)
                            {
                                var value = GetValue(property.Name, i - Offset);

                                if (value == string.Empty && i != Offset)
                                    if (indexProperty != null)
                                        value = indexProperty.GetValue(newList, new object[]
                                        {
                                            i - Offset - 1
                                        }).ToString();

                                if (string.IsNullOrEmpty(value))
                                {
                                    var Object = generic[0].IsValueType
                                        ? Activator.CreateInstance(generic[0])
                                        : string.Empty;

                                    if (add != null)
                                        add.Invoke(newList, new[]
                                        {
                                            Object
                                        });
                                }
                                else
                                {
                                    if (add != null)
                                        add.Invoke(newList, new[]
                                        {
                                            Convert.ChangeType(value, generic[0])
                                        });
                                }
                            }

                            property.SetValue(data, newList);
                        }
                        else if (property.PropertyType == typeof(Data) ||
                                 property.PropertyType.BaseType == typeof(Data))
                        {
                            var pData = (Data)Activator.CreateInstance(property.PropertyType);
                            LoadData(pData);
                            property.SetValue(data, pData);
                        }
                    }
                    else
                    {
                        var value = GetValue(property.Name, 0);

                        if (!string.IsNullOrEmpty(value))
                            property.SetValue(data, Convert.ChangeType(value, property.PropertyType));
                    }
                }
        }

        public int GetArraySize(string name)
        {
            var index = _table.GetColumnIndexByName(name);
            return index != -1 ? _table.GetArraySizeAt(this, index) : 0;
        }

        public string GetName()
        {
            return _table.GetValueAt(0, RowStart);
        }


        public string GetValue(string name, int level)
        {
            return _table.GetValue(name, level + RowStart);
        }

        private bool[] LoadBoolArray(string column)
        {
            var array = new bool[GetArraySize(column)];

            for (var i = 0; i < array.Length; i++)
            {
                var value = GetValue(column, i);

                if (string.IsNullOrEmpty(value)) continue;

                if (bool.TryParse(value, out var boolean))
                    array[i] = boolean;
            }

            return array;
        }

        private int[] LoadIntArray(string column)
        {
            var array = new int[GetArraySize(column)];

            for (var i = 0; i < array.Length; i++)
            {
                var value = GetValue(column, i);

                if (string.IsNullOrEmpty(value)) continue;

                if (int.TryParse(value, out var number))
                    array[i] = number;
            }

            return array;
        }

        private string[] LoadStringArray(string column)
        {
            var array = new string[GetArraySize(column)];

            for (var i = 0; i < array.Length; i++) array[i] = GetValue(column, i);

            return array;
        }
    }
}